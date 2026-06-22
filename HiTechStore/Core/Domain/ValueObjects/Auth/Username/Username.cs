using System.Text.RegularExpressions;

using HiTechStore.Core.Helpers.Result;

namespace HiTechStore.Core.Domain.ValueObjects.Auth.Username;

public class UsernameValueObject : ValueObject
{
    public static Result<UsernameValueObject> Create(string? username)
    {
        Result<UsernameValueObject> result = new();

        if (string.IsNullOrEmpty(username))
        {
            result.AddError(
                UsernameErrors.UsernameRequiredNotToBeNullOrEmpty()
            );
        }
        else
        {
            if (!char.IsAsciiLetter(username.ElementAt(0)))
            {
                result.AddError(
                    UsernameErrors.UsernameStartWithAlphabet()
                );
            }

            var minimumLength = 4;

            if (username.Count() < minimumLength)
            {
                result.AddError(
                    UsernameErrors.UsernameTooShort()
                );
            }

            var usernameRegexp = new Regex($"^\\w{{{minimumLength},}}$", RegexOptions.ECMAScript);
            if (!usernameRegexp.IsMatch(username))
            {
                result.AddError(
                    UsernameErrors.UsernameRequiredAlphaNumericOrUndescore()
                );
            }
        }

        if (result.IsValid)
        {
            result.Value = new UsernameValueObject(username!);
        }

        return result;
    }

    public string Username { get; }
    private UsernameValueObject(string username)
    {
        Username = username;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        return [Username];
    }
}