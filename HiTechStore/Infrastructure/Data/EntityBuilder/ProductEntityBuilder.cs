using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiTechStore.Infrastructure.Data.EntityBuilder;

public class ProductEntityBuilder : IEntityTypeConfiguration<Product>
{

    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(e => e.ProductId);
        builder.Property(e => e.Title).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Description).IsRequired().HasMaxLength(500);
        builder.Property(e => e.CreatedAt)
          .HasDefaultValueSql("CURRENT_TIMESTAMP"); // For Postgress
        builder.HasQueryFilter((p) => p.IsDeleled == null || !p.IsDeleled!.Value);

        builder.HasMany((p) => p.ComponentModels)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "ProductComponents",
                entity => entity.HasOne<ComponentModel>().WithMany().HasForeignKey("ComponentModelId"),
                entity => entity.HasOne<Product>().WithMany().HasForeignKey("ProductId"),
                entity =>
                {
                    entity.HasKey("ProductId", "ComponentModelId");
                    entity.HasIndex("ProductId", "ComponentModelId").IsUnique();
                }
            );
    }
}