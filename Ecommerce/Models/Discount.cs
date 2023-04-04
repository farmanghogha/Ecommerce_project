

using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class Discount
    {
        [Key]
        public int DId { get; set; }
        public int ProductId { get; set; }
        public DiscountType DiscountType { get; set; }
        public double Amount { get; set; }
        public DateTime FromTo { get; set; }
        public DateTime validTo { get; set; }
    }
}
