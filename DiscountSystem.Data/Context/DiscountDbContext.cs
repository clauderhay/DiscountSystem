using DiscountSystem.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiscountSystem.Data.Context;

public class DiscountDbContext : DbContext
{
    public DiscountDbContext(DbContextOptions<DiscountDbContext> options)
        : base(options)
    {
    }

    public DbSet<DiscountCodeEntity> DiscountCodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DiscountDbContext).Assembly);
    }
}