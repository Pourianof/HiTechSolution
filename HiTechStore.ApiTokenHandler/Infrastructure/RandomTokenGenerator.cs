
using System.Security.Cryptography;

using HiTechStore.ApiTokenHandler.Core;

namespace HiTechStore.ApiTokenHandler.Infrastructure;

internal class RandomTokenGenerator : IRandomSecureTokenGenerator
{
    public Task<string> Genreate()
    {
        var randomNumber = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Task.FromResult(Convert.ToBase64String(randomNumber));
        }
    }
}