using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiTechStore.Infrastructure.Data.EntityBuilder;

public class BrandModelEntityBuilder : IEntityTypeConfiguration<BrandModel>
{
    public void Configure(EntityTypeBuilder<BrandModel> builder)
    {
        builder
            .HasIndex(bm => new { bm.NormalizedName, bm.BrandId })
            .IsUnique();
    }
}
