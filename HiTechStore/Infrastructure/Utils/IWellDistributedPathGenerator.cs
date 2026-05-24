using System.Security.Cryptography;
using System.Text;

namespace HiTechStore.Infrastructure.Utils;

public interface IWellDistributedPathGenerator
{
    Task<string> Generate(string seed);
}

public class Sha256TwoPartDistributedPathGenerator : IWellDistributedPathGenerator
{
    public async Task<string> Generate(string seed)
    {
        using SHA256 sha256 = SHA256.Create();


        byte[] inputBytes = Encoding.UTF8.GetBytes(seed);

        using var stream = new MemoryStream(inputBytes);
        byte[] hashBytes = await sha256.ComputeHashAsync(stream);

        string hex = Convert.ToHexString(hashBytes).ToLower();

        return Path.Combine($"{hex.Substring(0, 2)}", $"{hex.Substring(2, 4)}");

    }
}