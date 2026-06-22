namespace HiTechStore.Core.Helpers.Result;

public static class ResultExtenstion
{
    /// <summary>
    /// Assign a fieldname to ValidationResultError
    /// </summary>
    public static Result<T> WithFieldname<T>(this Result<T> result, string fieldname)
    {
        var errors = result.Errors.Select(
           err =>
           {
               if (err is ValidationResultError e)
               {
                   e.FieldName = fieldname;
               }

               return err;
           }
        ).ToList();

        return new Result<T>()
        {
            Errors = errors,
            Value = result.Value
        };
    }
}