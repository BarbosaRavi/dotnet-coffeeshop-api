using CoffeeShopApi.Domain.Entities;
using CoffeeShopApi.Domain.Exceptions;

namespace CoffeeShopApi.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt is not null;
    public bool IsActive => !IsExpired && !IsRevoked;

    private RefreshToken() {}

    public static RefreshToken Create(Guid userId, string token, DateTime expiresAt)
    {
        if (userId == Guid.Empty) throw new BusinessRuleException("User is required.");
        if (string.IsNullOrWhiteSpace(token)) throw new BusinessRuleException("Token hash cannot be empty.");
        if (expiresAt <= DateTime.UtcNow) throw new BusinessRuleException("Expiration must be in the future.");

        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow
        };
    }
    
    public void Revoke()
    {
        if (IsRevoked) throw new BusinessRuleException("Token is already revoked");

        RevokedAt = DateTime.UtcNow;
    }
}