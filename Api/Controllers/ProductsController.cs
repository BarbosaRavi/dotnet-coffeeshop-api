using CoffeeShopApi.Application.DTOs;
using CoffeeShopApi.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShopApi.Api.Controllers;

[ApiController]
[Route("api/products")]

public class ProductsController(ProductService service): ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductResponse>>> GetAll(
        [FromQuery] PagedRequest request, CancellationToken ct) => 
            Ok(await service.GetPagedAsync(request, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> GetById(Guid id, CancellationToken ct) =>
        Ok(await service.GetByIdAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create(
        CreateProductRequest request, CancellationToken ct)
    {
        var product = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> Update(
        Guid id, UpdateProductRequest request, CancellationToken ct) =>
        Ok(await service.UpdateAsync(id, request, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> SoftDelete(Guid id, CancellationToken ct)
    {
        await service.SoftDeleteAsync(id, ct);
        return NoContent();    
    }

    [HttpPatch("{id:guid}/restore")]
    public async  Task<IActionResult> Restore(Guid id, CancellationToken ct) => 
        Ok(await service.RestoreAsync(id, ct));

    [HttpDelete("{id:guid}/permanent")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return NoContent();    
    }
}