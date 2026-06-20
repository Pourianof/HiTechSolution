using System.Security.Cryptography;
using System.Text;

namespace HiTechPay.Sdk.Keys;

public interface IVerifier
{
    Task<bool> Verify(string mainKey, string signatureBase64);
}

internal class Verifier(PaySdkOptions options) : IVerifier
{
    public async Task<bool> Verify(string mainKey, string signatureBase64)
    {
        if (string.IsNullOrEmpty(signatureBase64))
        {
            throw new ArgumentNullException(nameof(signatureBase64));
        }

        if (string.IsNullOrEmpty(mainKey))
        {
            throw new ArgumentNullException(nameof(mainKey));
        }

        byte[] bytes = Encoding.UTF8.GetBytes(mainKey);
        byte[] signature = Convert.FromBase64String(signatureBase64);

        var pubKey = await PublicKeyResolver.Resolve(options.GetPaymentServerAddressOrThrow(), options.KeyStorageDirectory);

        return pubKey.VerifyData(
            bytes,
            signature,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1
        );


    }
}