using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class Dealer
    {
        public string? UserName { get; set; }
        [Key]
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Password { get; set; }
        public Status? status { get; set; }
        public string? reason { get; set; }
    }
}
