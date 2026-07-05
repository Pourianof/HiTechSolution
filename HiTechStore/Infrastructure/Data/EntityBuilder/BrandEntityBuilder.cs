using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiTechStore.Infrastructure.Data.EntityBuilder;

public class BrandEntityBuilder : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder
            .HasMany((b) => b.Models)
            .WithOne(bm => bm.Brand)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
