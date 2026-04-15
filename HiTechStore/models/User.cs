using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

using Microsoft.AspNetCore.Identity;

namespace HiTechStore.Models
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime RegisteredAt { get; set; }
        virtual public IEnumerable<Order>? Orders { get; set; }
        virtual public Cart? ActiveCart { get; set; }
        virtual public IEnumerable<IdentityRole>? Roles { get; set; }
        [NotMapped]
        public IEnumerable<Claim>? Claims { get; set; }
    }
}