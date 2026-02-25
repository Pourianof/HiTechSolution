using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.EntityBuilder;

public class DiscountCodeEntityBuilder
{
    public static void Build(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DiscountCode>(
             (entity) =>
             {
                 entity.ToTable("DiscountCodes");
                 entity.Property(dc => dc.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                 entity.Property(dc => dc.IsDeactivated).HasDefaultValue(false);
                 entity.HasOne(dc => dc.Creator).WithMany().HasForeignKey(dc => dc.CreatorId);
             }
         );
    }
}
