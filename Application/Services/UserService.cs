using System.Runtime.CompilerServices;
using CoffeeShopApi.Application.DTOs;
using CoffeeShopApi.Application.Interfaces;
using CoffeeShopApi.Application.Mappings;
using CoffeeShopApi.Domain.Entities;
using CoffeeShopApi.Domain.Exceptions;

namespace CoffeeShopApi.Application.Services;

public class UserService(IUserRepository users, IRefreshTokenRepository refreshTokens, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
{
    private const int MaxPageSize = 100;

    public async Task<PagedResult<UserResponse>> GetPagedResultAsync(PagedRequest request, CancellationToken ct = default)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, MaxPageSize);

        var (items, totalCount) = await users.GetPagedAsync(page, pageSize, ct);

        return new PagedResult<UserResponse>(
            items.Select(u => u.ToResponse()).ToList(),
            page,
            pageSize,
            totalCount
        );
    }

    public async Task<UserResponse> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var user = await users.GetByIdAsync(id, ct) ?? throw new NotFoundException("User not found");

        return user.ToResponse();
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken ct = default)
    {
        var email = User.NormalizeEmail(request.Email);

        if (await users.EmailExistsAsync(email, ct)) throw new ConflictException("Email already in use.");

        var hashedPassword = passwordHasher.Hash(request.Password);
        var user = User.Create(request.Name, email, hashedPassword);

        users.Add(user);
        await unitOfWork.SaveChangesAsync(ct);

        return user.ToResponse();
    }

    public async Task<UserResponse> UpdateAsync(Guid id, Guid currentUserId, UpdateUserRequest request, CancellationToken ct = default)
    {
        EnsureSelf(id, currentUserId);
        var user = await users.GetByIdAsync(id, ct) ?? throw new NotFoundException("User not found");
        var email = User.NormalizeEmail(request.Email);

        if (email != user.Email && await users.EmailExistsAsync(email, ct)) throw new ConflictException("Email already in use.");

        user.Update(request.Name, email);
        await unitOfWork.SaveChangesAsync(ct);

        return user.ToResponse();
    }

    public async Task DeleteAsync(Guid id, Guid currentUserId, CancellationToken ct = default)
    {
        EnsureSelf(id, currentUserId);
        var user = await users.GetByIdAsync(id, ct) ?? throw new NotFoundException("User not found");
        users.Remove(user);
        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task ChangePasswordAsync(Guid id, Guid currentUserId, ChangePasswordRequest request , CancellationToken ct = default)
    {
        EnsureSelf(id, currentUserId);
        var user = await users.GetByIdAsync(id, ct) ?? throw new NotFoundException("User not found");

        if (!passwordHasher.Verify(request.CurrentPassword, user.Password)) throw new BusinessRuleException("Current password is incorrect."); 

        var hashedPassword = passwordHasher.Hash(request.NewPassword);
        
        user.ChangePassword(hashedPassword);

        var activeTokens = await refreshTokens.GetActiveByUserIdAsync(user.Id, ct);
        foreach (var token in activeTokens)
            token.Revoke();

        await unitOfWork.SaveChangesAsync(ct);
    }

    private static void EnsureSelf(Guid targetId, Guid currentUserId)
    {
        if (targetId != currentUserId)
            throw new ForbiddenException("You can only modify your own account.");
    }
}