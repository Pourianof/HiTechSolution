using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.EntityBuilder
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