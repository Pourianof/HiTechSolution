using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiTechStore.Infrastructure.Data.EntityBuilder;

public class DiscountEntityPropertyEntityBuilder : IEntityTypeConfiguration<DiscountEntityProperty>
{
    public void Configure(EntityTypeBuilder<DiscountEntityProperty> builder)
    {
        builder.ToTable("DiscountEntityProperties");
        builder.HasIndex(p => new { p.EntityId, p.Name }).IsUnique();
        builder.HasOne(dep => dep.SubEntity)
            .WithMany()
            .HasForeignKey(dep => dep.SubEntityId)
            .IsRequired(false);
    }
}