namespace HiTechStore.Core.Models;

public class UserPermission
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public int PermissionId { get; set; }
    virtual public Permission? Permission { get; set; }
    public string? GrantedByUserId { get; set; }
    public DateTime GrantedAt { get; set; }
    public PermissionScope Scope { get; set; }
}

public enum PermissionScope
{
    All,
    Self
}