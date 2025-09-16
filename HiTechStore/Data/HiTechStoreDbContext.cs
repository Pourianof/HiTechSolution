
using HiTechStore.Data.EntityBuilder;
using HiTechStore.Models;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data
{
    public class HiTechStoreDbContext : IdentityDbContext<User>
    {
        public HiTechStoreDbContext(DbContextOptions<HiTechStoreDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ProductEntityBuilder.Build(modelBuilder);
            modelBuilder.Entity<ProductCategory>().ToTable("ProductCategories").HasKey(pc => new { pc.ProductId, pc.CategoryId });

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

            modelBuilder.Entity<Product>()
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP"); // For Postgress

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductScore> ProductScores { get; set; }
    }

}