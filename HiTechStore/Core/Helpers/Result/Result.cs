namespace HiTechStore.Core.Helpers.Result;

public class Result
{
    public List<ResultError> Errors { get; init; } = new();
    public bool HasError => Errors?.Count() > 0;

}

public class Result<T> : Result
{
    public T? Value { get; set; }
    public bool IsValid => !HasError;

    public Result<T> AddError(ResultError error)
    {
        Errors.Add(error);

        return this;
    }

    public Result<T> AddAllErrors(IEnumerable<ResultError> erros)
    {
        Errors.AddRange(erros);

        return this;
    }

    public Result<T> Failure(string title, string? description = default, string? code = default)
    {
        return new()
        {
            Errors = [
                new ResultError() {
                    Title=title,
                    Description = description,
                    Code = code
                }
            ]
        };
    }

    public Result<TNewType> WithValue<TNewType>(TNewType value)
    {
        var result = new Result<TNewType>
        {
            Value = value
        };

        if (HasError)
        {
            result.AddAllErrors(Errors!);
        }

        return result;
    }
}

public class ResultError
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Code { get; set; }

    public ResultError() { }

    public ResultError(string title, string? description, string? code)
    {
        Title = title;
        Description = description;
        Code = code;
    }
}

public class ValidationResultError : ResultError
{
    public string? FieldName { get; set; }
    public ValidationResultError() { }

    public ValidationResultError(string title, string? description, string? code, string? fieldName)
        : base(title, description, code)
    {
        FieldName = fieldName;
    }
}

