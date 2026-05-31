using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Infrastructure.Data.EntityBuilder;

public static class UserEntityBuilder
{
    public static void BuildUsertModels(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.RegisteredAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}