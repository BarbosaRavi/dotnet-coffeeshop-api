using CoffeeShopApi.Application.DTOs;
using CoffeeShopApi.Application.Interfaces;
using CoffeeShopApi.Domain.Entities;
using CoffeeShopApi.Domain.Exceptions;

namespace CoffeeShopApi.Application.Services;

public class AuthService(
    IUserRepository users,
    IRefreshTokenRepository refreshTokens,
    ITokenService tokenService,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork)
{
    private async Task<AuthResponse> IssueTokensAsync(User user, CancellationToken ct)
    {
        var accessToken = tokenService.CreateAccessToken(user);
        var refreshToken = tokenService.CreateRefreshToken();

        refreshTokens.Add(RefreshToken.Create(user.Id, refreshToken.TokenHash, refreshToken.ExpiresAt));
        await unitOfWork.SaveChangesAsync(ct);

        return new AuthResponse(
            accessToken.Token,
            accessToken.ExpiresAt,
            refreshToken.Token,
            refreshToken.ExpiresAt);
    }

    public async Task<AuthResponse> RegisterAsync(CreateUserRequest request, CancellationToken ct = default)
    {
        var email = User.NormalizeEmail(request.Email);

        if (await users.EmailExistsAsync(email, ct))
            throw new ConflictException("Email already in use.");

        var user = User.Create(request.Name, email, passwordHasher.Hash(request.Password));
        users.Add(user);

        return await IssueTokensAsync(user, ct);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var email = User.NormalizeEmail(request.Email);
        var user = await users.GetByEmailAsync(email, ct);

        if (user is null || !passwordHasher.Verify(request.Password, user.Password))
            throw new UnauthorizedException("Invalid email or password.");

        return await IssueTokensAsync(user, ct);
    }

    public async Task<AuthResponse> RefreshAsync(RefreshRequest request, CancellationToken ct = default)
    {
        var hash = tokenService.HashRefreshToken(request.RefreshToken);
        var stored = await refreshTokens.GetByHashAsync(hash, ct);

        if (stored is null || !stored.IsActive)
            throw new UnauthorizedException("Invalid refresh token.");

        stored.Revoke();
        return await IssueTokensAsync(stored.User, ct);
    }

    public async Task LogoutAsync(RefreshRequest request, CancellationToken ct = default)
    {
        var hash = tokenService.HashRefreshToken(request.RefreshToken);
        var stored = await refreshTokens.GetByHashAsync(hash, ct);

        if (stored is null || !stored.IsActive) return;

        stored.Revoke();
        await unitOfWork.SaveChangesAsync(ct);
    }
}
