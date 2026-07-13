
using HiTechStore.Infrastructure.Data;
using HiTechStore.IntegrationTests.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Testcontainers.PostgreSql;

namespace HiTechStore.IntegrationTests.Fixtures;

public sealed class IntegrationFixture : IAsyncLifetime
{
    private PostgreSqlContainer _container = default!;

    public HiTechStoreWebApplicationFactory Factory { get; private set; } = default!;

    public HttpClient Client { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        _container = new PostgreSqlBuilder("postgres:17")
            .Build();

        await _container.StartAsync();

        Factory = new HiTechStoreWebApplicationFactory(
            _container.GetConnectionString());

        Client = Factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        Client.Dispose();
        Factory.Dispose();
        await _container.DisposeAsync();
    }
}