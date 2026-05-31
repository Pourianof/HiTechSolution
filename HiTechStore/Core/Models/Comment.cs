using HiTechStore.Core;

namespace HiTechStore.Core.Models;

public class Comment : IModel
{
    public int CommentId { get; set; }
    public string? Text { get; set; }
    public int? ProductId { get; set; }
    public DateTime CreatedAt { get; set; }
    virtual public Product? Product { get; set; }
    public string UserId { get; set; } = default!;
    virtual public User? User { get; set; }
    public int? RateId { get; set; }
    virtual public ProductScore? Rate { get; set; }
    public int? ParentId { get; set; }
    virtual public Comment? Parent { get; set; }
    virtual public IEnumerable<Comment>? Responses { get; set; }
}