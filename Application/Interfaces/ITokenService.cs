using CoffeeShopApi.Application.DTOs;
using CoffeeShopApi.Domain.Entities;

namespace CoffeeShopApi.Application.Interfaces;

public interface ITokenService
{
    AccessToken CreateAccessToken(User user);
    GeneratedRefreshToken CreateRefreshToken();
    string HashRefreshToken(string token);
}
