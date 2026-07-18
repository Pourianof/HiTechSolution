using System.Net.Http.Headers;
using System.Net.Http.Json;

using FluentAssertions;

using HiTechStore.Core;
using HiTechStore.Core.Models;
using HiTechStore.Infrastructure.Data;
using HiTechStore.Infrastructure.Data.Seeders;
using HiTechStore.IntegrationTests.Fixtures;
using HiTechStore.IntegrationTests.Helpers;
using HiTechStore.IntegrationTests.Infrastructure;
using HiTechStore.IntegrationTests.TestData;
using HiTechStore.Presentation.RealTime;
using HiTechStore.Presentation.Requests.Permission;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;



namespace HiTechStore.IntegrationTests;

public class PermissionChangeSignalRIntegrationTests :
    IClassFixture<IntegrationFixture>,
    IAsyncLifetime
{
    private readonly IntegrationFixture _context;

    // TODO: replace with real, seeded test user ids (created in SeedAsync).
    private const string TargetUserId = "22222222-2222-2222-2222-222222222222";

    // TODO: replace with a real permission code from your `Permissions`
    // static class (not in the files I was given, so I couldn't reference
    // it directly, e.g. Permissions.Reports.View).
    private const string PermissionCodeUnderTest = Permissions.Product.Create;

    public PermissionChangeSignalRIntegrationTests(
        IntegrationFixture context
    )
    {
        _context = context;
    }


    [Fact]
    public async Task ModifyPermissions_ShouldDispatchNotificationViaSignalRChannel()
    {
        // Arrange
        var admin = await _context.Factory.UseServiceAsync<IUnitOfWork, User>(
            async uow =>
            {
                return (await uow.UserRepository.GetUserByEmailAsync(TestUsers.Admin.Email))!;
            }
        );

        var actorToken = TestJwtTokenGenerator.GenerateTestJwtToken(admin.Id);

        await using var hub = await ConnectToHub();

        _context.Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", actorToken);

        var requestBody = new UpdatePermissionsRequest
        {
            Permissions = [
                new PermissionChangeRequest
                {
                    PermissionCode = PermissionCodeUnderTest,
                    Action = "grant",
                    Scope = "all",
                }
            ]
        };

        var response = await _context.Client.PatchAsJsonAsync($"/api/auth/{TargetUserId}/permissions", requestBody);

        response.EnsureSuccessStatusCode();

        // Assert
        var notif = await hub.Received.Task.WaitAsync(
            TimeSpan.FromSeconds(1000)
        );

        notif.Should().BeOfType<UserNotification>();
        notif.Type.Should().Be("PermissionChanged");
        notif.OwnerId.Should().Be(TargetUserId);
    }

    [Fact]
    public async Task ModifyPermissions_ShouldSaveNotificationAndDispatchItWhenUserConnectViaSignalR()
    {
        // Arrange
        var admin = await _context.Factory.UseServiceAsync<IUnitOfWork, User>(
            async uow =>
            {
                return (await uow.UserRepository.GetUserByEmailAsync(TestUsers.Admin.Email))!;
            }
        );

        var actorToken = TestJwtTokenGenerator.GenerateTestJwtToken(admin.Id);


        _context.Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", actorToken);

        var requestBody = new UpdatePermissionsRequest
        {
            Permissions = [
                new PermissionChangeRequest
                {
                    PermissionCode = PermissionCodeUnderTest,
                    Action = "grant",
                    Scope = "all",
                }
            ]
        };

        var response = await _context.Client.PatchAsJsonAsync($"/api/auth/{TargetUserId}/permissions", requestBody);
        response.EnsureSuccessStatusCode();

        // No connection yet, so notification must be saved
        using (var scope = _context.Factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<HiTechStoreDbContext>();

            var notification = dbContext.Set<UserNotification>().FirstOrDefault();

            notification.Should().NotBeNull();
            notification.IsRead.Should().BeFalse();
            notification.Type.Should().Be("PermissionChanged");
        }

        await using var hub = await ConnectToHub();

        // Assert
        var notif = await hub.Received.Task.WaitAsync(
            TimeSpan.FromSeconds(1000)
        );

        notif.Should().BeOfType<UserNotification>();
        notif.Type.Should().Be("PermissionChanged");
        notif.OwnerId.Should().Be(TargetUserId);
    }

    private async Task<HubTestConnection> ConnectToHub()
    {
        var token = TestJwtTokenGenerator.GenerateTestJwtToken(TargetUserId);

        var hubUrl = new Uri(
            _context.Factory.Server.BaseAddress,
            NotificationHub.Route);

        var connection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                options.HttpMessageHandlerFactory =
                    _ => _context.Factory.Server.CreateHandler();

                options.AccessTokenProvider =
                    () => Task.FromResult<string?>(token);
            })
            .Build();

        var hub = new HubTestConnection(connection);

        await hub.StartAsync();

        return hub;
    }

    private async Task SeedAsync()
    {
        using var scope = _context.Factory.Services.CreateScope();

        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var targetUser = await unitOfWork.UserRepository.GetUserByIdAsync(TargetUserId);

        await unitOfWork.UserRepository.RegisterUser(
            new()
            {
                Id = TargetUserId,
                Email = TestUsers.NormalUser.Email,
                UserName = TestUsers.NormalUser.Username
            },
            TestUsers.NormalUser.Password
        );

        await unitOfWork.Complete();

        await Task.CompletedTask;
    }

    public async Task InitializeAsync()
    {
        using (var scope = _context.Factory.Services.CreateScope())
        {

            var dbContext = scope.ServiceProvider.GetRequiredService<HiTechStoreDbContext>();

            await dbContext.Database.ExecuteSqlRawAsync("DROP SCHEMA public CASCADE;");

            await dbContext.Database.ExecuteSqlRawAsync("CREATE SCHEMA public;");

            await dbContext.Database.EnsureCreatedAsync();

            await SeederExtension.SeedRequiredBaseData(scope.ServiceProvider);
        }

        await SeedAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;
}