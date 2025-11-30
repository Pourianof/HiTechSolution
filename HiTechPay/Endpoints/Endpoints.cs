namespace HiTechPay.Endpoints;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapKeysEndpoints();

        return app;
    }
}