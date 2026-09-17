namespace CoffeeShopApi.Infrastructure.Security;

public sealed record JwtOptions(string Issuer, string Audience, string Secret, int AccessTokenMinutes, int RefreshTokenDays);