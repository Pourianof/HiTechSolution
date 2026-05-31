using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Infrastructure.Data.EntityBuilder
{
    public class ComponentModelEntityBuilder
    {
        public static void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ComponentModel>(
                (entity) =>
                {
                    entity
                        .HasIndex(m => new { m.ComponentTypeId, m.BrandModelId })
                        .IsUnique();
                }
            );
        }
    }
}