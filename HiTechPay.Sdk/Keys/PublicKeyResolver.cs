using System.Security.Cryptography;

namespace HiTechPay.Sdk.Keys;

internal class PublicKeyResolver
{
    private const string _pubKeyPath = "keys";
    private const string _pubKeyFileName = "public_key.pem";

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

            if (!string.IsNullOrWhiteSpace(keyStorageDirectory) && !Directory.Exists(keyStorageDirectory))
            {
                Directory.CreateDirectory(keyStorageDirectory);
            }

            var fullPath = Path.Combine(keyStorageDirectory ?? "", _pubKeyFileName);
            await File.WriteAllTextAsync(fullPath, pubKey);

            var rsa = RSA.Create();
            rsa.ImportFromPem(pubKey);

            return rsa;
        }
    }
}