using System.Security.Cryptography;
using System.Text;

namespace HiTechStore.Core.Helpers;

public interface IDiscountCodeGenerator
{
    string GenerateCode(int length);
}

public class DiscountCodeGenerator : IDiscountCodeGenerator
{
    private const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    public string GenerateCode(int length)
    {
        var result = new StringBuilder(length);
        byte[] buffer = new byte[length];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(buffer);
        }

        for (int i = 0; i < length; i++)
        {
            result.Append(chars[buffer[i] % chars.Length]);
        }

        return result.ToString();
    }
}