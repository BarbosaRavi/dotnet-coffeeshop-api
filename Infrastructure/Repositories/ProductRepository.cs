using System.ComponentModel.DataAnnotations;
using CoffeeShopApi.Application.Interfaces;
using CoffeeShopApi.Domain.Entities;
using CoffeeShopApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopApi.Infrastructure.Repositories;

public class ProductRepository(AppDbContext db): IProductRepository
{
    public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default) => db.Products.FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task<Product?> GetByIdIncludingDeletedAsync(Guid id, CancellationToken ct = default) => db.Products.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var query = db.Products.AsNoTracking();
        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public void Add(Product product) => db.Products.Add(product);
    public void Remove(Product product) => db.Products.Remove(product);
}