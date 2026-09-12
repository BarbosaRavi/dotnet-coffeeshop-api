using CoffeeShopApi.Domain.Exceptions;

namespace CoffeeShopApi.Domain.Entities;

public class User
{
    public Guid Id { get; private set;}
    public string Name { get; private set;} = string.Empty;
    public string Email { get; private set;} = string.Empty;
    public string Password { get; private set;} = string.Empty;
    public DateTime CreatedAt { get; private set;}
    public DateTime? UpdatedAt { get; private set;}

    private User() {}

    public static User Create(string name, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new BusinessRuleException("Name cannot be empty.");
        if (string.IsNullOrWhiteSpace(email)) throw new BusinessRuleException("Email cannot be empty.");
        if (string.IsNullOrWhiteSpace(password)) throw new BusinessRuleException("Password cannot be empty.");

        return new User()
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Email = NormalizeEmail(email),
            Password= password,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new BusinessRuleException("Name cannot be empty.");
        if (string.IsNullOrWhiteSpace(email)) throw new BusinessRuleException("Email cannot be empty.");

        Name = name.Trim();
        Email = NormalizeEmail(email);
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void ChangePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password)) throw new BusinessRuleException("Password cannot be empty.");

        Password = password;
        UpdatedAt = DateTime.UtcNow;
    }

    public static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}