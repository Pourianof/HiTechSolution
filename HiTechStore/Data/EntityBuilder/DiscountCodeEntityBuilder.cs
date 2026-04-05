using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.EntityBuilder;

public class DiscountCodeEntityBuilder
{
    public static void Build(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Discount>(
             (entity) =>
             {
                 entity.ToTable("Discounts");
                 entity.Property(dc => dc.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                 entity.Property(dc => dc.IsDeactivated).HasDefaultValue(false);
                 entity.HasOne(dc => dc.Creator).WithMany().HasForeignKey(dc => dc.CreatorId);
             }
         );
    }
}
