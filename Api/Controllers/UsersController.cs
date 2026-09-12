using CoffeeShopApi.Application.DTOs;
using CoffeeShopApi.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShopApi.Api.Controllers;

[ApiController]
[Route("api/users")]

public class UsersController(UserService service): ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<UserResponse>>> GetAll(
        [FromQuery] PagedRequest request, CancellationToken ct) =>
            Ok(await service.GetPagedResultAsync(request, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponse>> GetById(Guid id, CancellationToken ct) =>
        Ok(await service.GetByIdAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create(
        CreateUserRequest request, CancellationToken ct)
    {
        var user = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new {id = user.Id}, user);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserResponse>> Update(
        Guid id, UpdateUserRequest request, CancellationToken ct
    ) =>
        Ok(await service.UpdateAsync(id, request, ct));

    [HttpPut("{id:guid}/change-password")]
    public async Task<IActionResult> ChangePassword(Guid id, ChangePasswordRequest request, CancellationToken ct)
    {
        await service.ChangePasswordAsync(id, request, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return NoContent();
    }
}