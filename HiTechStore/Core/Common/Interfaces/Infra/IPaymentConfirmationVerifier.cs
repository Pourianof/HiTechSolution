namespace HiTechStore.Core.Common.Interfaces.Infra;

public interface IPaymentConfirmationVerifier
{
    Task<bool> Verify(string data, string signature);
}