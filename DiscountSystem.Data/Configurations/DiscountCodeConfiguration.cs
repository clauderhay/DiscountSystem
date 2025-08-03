using DiscountSystem.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscountSystem.Data.Configurations;

public class DiscountCodeConfiguration : IEntityTypeConfiguration<DiscountCodeEntity>
{
    public void Configure(EntityTypeBuilder<DiscountCodeEntity> builder)
    {
        builder.ToTable("DiscountCodes");

        builder.HasKey(dc => dc.Id);

        builder.Property(dc => dc.Code)
            .IsRequired()
            .HasMaxLength(8)
            .IsFixedLength()
            .UseCollation("C"); // Case-sensitive for PostgreSQL

        // Unique index on code
        builder.HasIndex(dc => dc.Code)
            .IsUnique()
            .HasDatabaseName("UX_DiscountCodes_Code");

        // Partial index - only index unused codes (PostgreSQL feature)
        builder.HasIndex(dc => new { dc.Code, dc.IsUsed })
            .HasFilter("\"IsUsed\" = false")
            .HasDatabaseName("IX_DiscountCodes_UnusedOnly");

        builder.Property(dc => dc.CreatedAt)
            .IsRequired();
    }
}