
using HiTechPay.Sdk;

using HiTechStore.Core.Common.Interfaces.Infra;

namespace HiTechStore.Infrastructure.Payment;

public class HiTechPayPaymentConfirmationVerifier(IHiTechPaySdkFacade hiTechPaySdkFacade) : IPaymentConfirmationVerifier
{
    public Task<bool> Verify(string data, string signature) => hiTechPaySdkFacade.Verifier.Verify(data, signature);
}