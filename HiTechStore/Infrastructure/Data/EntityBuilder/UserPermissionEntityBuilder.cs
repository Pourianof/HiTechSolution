using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Infrastructure.Data.EntityBuilder;

public static class UserPermissionEntityBuilder
{
    public static void BuildUserPermissionEntity(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserPermission>(entity =>
        {
            entity.Property(u => u.GrantedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}