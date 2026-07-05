using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiTechStore.Infrastructure.Data.EntityBuilder;

public class ConditionComponentModelsEntityBuilder : IEntityTypeConfiguration<ConditionComponent>
{
    public void Configure(EntityTypeBuilder<ConditionComponent> builder)
    {
        builder.HasKey(cc => cc.ConditionComponentId);

        builder.HasMany(cc => cc.SubConditions)
            .WithOne(cc => cc.Parent)
            .HasForeignKey(cc => cc.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cc => cc.Lambda)
            .WithOne(l => l.OwnerCondition)
            .HasForeignKey<ConditionLambda>(l => l.OwnerConditionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}