namespace CoffeeShopApi.Application.DTOs;
using System.ComponentModel.DataAnnotations;

public record AccessToken(
    string Token, 
    DateTime ExpiresAt
);

public record GeneratedRefreshToken(
    string Token, 
    string TokenHash, 
    DateTime ExpiresAt
);

public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password
);

public record RefreshRequest(
    [Required] string RefreshToken
);

public record AuthResponse(
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt
);