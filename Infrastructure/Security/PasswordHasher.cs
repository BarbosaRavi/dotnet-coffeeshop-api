using CoffeeShopApi.Application.Interfaces;
using CoffeeShopApi.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CoffeeShopApi.Infrastructure.Security;

public class PasswordHasher: IPasswordHasher
{
    private readonly PasswordHasher<User> _hasher = new();

    public string Hash(string password) => _hasher.HashPassword(null!, password);

    public bool Verify(string password, string passwordHash) =>
        _hasher.VerifyHashedPassword(null!, passwordHash, password) 
            != PasswordVerificationResult.Failed;
}