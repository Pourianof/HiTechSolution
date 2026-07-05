using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
public class ComponentTypeEntityBuilder : IEntityTypeConfiguration<ComponentType>
{
    public void Configure(EntityTypeBuilder<ComponentType> builder)
    {
        builder.HasMany(c => c.Properties)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }
}