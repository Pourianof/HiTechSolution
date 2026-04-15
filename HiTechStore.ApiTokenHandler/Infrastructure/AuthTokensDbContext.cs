using HiTechStore.ApiTokenHandler.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.ApiTokenHandler.Infrastructure;

internal class AuthTokensDbContext : DbContext
{
    public AuthTokensDbContext(DbContextOptions<AuthTokensDbContext> options)
            : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("AuthTokens");

        modelBuilder.Entity<RefreshToken>(
            entity =>
            {
                entity.HasIndex(e => e.UserId);
            }
        );

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<RefreshToken> RefreshTokens { get; set; }
}
