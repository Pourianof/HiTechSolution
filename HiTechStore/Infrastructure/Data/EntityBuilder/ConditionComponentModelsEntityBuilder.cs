using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Infrastructure.Data.EntityBuilder;

public static class ConditionComponentModelsEntityBuilder
{
    public static void BuildConditionComponentModels(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConditionComponent>(entity =>
        {
            entity.HasKey(cc => cc.ConditionComponentId);

            entity.HasMany(cc => cc.SubConditions)
                .WithOne(cc => cc.Parent)
                .HasForeignKey(cc => cc.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(cc => cc.Lambda)
                .WithOne(l => l.OwnerCondition)
                .HasForeignKey<ConditionLambda>(l => l.OwnerConditionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ConditionLambda>(entity =>
        {
            entity.HasKey(l => l.ConditionLambdaId);

            // 🔥 این قسمت مهمه
            entity.HasOne(l => l.Body)
                .WithMany() // هیچ navigation برعکس نداره
                .HasForeignKey(l => l.BodyId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔥 اینم صریح تعریف کن (حتی اگر بالا هست)
            entity.HasOne(l => l.OwnerCondition)
                .WithOne(c => c.Lambda)
                .HasForeignKey<ConditionLambda>(l => l.OwnerConditionId);
        });
    }
}