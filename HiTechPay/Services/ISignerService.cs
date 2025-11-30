using System.Security.Cryptography;

namespace HiTechPay.Services;

public interface ISignerService
{
    string Sign(string text);
}