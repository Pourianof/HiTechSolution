using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiTechStore.Infrastructure.Data.EntityBuilder
{
    public class ComponentModelEntityBuilder : IEntityTypeConfiguration<ComponentModel>
    {
        public void Configure(EntityTypeBuilder<ComponentModel> builder)
        {
            builder
                .HasIndex(m => new { m.ComponentTypeId, m.BrandModelId })
                .IsUnique();
        }
    }
}