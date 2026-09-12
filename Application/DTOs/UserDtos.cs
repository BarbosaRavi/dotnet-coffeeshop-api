using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace CoffeeShopApi.Application.DTOs;

public record CreateUserRequest(
    [Required, StringLength(50, MinimumLength = 2)] string Name,
    [Required, StringLength(250), EmailAddress] string Email,
    [Required, StringLength(128, MinimumLength = 8)]string Password

)
{
    [Required, Compare(nameof(Password))]
    public string PasswordConfirmation {get; init;} = "";
}

public record UpdateUserRequest(
    [Required, StringLength(50, MinimumLength = 2)] string Name,
    [Required, StringLength(250), EmailAddress] string Email
);

public record ChangePasswordRequest(
    [Required, StringLength(128, MinimumLength = 8)] string CurrentPassword,
    [Required, StringLength(128, MinimumLength = 8)] string NewPassword
)
{
    [Required, Compare(nameof(NewPassword))]
    public string NewPasswordConfirmation {get; init;} = "";
}

public record UserResponse(
    Guid Id,
    string Name,
    string Email
);