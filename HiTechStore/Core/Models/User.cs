using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

using Microsoft.AspNetCore.Identity;

namespace HiTechStore.Core.Models
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime RegisteredAt { get; set; }
        public string? AvatarUrl { get; set; }
        virtual public IEnumerable<Order>? Orders { get; set; }
        virtual public Cart? ActiveCart { get; set; }
        // maybe it better to include these two props in dto, but im tired
        [NotMapped]
        public IEnumerable<Claim>? Claims { get; set; }
        [NotMapped]
        public IEnumerable<string> Roles { get; set; } = [];
        virtual public List<UserPermission>? Permissions { get; set; }
    }
}