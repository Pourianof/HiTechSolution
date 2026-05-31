
using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Infrastructure.Data.EntityBuilder
{
    public class OrderEntityBuilder
    {
        public static void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(
                 (entity) =>
                 {
                     entity.HasMany(o => o.Items)
                        .WithOne(o => o.Order)
                        .OnDelete(DeleteBehavior.Cascade);
                 }
             );
        }
    }
}