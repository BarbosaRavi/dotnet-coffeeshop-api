using System.ComponentModel.DataAnnotations;

namespace CoffeeShopApi.Application.DTOs;

public record CreateProductRequest(
    [Required, StringLength(100, MinimumLength = 2)] string Name,
    [Required, StringLength(500)] string Description,
    [Required, Range(0.01, 10000)] decimal Price
);

public record UpdateProductRequest(
    [Required, StringLength(100, MinimumLength = 2)] string Name,
    [Required, StringLength(500)] string Description,
    [Required, Range(0.01, 10000)] decimal Price
);

public record ProductResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price
);