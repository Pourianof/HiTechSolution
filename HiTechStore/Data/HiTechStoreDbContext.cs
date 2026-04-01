
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
            ProductScoreEntityBuilder.Build(modelBuilder);
            CategoryEntityBuilder.Build(modelBuilder);
            ComponentTypeEntityBuilder.Build(modelBuilder);
            ComponentModelEntityBuilder.Build(modelBuilder);
            ProductPropertyValueRelationEntityBuilder.Build(modelBuilder);
            BrandEntityBuilder.Build(modelBuilder);
            BrandModelEntityBuilder.Build(modelBuilder);
            CategoryComponentRelationEntityBuilder.Build(modelBuilder);
            CartEntityBuilder.Build(modelBuilder);
            OrderEntityBuilder.Build(modelBuilder);
            DiscountCodeEntityBuilder.Build(modelBuilder);
            DiscountEntityEntityBuilder.Build(modelBuilder);
            DiscountRuleEntityBuilder.Build(modelBuilder);
            modelBuilder.BuildConditionComponentModels();
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductScore> ProductScores { get; set; }
        public DbSet<ComponentType> ComponentType { get; set; }
        public DbSet<Brand> Brand { get; set; }
        public DbSet<BrandModel> BrandModel { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<DiscountCode> DiscountCodes { get; set; }
        public DbSet<DiscountEntity> DiscountEntities { get; set; }
        public DbSet<ConditionComponent> ConditionComponents { get; set; }
    }

}