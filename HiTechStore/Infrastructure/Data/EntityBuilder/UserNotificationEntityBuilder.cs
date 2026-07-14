using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiTechStore.Infrastructure.Data.EntityBuilder;

public class UserNotificationEntityBuilder : IEntityTypeConfiguration<UserNotification>
{
    public void Configure(EntityTypeBuilder<UserNotification> builder)
    {
        builder.Ignore(un => un.IsRead);
        builder.Property(un => un.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}