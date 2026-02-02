using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.EntityBuilder
{
    public class ProductEntityBuilder
    {
        public static void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                {
                    entity.ToTable("Products");
                    entity.HasKey(e => e.ProductId);
                    entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
                    entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
                    entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP"); // For Postgress
                    entity.HasQueryFilter((p) => p.IsDeleled == null || !p.IsDeleled!.Value);

                    entity.HasMany((p) => p.ComponentModels)
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
            });
        }
    }
}