namespace HiTechPay.Endpoints;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api");
        group.MapKeysEndpoints();

        return app;
    }
}