using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.EntityBuilder
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