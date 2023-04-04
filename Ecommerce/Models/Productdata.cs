

using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class Productdata
    {
        [Key]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public double DiscountAmount { get; set; }
    }
}
