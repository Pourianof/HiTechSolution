using System.Threading.Tasks;

using HiTechStore.Infrastructure.Data.Seeders;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task<WebApplication> DbInitialize(this WebApplication app)
    {
        if (app.Environment.IsProduction())
        {
            using (var scope = app.Services.CreateScope())
            {
                using var db = scope.ServiceProvider.GetRequiredService<HiTechStoreDbContext>();

                await db.Database.MigrateAsync();
            }
        }

        await app.SeedDatabase();

        return app;
    }
}