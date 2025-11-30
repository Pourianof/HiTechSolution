namespace HiTechPay.Endpoints;

public static class KeysEndpoints
{
    public static IEndpointRouteBuilder MapKeysEndpoints(this IEndpointRouteBuilder app)
    {
        var keysGroup = app.MapGroup("/keys");

        keysGroup.MapGetPublicKey();

        return app;
    }
}