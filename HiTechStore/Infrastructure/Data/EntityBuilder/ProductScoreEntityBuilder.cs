using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiTechStore.Infrastructure.Data.EntityBuilder;

public class ProductScoreEntityBuilder : IEntityTypeConfiguration<ProductScore>
{
    public void Configure(EntityTypeBuilder<ProductScore> builder)
    {
        builder.HasKey(ps => ps.ProductScoreId);

        builder.HasOne<Product>()
         .WithMany(p => p.Scores)
         .HasForeignKey(ps => ps.ProductId);

        builder.HasOne<User>()
         .WithMany()
         .HasForeignKey(ps => ps.UserId);

        builder.HasIndex(ps => new { ps.ProductId, ps.UserId }).IsUnique();
    }
}
