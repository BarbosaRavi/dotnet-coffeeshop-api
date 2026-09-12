using CoffeeShopApi.Domain.Entities;

namespace CoffeeShopApi.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    Task<(IReadOnlyList<User> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    void Add(User user);
    void Remove(User user);
}