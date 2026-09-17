using CoffeeShopApi.Application.Interfaces;
using CoffeeShopApi.Domain.Entities;
using CoffeeShopApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopApi.Infrastructure.Repositories;

public class RefreshTokenRepository(AppDbContext db) : IRefreshTokenRepository
{
    public Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken ct = default) =>
        db.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == tokenHash, ct);

    public Task<List<RefreshToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        db.RefreshTokens
            .Where(r => r.UserId == userId && r.RevokedAt == null && r.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(ct);


    public void Add(RefreshToken refreshToken) => db.RefreshTokens.Add(refreshToken);
}