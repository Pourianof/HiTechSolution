using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Infrastructure.Data.EntityBuilder;

public static class PermissionAuditEntityBuilder
{
    public static void BuildPermissionAuditBuilderEntity(this ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<PermissionAudit>(entity =>
        {
            entity.Property(u => u.OccurredAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}