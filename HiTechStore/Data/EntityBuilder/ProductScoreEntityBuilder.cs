using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.EntityBuilder
{
    public class ProductScoreEntityBuilder
    {
        public static void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductScore>(entity =>
                {
                    entity.HasKey(ps => ps.ProductScoreId);

                    entity.HasOne<Product>()           // بدون navigation در dependent
                     .WithMany(p => p.Scores)    // اگر Product.Scores وجود دارد، مشخصش کن؛ در غیر این صورت .WithMany()
                     .HasForeignKey(ps => ps.ProductId);

                    entity.HasOne<User>()
                     .WithMany()
                     .HasForeignKey(ps => ps.UserId);

                    entity.HasIndex(ps => new { ps.ProductId, ps.UserId }).IsUnique();
                });
        }
    }
}