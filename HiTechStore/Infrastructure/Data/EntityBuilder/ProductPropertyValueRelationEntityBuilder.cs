using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Infrastructure.Data.EntityBuilder
{
    public class ProductPropertyValueRelationEntityBuilder
    {
        public static void Build(ModelBuilder modelBuilder)
        {
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
    }
}