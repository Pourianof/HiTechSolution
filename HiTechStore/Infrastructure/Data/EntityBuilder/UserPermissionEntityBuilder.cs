using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiTechStore.Infrastructure.Data.EntityBuilder;

public class UserPermissionEntityBuilder : IEntityTypeConfiguration<UserPermission>
{
    public void Configure(EntityTypeBuilder<UserPermission> builder)
    {
        builder.Property(u => u.GrantedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}