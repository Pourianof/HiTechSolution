
using HiTechStore.Data.EntityBuilder;
using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data
{
    public class HiTechStoreDbContext : DbContext
    {
        public HiTechStoreDbContext(DbContextOptions<HiTechStoreDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ProductEntityBuilder.Build(modelBuilder);
        }

        public DbSet<Product> Products { get; set; }
    }

}