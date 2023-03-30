using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsActive { get; set; }
    }
}
