using CoffeeShopApi.Application.Interfaces;
using CoffeeShopApi.Domain.Entities;
using CoffeeShopApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopApi.Infrastructure.Repositories;

public class UserRepository(AppDbContext db): IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) => db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default) => db.Users.AnyAsync(u => u.Email == email, ct);

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<(IReadOnlyList<User> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var query = db.Users.AsNoTracking();
        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public void Add(User user) => db.Users.Add(user);
    public void Remove(User user) => db.Users.Remove(user);
}