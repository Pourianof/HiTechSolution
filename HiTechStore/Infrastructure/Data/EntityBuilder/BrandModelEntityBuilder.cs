using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Infrastructure.Data.EntityBuilder
{
    public class BrandModelEntityBuilder
    {
        public static void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BrandModel>(
                 (entity) =>
                 {
                     entity
                         .HasIndex(bm => new { bm.NormalizedName, bm.BrandId })
                         .IsUnique();
                 }
             );
        }
    }
}