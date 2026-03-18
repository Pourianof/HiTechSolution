namespace HiTechStore.Core.BackgroundJobs;

public class FailedOrdersRollbackHandler(IServiceScopeFactory serviceScopeFactory, ILogger<FailedOrdersRollbackHandler> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        var timer = new PeriodicTimer(TimeSpan.FromMinutes(15));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            logger.LogInformation("[+] Start rollback");
            await RollbackOrders();
            logger.LogInformation("[-] Rollback finished");
        }
    }


    private async Task RollbackOrders()
    {
        using var scope = serviceScopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        if (unitOfWork is null)
        {
            logger.LogWarning("Cannot access to {RequiredService}", nameof(IUnitOfWork));
            return;
        }

        var orderBeforeDateTime = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(15));

        var failedOrders = await unitOfWork.OrderRepository.GetPendingOrders(orderBeforeDateTime);

        if (failedOrders is null || !failedOrders.Any())
        {
            logger.LogInformation("No pending order existed");
            return;
        }

        logger.LogInformation("{OrdersCount} number of order for rolling back", failedOrders.Count());

        // release the locked amount of ordered items
        foreach (var item in failedOrders.SelectMany(order => order.Items!))
        {
            item.ProductVariation!.Inventory += item.Count;
        }

        // change orders state to cancelled
        foreach (var order in failedOrders)
        {
            order.PaymentState = Models.OrderPaymentState.Cancelled;
        }

        await unitOfWork.Complete();
    }
}
