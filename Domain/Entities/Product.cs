using CoffeeShopApi.Domain.Exceptions;

namespace CoffeeShopApi.Domain.Entities;

public class Product
{
    public Guid Id {get; private set;}
    public string Name {get; private set;} = string.Empty;
    public string Description {get; private set;} = string.Empty;
    public decimal Price {get; private set;}
    public DateTime CreatedAt {get; private set;}
    public DateTime? UpdatedAt {get; private set;}
    public DateTime? DeletedAt {get; private set;}
    public bool IsDeleted => DeletedAt is not null;
    private Product() {}

    public static Product Create(string name, string description, decimal price)
    {
        Validate(name, description, price);

        return new Product
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Description = description.Trim(),
            Price = price,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string description, decimal price)
    {
        if (IsDeleted) throw new BusinessRuleException("Cannot update a deleted product.");

        Validate(name, description, price);

            Name = name.Trim();
            Description = description.Trim();
            Price = price;
            UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        if (IsDeleted) throw new BusinessRuleException("Product is already deleted.");

        DeletedAt = DateTime.UtcNow;
    }

    public void Restore()
    {
        if (!IsDeleted) throw new BusinessRuleException("Product is not deleted.");

        DeletedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void Validate(string name, string description, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new BusinessRuleException("Name cannot be empty.");
        if (string.IsNullOrWhiteSpace(description)) throw new BusinessRuleException("Description cannot be empty.");
        if (price <= 0) throw new BusinessRuleException("Price must be greater than zero.");
    }
}