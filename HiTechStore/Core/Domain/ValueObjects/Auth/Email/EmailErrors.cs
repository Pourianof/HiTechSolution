using HiTechStore.Core.Helpers.Result;

namespace HiTechStore.Core.Domain.ValueObjects.Auth.Email;

public static class EmailErrors
{
    public static ValidationResultError EmptyOrNullEmail() => new()
    {
        Title = "Null or empty email",
        Description = "Email could not be null or empty string",
        Code = nameof(EmptyOrNullEmail)
    };

    public static ValidationResultError InvalidEmail() => new()
    {
        Title = "Invalid email",
        Description = "Specified email is not a valid email address",
        Code = nameof(InvalidEmail),
    };
}