using System.Security.Cryptography;

namespace HiTechPay.Sdk.Keys;

internal class PublicKeyResolver
{
    private static string _pubKeyPath = "public_key.pem";

    public async static Task<RSA> Resolve(string serverAddress)
    {
        if (File.Exists(_pubKeyPath))
        {
            var rsa = RSA.Create();
            var pubKey = File.ReadAllText(_pubKeyPath);
            rsa.ImportFromPem(pubKey);
            return rsa;
        }
        else
        {
            var client = new HttpClient()
            {
                BaseAddress = new Uri(serverAddress)
            };

            var response = await client.GetAsync("/api/keys/pub-key");
            var pubKey = await response.Content.ReadAsStringAsync();

            await File.WriteAllTextAsync(_pubKeyPath, pubKey);

            var rsa = RSA.Create();
            rsa.ImportFromPem(pubKey);

            return rsa;
        }
    }
}