using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiTechStore.Infrastructure.Data.EntityBuilder;

public class PermissionEntityBuilder : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
    }
}