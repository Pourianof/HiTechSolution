using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiTechStore.Infrastructure.Data.EntityBuilder
{
    public class CategoryComponentRelationEntityBuilder : IEntityTypeConfiguration<CategoryComponent>
    {
        public void Configure(EntityTypeBuilder<CategoryComponent> builder)
        {
            builder.HasKey(categoryComponent => new { categoryComponent.ComponentTypeId, categoryComponent.CategoryId });
        }
    }
}