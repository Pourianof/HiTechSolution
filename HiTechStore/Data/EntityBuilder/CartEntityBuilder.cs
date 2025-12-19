
using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.EntityBuilder
{
    public class CartEntityBuilder
    {
        public static void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cart>(
                 (entity) =>
                 {
                     entity.HasMany(c => c.Items)
                        .WithOne(c => c.Cart)
                        .OnDelete(DeleteBehavior.Cascade);
                 }
             );
        }
    }
}