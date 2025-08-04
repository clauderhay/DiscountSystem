using DiscountSystem.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiscountSystem.Data.Context;

public class DiscountDbContext(DbContextOptions<DiscountDbContext> options) : DbContext(options)
{
    public DbSet<DiscountCodeEntity> DiscountCodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DiscountDbContext).Assembly);
    }
}