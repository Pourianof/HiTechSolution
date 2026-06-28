namespace HiTechStore.Core.Models;


public class PermissionAudit : IModel
{
    public int Id { get; set; }

    virtual public User? ActorUser { get; set; }

    virtual public User? TargetUser { get; set; }

    virtual public Permission? Permission { get; set; }

    public PermissionAction Action { get; set; }

    public DateTime OccurredAt { get; set; }
}

public enum PermissionAction
{
    Granted,
    Revoked
}