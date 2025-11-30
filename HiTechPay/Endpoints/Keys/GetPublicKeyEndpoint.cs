using HiTechPay.Infrastructure;

public static class GetPublicKeyEndpoint
{
    public const string Name = "GetPubKey";

    public static IEndpointRouteBuilder MapGetPublicKey(this IEndpointRouteBuilder app)
    {
        app.MapGet("pub-key", (HttpContext context, IRsaProvider rsaProvider, CancellationToken token) =>
        {
            string pem = rsaProvider.GetPrivateKey().ExportRSAPublicKeyPem();

            return Results.Content(pem, "application/x-pem-file");

        }).WithName(Name);

        return app;
    }
}