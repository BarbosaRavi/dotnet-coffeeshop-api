using CoffeeShopApi.Domain.Entities;

namespace CoffeeShopApi.Application.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Product?> GetByIdIncludingDeletedAsync(Guid id, CancellationToken ct = default);
    Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    void Add(Product product);
    void Remove(Product product);
}