namespace HiTechStore.Core.Helpers.Result;

public class Result<T>
{
    public T? Value { get; set; }
    public IEnumerable<ResultError>? Errors { get; set; }

    public bool HasError => Errors?.Count() > 0;
    public bool IsValid => !HasError;
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

