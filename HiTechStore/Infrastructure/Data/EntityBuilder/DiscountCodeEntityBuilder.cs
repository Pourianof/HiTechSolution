using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiTechStore.Infrastructure.Data.EntityBuilder;

public class DiscountCodeEntityBuilder : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> builder)
    {
        builder.ToTable("Discounts");
        builder.Property(dc => dc.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(dc => dc.IsDeactivated).HasDefaultValue(false);
        builder.HasOne(dc => dc.Creator).WithMany().HasForeignKey(dc => dc.CreatorId);
    }
}
