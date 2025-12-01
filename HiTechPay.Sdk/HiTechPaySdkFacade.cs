using HiTechPay.Sdk.Connection;
using HiTechPay.Sdk.Keys;

namespace HiTechPay.Sdk;

public interface IHiTechPaySdkFacade
{
    IVerifier Verifier { get; }
    IServerConnectionHelper ServerConnectionHelper { get; }

}

public class HiTechPaySdkFacade : IHiTechPaySdkFacade
{
    public IVerifier Verifier { get; init; }
    public IServerConnectionHelper ServerConnectionHelper { get; init; }
    public HiTechPaySdkFacade(IVerifier verifier, IServerConnectionHelper serverConnectionHelper)
    {
        Verifier = verifier;
        ServerConnectionHelper = serverConnectionHelper;
    }
}