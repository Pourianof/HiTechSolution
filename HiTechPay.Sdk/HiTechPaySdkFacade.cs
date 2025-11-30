using HiTechPay.Sdk.Keys;

namespace HiTechPay.Sdk;

public interface IHiTechPaySdkFacade
{
    IVerifier Verifier { get; }
}

public class HiTechPaySdkFacade : IHiTechPaySdkFacade
{
    public IVerifier Verifier { get; init; }

    public HiTechPaySdkFacade(IVerifier verifier)
    {
        Verifier = verifier;
    }
}