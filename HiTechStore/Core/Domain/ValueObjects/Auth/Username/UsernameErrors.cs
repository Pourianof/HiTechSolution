using System.ComponentModel;

using HiTechStore.Core.Helpers.Result;

namespace HiTechStore.Core.Domain.ValueObjects.Auth.Username;


public static class UsernameErrors
{
    public static ValidationResultError UsernameRequiredNotToBeNullOrEmpty() => new()
    {
        Title = "Username is empty",
        Description = "Username must not to be a null or empty string",
        Code = nameof(UsernameRequiredNotToBeNullOrEmpty),
    };

    public static ValidationResultError UsernameStartWithAlphabet() => new()
    {
        Title = "Username invalid",
        Description = "Username must begin with an alphabetic letter",
        Code = nameof(UsernameStartWithAlphabet),
    };

    public static ValidationResultError UsernameRequiredAlphaNumericOrUndescore() => new()
    {
        Title = "Username invalid",
        Description = "Username must include alphabet(a-z or A-Z) or digits(0-9) or underscore(_)",
        Code = nameof(UsernameRequiredAlphaNumericOrUndescore)
    };

    public static ValidationResultError UsernameTooShort() => new()
    {
        Title = "Short username",
        Description = "Username must be at least 4 character contains english alpabet, digits or underscore(_)",
        Code = nameof(UsernameTooShort),
    };
}