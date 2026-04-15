using HiTechStore.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.EntityBuilder;

public static class UserEntityBuilder
{
    public static void BuildUsertModels(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.RegisteredAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity
                .HasMany(u => u.Roles)
                .WithOne()
                .HasForeignKey(ur => ur.Id)
                .IsRequired();

            modelBuilder.Entity<IdentityRole<string>>()
                .HasMany<IdentityUserRole<string>>()
                .WithOne()
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
        });
    }
}