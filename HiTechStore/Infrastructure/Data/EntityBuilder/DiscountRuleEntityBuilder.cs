
using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Infrastructure.Data.EntityBuilder;

public class DiscountRuleEntityBuilder
{
    public static void Build(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DiscountRule>(
             (entity) =>
             {
                 entity.ToTable("DiscountRules");
                 entity.OwnsOne(rule => rule.DiscountAction)
                    .WithOwner();
             }
         );
    }
}
