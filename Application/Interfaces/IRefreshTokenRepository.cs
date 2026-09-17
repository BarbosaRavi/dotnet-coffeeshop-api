using CoffeeShopApi.Domain.Entities;

namespace CoffeeShopApi.Application.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken ct = default);
    Task<List<RefreshToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default);
    void Add(RefreshToken refreshToken);
}
