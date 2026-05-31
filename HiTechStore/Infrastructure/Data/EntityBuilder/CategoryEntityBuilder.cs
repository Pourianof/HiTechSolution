using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Infrastructure.Data.EntityBuilder
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