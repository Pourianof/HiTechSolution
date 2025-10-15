using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.EntityBuilder
{
    public class CategoryEntityBuilder
    {
        public static void Build(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Category>(
                (entity) =>
                {
                    entity.HasMany(c => c.Properties)
                        .WithOne()
                        .OnDelete(DeleteBehavior.Cascade);
                }
            );
        }
    }
}