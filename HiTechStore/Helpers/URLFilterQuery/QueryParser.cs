using System.Text.RegularExpressions;

namespace HiTechStore.Helpers.URLFilterQuery;

public class QueryParser : IQueryParser
{
    public Queries Parse(IQueryCollection queryParams)
    {
        var queries = new Queries();

        foreach (var (key, values) in queryParams)
        {
            // eg: key = "price[gte]"
            var (field, op) = ParseKey(key); // like : ("price", "gte")
            var @operator = MapToOperator(op);
            queries.Register(field, @operator, values);
        }

        return queries;
    }

    private static (string field, string op) ParseKey(string key)
    {
        var match = Regex.Match(key, @"^(.+)\[(.+)\]$");
        if (match.Success)
            return (match.Groups[1].Value, match.Groups[2].Value);
        return (key, "eq");
    }

    private static QueryOperator MapToOperator(string operatorStr) => operatorStr.ToLower().Trim() switch
    {
        "eq" => QueryOperator.Equal,
        "gt" => QueryOperator.GreaterThan,
        "gte" => QueryOperator.GreaterThanOrEqual,
        "lt" => QueryOperator.LessThan,
        "lte" => QueryOperator.LessThanOrEqual,
        "in" => QueryOperator.In,
        _ => QueryOperator.Equal
    };
}
