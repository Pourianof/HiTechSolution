using Npgsql;

namespace HiTechStore.Helpers.Types;

public static class IConfigurationExtension
{
    public static string ProvideConnectionString(this IConfiguration configuration)
    {
        var baseConnStr = configuration.GetConnectionString("DefaultConnection");
        var username = configuration["Db:Username"];
        var password = configuration["Db:Password"];

        var fullConnStr = new NpgsqlConnectionStringBuilder(baseConnStr)
        {
            Username = username,
            Password = password,
        }.ConnectionString;

        return fullConnStr;
    }
}