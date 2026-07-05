using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiTechStore.Infrastructure.Data.EntityBuilder;

public class PermissionAuditEntityBuilder : IEntityTypeConfiguration<PermissionAudit>
{
    public void Configure(EntityTypeBuilder<PermissionAudit> builder)
    {
        builder.Property(u => u.OccurredAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}