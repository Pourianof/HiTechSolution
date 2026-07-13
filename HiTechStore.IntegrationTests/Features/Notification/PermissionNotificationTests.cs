using System.Net.Http.Headers;
using System.Net.Http.Json;

using FluentAssertions;

using HiTechStore.Core;
using HiTechStore.Core.Common.Events;
using HiTechStore.Core.Dto.Permission;
using HiTechStore.Core.Models;
using HiTechStore.Infrastructure.Data;
using HiTechStore.Infrastructure.Data.Repositories; // OutboxMessageRepository
using HiTechStore.IntegrationTests.Fixtures;
using HiTechStore.IntegrationTests.Infrastructure;
using HiTechStore.IntegrationTests.TestData;
using HiTechStore.Presentation.RealTime;
using HiTechStore.Presentation.Requests.Auth;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

using Xunit;

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
    public async Task ModifyPermissions_GrantsPermission_And_NotifiesTargetUser_ViaSignalR()
    {
        // Arrange
        var admin = await _context.Factory.UseServiceAsync<IUnitOfWork, User>(
            async uow =>
            {
                return (await uow.UserRepository.GetUserByEmailAsync(TestUsers.Admin.Email))!;
            }
        );

        var actorToken = TestJwtTokenGenerator.GenerateTestJwtToken(admin.Id);
        var targetToken = TestJwtTokenGenerator.GenerateTestJwtToken(TargetUserId);

        var hubUrl = new Uri(_context.Factory.Server.BaseAddress, NotificationHub.Route);

        // Connect to the hub AS THE TARGET USER, before triggering the
        // change, so we can prove the notification actually reaches the
        // correct user (not just "someone").
        await using var targetConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                options.HttpMessageHandlerFactory = _ => _context.Factory.Server.CreateHandler();
                options.AccessTokenProvider = () => Task.FromResult<string?>(targetToken);
            })
            .Build();

        var received = new TaskCompletionSource<PermissionChangedEvent>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        // "PermissionChanged" is the exact method name used in
        // PermissionChangeDispatcher.DispatchAsync, so this part is not a guess.
        targetConnection.On<PermissionChangedEvent>("PermissionChanged", evt =>
        {
            received.TrySetResult(evt);
        });

        await targetConnection.StartAsync();

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
        var evt = await received.Task.WaitAsync(
            TimeSpan.FromSeconds(5)
        );

        evt.Should().BeOfType<PermissionChangedEvent>();
        evt.TargetUserId.Should().Be(TargetUserId);
    }

    private async Task SeedAsync()
    {
        using var scope = _context.Factory.Services.CreateScope();

        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

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
        await SeedAsync();
    }

    public async Task DisposeAsync()
    {
    }
}