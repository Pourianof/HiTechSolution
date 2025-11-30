using System.Security.Cryptography;
using System.Text;

using HiTechPay.Infrastructure;

namespace HiTechPay.Services;

public class SingerService(IRsaProvider rsaProvider) : ISignerService
{
    private IRsaProvider _rsaProvider { get; set; } = rsaProvider;
    public string Sign(string text)
    {
        var rsa = _rsaProvider.GetPrivateKey();

        if (rsa == null)
            throw new Exception("Private key is not loaded. Cannot sign.");

        byte[] bytes = Encoding.UTF8.GetBytes(text);

        byte[] signature = rsa.SignData(
            bytes,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1
        );

        return Convert.ToBase64String(signature);
    }
}


public static class SignerServiceProvider
{
    public static IServiceCollection UseSigner(this IServiceCollection services)
    {
        services.AddScoped<ISignerService, SingerService>();

        return services;
    }

}
