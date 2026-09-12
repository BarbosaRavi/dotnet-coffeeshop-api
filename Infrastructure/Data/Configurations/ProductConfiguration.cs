using CoffeeShopApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoffeeShopApi.Infrastructure.Data.Configurations;

public class ProductConfiguration: IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(500).IsRequired();
        builder.Property(p => p.Price).HasPrecision(18, 2);
        builder.HasIndex(p => p.Name);
        builder.HasIndex(p => p.DeletedAt);
        builder.HasQueryFilter(p => p.DeletedAt == null);   
        builder.Ignore(p => p.IsDeleted);                   
    }
}