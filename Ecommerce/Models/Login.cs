using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class Login
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string password { get; set; }
    }
}
