namespace HiTechPay.Sdk;

public class PaySdkOptions
{
    public string? PaymentServerAddress { private get; init; }

    public string GetPaymentServerAddressOrThrow()
    {
        if (PaymentServerAddress is null)
        {
            throw new InvalidOperationException("No url provided for addressing payment server");
        }

        return PaymentServerAddress;
    }

    public string? KeyStorageDirectory { get; set; }
}