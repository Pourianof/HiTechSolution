using HiTechStore.Core.Dto.Auth;
using HiTechStore.Core.Helpers.Result;

namespace HiTechStore.Core.Services.Authorization;

public static class AuthorizationErrors
{
    public static ValidationResultError OldPasswordRequired() =>
        new("Invalid data", "Current password must be provided.", "OldPasswordRequired", nameof(ChangePasswordDto.OldPassword));

    public static ValidationResultError NewPasswordRequired() =>
        new("Invalid data", "New password must be provided.", "NewPasswordRequired", nameof(ChangePasswordDto.NewPassword));

    public static ValidationResultError PasswordConfirmationRequired() =>
        new("Invalid data", "Password confirmation must be provided.", "PasswordConfirmationRequired", nameof(ChangePasswordDto.PasswordConfirmation));

    public static ValidationResultError PasswordConfirmationMismatch() =>
        new("Password mismatch", "Password confirmation does not match.", "PasswordConfirmationMismatch", nameof(ChangePasswordDto.PasswordConfirmation));

    public static ValidationResultError PasswordMismatch(string? description = null) =>
        new("Current password is incorrect.", description ?? "Current password is incorrect.", "PasswordMismatch", nameof(ChangePasswordDto.OldPassword));

    public static ValidationResultError PasswordRequiresDigit() =>
        new("Weak password", "Password must include at least one digit.", "PasswordRequiresDigit", nameof(ChangePasswordDto.NewPassword));

    public static ValidationResultError PasswordRequiresLower() =>
        new("Weak password", "Password must include at least one lowercase letter.", "PasswordRequiresLower", nameof(ChangePasswordDto.NewPassword));

    public static ValidationResultError PasswordRequiresUpper() =>
        new("Weak password", "Password must include at least one uppercase letter.", "PasswordRequiresUpper", nameof(ChangePasswordDto.NewPassword));

    public static ValidationResultError PasswordRequiresNonAlphanumeric() =>
        new("Weak password", "Password must include at least one special character.", "PasswordRequiresNonAlphanumeric", nameof(ChangePasswordDto.NewPassword));

    public static ValidationResultError PasswordTooShort(string? description = null) =>
        new("Weak password", description ?? "Password is too short.", "PasswordTooShort", nameof(ChangePasswordDto.NewPassword));

    public static ValidationResultError GenericPassword(string title, string? description, string? code, string? fieldName = null) =>
        new(title, description, code, fieldName);
}