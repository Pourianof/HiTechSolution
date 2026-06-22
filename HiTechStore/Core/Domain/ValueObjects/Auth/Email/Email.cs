using System.ComponentModel.DataAnnotations;

using HiTechStore.Core.Helpers.Result;
using HiTechStore.Core.Services.Authorization;

namespace HiTechStore.Core.Domain.ValueObjects.Auth.Email;

public sealed class EmailValueObject : ValueObject
{
    public string Email { get; }
    private EmailValueObject(string email)
    {
        Email = email;
    }

    public static Result<EmailValueObject> Create(string email)
    {
        var result = new Result<EmailValueObject>();

        if (string.IsNullOrEmpty(email))
        {
            result.AddError(
                AuthErrors.RegistrationErrors.EmailRequired()
            );
        }
        else if (!IsValid(email))
        {
            result.AddError(
               EmailErrors.InvalidEmail()
            );

            return result;
        }

        return new Result<EmailValueObject>
        {
            Value = new EmailValueObject(email)
        };
    }

    private static bool IsValid(string email)
    {
        return new EmailAddressAttribute().IsValid(email);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        return [Email];
    }
}