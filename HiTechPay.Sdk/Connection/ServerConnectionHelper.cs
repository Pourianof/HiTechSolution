using System.Web;

namespace HiTechPay.Sdk.Connection;

public interface IServerConnectionHelper
{
    Uri GetPaymentUrl(string key, string callbackUrl);
}

internal class ServerConnectionHelper : IServerConnectionHelper
{
    public Uri GetPaymentUrl(string key, string callbackUrl)
    {
        var url = new UriBuilder("http://localhost:5035");

        var query = HttpUtility.ParseQueryString(url.Query);
        query[ConnectionQueryStrings.Key] = key;
        query[ConnectionQueryStrings.CallbackUrl] = callbackUrl;

        url.Query = query.ToString();

        return url.Uri;
    }
}