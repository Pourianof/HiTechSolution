using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiTechStore.Infrastructure.Data.EntityBuilder;

public class ProductVariationEntityBuilder : IEntityTypeConfiguration<ProductVariation>
{
    public void Configure(EntityTypeBuilder<ProductVariation> builder)
    {
        builder.ToTable("ProductVariations");
        builder.Property(e => e.Price).IsRequired().HasColumnType("decimal(18,2)");
        builder.HasOne(pv => pv.Product)
            .WithMany(p => p.Variations)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
