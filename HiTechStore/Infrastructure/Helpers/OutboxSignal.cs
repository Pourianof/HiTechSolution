namespace HiTechStore.Infrastructure.Helpers;

public class OutboxSignal
{
    private readonly SemaphoreSlim _signal = new(0);

    public void Notify()
    {
        _signal.Release();
    }

    public Task WaitAsync(CancellationToken ct)
    {
        return _signal.WaitAsync(ct);
    }
}