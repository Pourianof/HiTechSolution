namespace HiTechStore.Infrastructure.Helpers;

public class OutboxSignal
{
    private readonly SemaphoreSlim _signal = new(0, 1);

    public void Notify()
    {
        if (_signal.CurrentCount == 0)
        {
            _signal.Release();
        }
    }

    public Task WaitAsync(CancellationToken ct)
    {
        return _signal.WaitAsync(ct);
    }
}