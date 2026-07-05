using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiTechStore.Infrastructure.Data.EntityBuilder;

public class CommentEntityBuilder : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder
            .HasOne(c => c.Product)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.ProductId);


        builder
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .IsRequired();

        builder.HasIndex(p => p.RateId).IsUnique(false);

        builder
            .HasOne(c => c.Rate)
            .WithOne()
            .IsRequired(false)
            .HasForeignKey<Comment>(c => c.RateId);

        builder.Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}