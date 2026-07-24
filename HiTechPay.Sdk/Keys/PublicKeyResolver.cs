using System.Security.Cryptography;

namespace HiTechPay.Sdk.Keys;

internal class PublicKeyResolver
{
    private const string _pubKeyPath = "keys/public_key.pem";

    public async static Task<RSA> Resolve(string serverAddress, string? keyStorageDirectory = _pubKeyPath)
    {
        if (File.Exists(keyStorageDirectory))
        {
            var rsa = RSA.Create();
            var pubKey = File.ReadAllText(keyStorageDirectory);
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

            var keyDirectory = Path.GetDirectoryName(keyStorageDirectory);
            if (!string.IsNullOrWhiteSpace(keyDirectory) && !Directory.Exists(keyDirectory))
            {
                Directory.CreateDirectory(keyDirectory);
            }

            await File.WriteAllTextAsync(keyStorageDirectory!, pubKey);

            var rsa = RSA.Create();
            rsa.ImportFromPem(pubKey);

            return rsa;
        }
    }
}