using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiTechStore.Infrastructure.Data.EntityBuilder;

public class ConditionLambdaEntityBuilder : IEntityTypeConfiguration<ConditionLambda>
{
    public void Configure(EntityTypeBuilder<ConditionLambda> builder)
    {
        builder.HasKey(l => l.ConditionLambdaId);

        builder.HasOne(l => l.Body)
            .WithMany()
            .HasForeignKey(l => l.BodyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.OwnerCondition)
            .WithOne(c => c.Lambda)
            .HasForeignKey<ConditionLambda>(l => l.OwnerConditionId);
    }
}