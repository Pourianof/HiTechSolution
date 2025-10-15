using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;
public class ComponentTypeEntityBuilder
{
    public static void Build(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ComponentType>(
            entity =>
            {
                entity.HasMany(c => c.Properties)
                    .WithOne()
                    .OnDelete(DeleteBehavior.Cascade);

            }
        );
    }
}