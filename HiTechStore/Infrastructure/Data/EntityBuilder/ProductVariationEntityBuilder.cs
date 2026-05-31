using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Infrastructure.Data.EntityBuilder
{
    public class ProductVariationEntityBuilder
    {
        public static void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductVariation>(entity =>
            {
                {
                    entity.ToTable("ProductVariations");
                    entity.Property(e => e.Price).IsRequired().HasColumnType("decimal(18,2)");
                    entity.HasOne(pv => pv.Product)
                        .WithMany(p => p.Variations)
                        .OnDelete(DeleteBehavior.Cascade);
                }
            });
        }
    }
}