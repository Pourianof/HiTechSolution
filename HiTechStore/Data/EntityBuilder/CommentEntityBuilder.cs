using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.EntityBuilder;

public static class CommentEntityBuilder
{
    public static void BuildCommentEntity(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(
            entity =>
            {
                entity
                    .HasOne(c => c.Product)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(c => c.ProductId);


                entity
                    .HasOne(c => c.User)
                    .WithMany()
                    .HasForeignKey(c => c.UserId)
                    .IsRequired();

                entity.HasIndex(p => p.RateId).IsUnique(false);

                entity
                    .HasOne(c => c.Rate)
                    .WithOne()
                    .IsRequired(false)
                    .HasForeignKey<Comment>(c => c.RateId);

                entity.Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            }
        );
    }
}