using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiTechStore.Infrastructure.Data.EntityBuilder;

public class DiscountEntityEntityBuilder : IEntityTypeConfiguration<DiscountEntity>
{
    public void Configure(EntityTypeBuilder<DiscountEntity> builder)
    {
        builder.ToTable("DiscountEntities");
        builder.HasIndex(p => p.Name).IsUnique();
        builder.HasMany(de => de.Properties)
           .WithOne(dep => dep.Entity)
           .HasForeignKey(dep => dep.EntityId);
    }
}
