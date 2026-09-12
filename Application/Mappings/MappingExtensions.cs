using CoffeeShopApi.Application.DTOs;
using CoffeeShopApi.Domain.Entities;

namespace CoffeeShopApi.Application.Mappings;

public static class MappingExtensions
{
    public static ProductResponse ToResponse(this Product product) => 
        new(product.Id, product.Name, product.Description, product.Price);

    public static UserResponse ToResponse(this User user) => 
        new(user.Id, user.Name, user.Email);
}