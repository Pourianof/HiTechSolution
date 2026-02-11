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
             }
         );
    }
}
