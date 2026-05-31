using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Infrastructure.Data.EntityBuilder
{
    public class BrandEntityBuilder
    {
        public static void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Brand>(
                (entity) =>
                {
                    entity
                        .HasMany((b) => b.Models)
                        .WithOne(bm => bm.Brand)
                        .OnDelete(DeleteBehavior.Cascade);
                }
            );
        }
    }
}