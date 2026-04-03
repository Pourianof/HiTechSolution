using System;

using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.EntityBuilder;

public class DiscountEntityEntityBuilder
{
    public static void Build(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DiscountEntity>(
             (entity) =>
             {
                 entity.ToTable("DiscountEntities");
                 entity.HasIndex(p => p.Name).IsUnique();
                 entity.HasMany(de => de.Properties)
                    .WithOne(dep => dep.Entity)
                    .HasForeignKey(dep => dep.EntityId);
             }
         );

        modelBuilder.Entity<DiscountEntityProperty>(
            (entity) =>
            {
                entity.ToTable("DiscountEntityProperties");
                entity.HasIndex(p => new { p.EntityId, p.Name }).IsUnique();
                entity.HasOne(dep => dep.SubEntity)
                    .WithMany()
                    .HasForeignKey(dep => dep.SubEntityId)
                    .IsRequired(false);
            }
        );
    }
}
