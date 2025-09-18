
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

            modelBuilder.Entity<Product>(
                (entity) =>
                {
                    entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP"); // For Postgress
                    entity.HasQueryFilter((p) => p.IsDeleled == null || !p.IsDeleled!.Value);
                }
            );

            modelBuilder.Entity<Category>(
                (entity) =>
                {
                    entity.HasMany(c => c.CategoryProperties)
                        .WithOne((cp) => cp.Category);
                }
            );

            modelBuilder.Entity<ProductPropertyValue>(
                entity =>
                {
                    entity.HasKey(p => new { p.ProductId, p.PropertyId });
                    entity.HasOne(ppv => ppv.Product)
                        .WithMany(p => p.Properties);
                    entity.HasOne(ppv => ppv.Property);
                }
            );

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductScore> ProductScores { get; set; }
    }

}