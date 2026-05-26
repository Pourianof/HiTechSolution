namespace HiTechStore.Core.Common.Interfaces.Infra;

public sealed record EmailNotification(
    string To,
    string Subject,
    string TemplateName,
    object? Model = null
);
