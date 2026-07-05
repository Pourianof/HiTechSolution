using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiTechStore.Infrastructure.Data.EntityBuilder;

public class ProductPropertyValueRelationEntityBuilder : IEntityTypeConfiguration<ProductPropertyValue>
{
    public void Configure(EntityTypeBuilder<ProductPropertyValue> builder)
    {
        builder.HasKey(p => new { p.ProductId, p.PropertyId });
        builder.HasOne(ppv => ppv.Product)
            .WithMany(p => p.Properties);
        builder.HasOne(ppv => ppv.Property);
    }
}