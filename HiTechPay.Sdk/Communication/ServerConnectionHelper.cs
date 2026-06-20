using System.Web;

namespace HiTechPay.Sdk.Communication;

public interface IServerConnectionHelper
{
    Uri GetPaymentUrl(string key, string callbackUrl);
}

internal class ServerConnectionHelper(PaySdkOptions connectionContext) : IServerConnectionHelper
{
    public Uri GetPaymentUrl(string key, string callbackUrl)
    {
        var url = new UriBuilder(connectionContext.GetPaymentServerAddressOrThrow())
        {
            Path = "/payment"
        };

        var query = HttpUtility.ParseQueryString(url.Query);
        query[ConnectionQueryStrings.Key] = key;
        query[ConnectionQueryStrings.CallbackUrl] = callbackUrl;

        url.Query = query.ToString();

        return url.Uri;
    }
}