using System.Security.Cryptography;

using HiTechPay.Infrastructure.Models;

using Microsoft.Extensions.Options;

namespace HiTechPay.Infrastructure;

public interface IRsaProvider
{
    RSA GetPrivateKey();
}

public class RsaProvider : IRsaProvider
{
    private RSA? _rsa { get; set; }
    private static RSA RegisterKeys(string path)
    {
        var rsa = RSA.Create(2048);
        File.WriteAllText(path, rsa.ExportRSAPrivateKeyPem());

        return rsa;
    }

    private SignatureOptions _opts { get; init; }
    public RsaProvider(IOptions<SignatureOptions> signOptions)
    {
        _opts = signOptions.Value;
    }

    public RSA GetPrivateKey()
    {
        if (_rsa is not null)
        {
            return _rsa;
        }


        var signatureOpts = _opts;

        if (signatureOpts is null)
        {
            throw new InvalidOperationException("signature configuration not exits");
        }
        else if (signatureOpts.PrivateKeyPath is null)
        {
            throw new InvalidOperationException("no file path specified for SignatureOptions.PrivateKeyPath configue");
        }

        if (File.Exists(signatureOpts.PrivateKeyPath))
        {
            var privatePem = File.ReadAllText(signatureOpts.PrivateKeyPath);
            _rsa = RSA.Create();
            _rsa.ImportFromPem(privatePem);
        }
        else
        {
            _rsa = RegisterKeys(signatureOpts.PrivateKeyPath);
        }

        return _rsa;
    }

    public Jwk CreateJwk(string keyId)
    {
        var parameters = _rsa!.ExportParameters(false);

        return new Jwk
        {
            kty = "RSA",
            n = Convert.ToBase64String(parameters.Modulus!),
            e = Convert.ToBase64String(parameters.Exponent!),
            kid = keyId
        };
    }
}
