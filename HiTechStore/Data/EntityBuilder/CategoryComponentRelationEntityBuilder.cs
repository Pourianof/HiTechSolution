using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.EntityBuilder
{
    public class CategoryComponentRelationEntityBuilder
    {
        public static void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CategoryComponent>(
                entity =>
                {
                    entity.HasKey(categoryComponent => new { categoryComponent.ComponentTypeId, categoryComponent.CategoryId });
                }
            );
        }
    }
}