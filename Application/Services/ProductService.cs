using CoffeeShopApi.Application.DTOs;
using CoffeeShopApi.Application.Interfaces;
using CoffeeShopApi.Application.Mappings;
using CoffeeShopApi.Domain.Entities;
using CoffeeShopApi.Domain.Exceptions;

namespace CoffeeShopApi.Application.Services;

public class ProductService(IProductRepository products, IUnitOfWork unitOfWork)
{
    private const int MaxPageSize = 100;

    public async Task<PagedResult<ProductResponse>> GetPagedAsync(PagedRequest request, CancellationToken ct = default)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, MaxPageSize);

        var (items, totalCount) = await products.GetPagedAsync(page, pageSize, ct);

        return new PagedResult<ProductResponse>(
            items.Select(p => p.ToResponse()).ToList(),
            page,
            pageSize,
            totalCount
        );
    }

    public async Task<ProductResponse> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var product = await products.GetByIdAsync(id, ct) ?? throw new NotFoundException("Product not found");

        return product.ToResponse();
    }

    public async Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken ct = default)
    {
        var product = Product.Create(request.Name, request.Description, request.Price);

        products.Add(product);
        await unitOfWork.SaveChangesAsync(ct);

        return product.ToResponse();
    }

    public async Task<ProductResponse> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default)
    {
        var product = await products.GetByIdAsync(id, ct) ?? throw new NotFoundException("Product not found");

        product.Update(request.Name, request.Description, request.Price);
        await unitOfWork.SaveChangesAsync(ct);

        return product.ToResponse();
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var product = await products.GetByIdAsync(id, ct) ?? throw new NotFoundException("Product not found");

        product.SoftDelete();
        await unitOfWork.SaveChangesAsync(ct);
    }
    
    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var product = await products.GetByIdIncludingDeletedAsync(id, ct) ?? throw new NotFoundException("Product not found");

        products.Remove(product);
        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<ProductResponse> RestoreAsync(Guid id, CancellationToken ct = default)
    {
        var product = await products.GetByIdIncludingDeletedAsync(id, ct) ?? throw new NotFoundException("Product not found");

        product.Restore();
        await unitOfWork.SaveChangesAsync(ct);

        return product.ToResponse();
    }
}