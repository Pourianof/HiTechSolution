using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiTechStore.Infrastructure.Data.EntityBuilder;

public class OutboxMessageEntityBuilder : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.Property(
            (om) => om.OccurredAt
        ).HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}