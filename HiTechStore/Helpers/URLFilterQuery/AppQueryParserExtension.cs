namespace HiTechStore.Helpers.URLFilterQuery;

public static class AppQueryParserExtension
{
    public static IServiceCollection AddQueryParser(this IServiceCollection services)
    {
        return services.AddScoped<IQueryParser, QueryParser>();
    }
}