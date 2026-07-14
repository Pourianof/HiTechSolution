using HiTechStore.Core.Models;

using Microsoft.AspNetCore.SignalR.Client;

namespace HiTechStore.IntegrationTests.Helpers;

public sealed class HubTestConnection : IAsyncDisposable
{
    public HubConnection Connection { get; }

    public TaskCompletionSource<UserNotification> Received { get; }

    public HubTestConnection(HubConnection connection)
    {
        Connection = connection;

        Received = new TaskCompletionSource<UserNotification>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        Connection.On<UserNotification>(
            nameof(UserNotification),
            notification =>
            {
                Received.TrySetResult(notification);
            });
    }

    public Task StartAsync()
        => Connection.StartAsync();

    public async ValueTask DisposeAsync()
    {
        await Connection.DisposeAsync();
    }
}