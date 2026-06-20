using System.Security.Cryptography;
using System.Text;

using HiTechPay.Sdk.Communication;

namespace HiTechPay.Sdk.Keys;

public interface IVerifier
{
    Task<bool> Verify(string mainKey, string signatureBase64);
}

internal class Verifier(ServerConnectionContext connectionContext) : IVerifier
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

        var pubKey = await PublicKeyResolver.Resolve(connectionContext.GetPaymentServerAddressOrThrow());

        return pubKey.VerifyData(
            bytes,
            signature,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1
        );


    }
}