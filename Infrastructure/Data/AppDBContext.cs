using CoffeeShopApi.Application.Interfaces;
using CoffeeShopApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopApi.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options): DbContext(options), IUnitOfWork
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}