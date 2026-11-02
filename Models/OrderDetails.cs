using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public class OrderDetails : ISqlEntity
    {
       [Key] public int Id { get; set; }

        // FK -> Order
        [Required]
        public required int OrderId { get; set; }
        public Order Order { get; set; } = null!;
        // FK -> Product
        [Required]
        public required int ProductId { get; set; }
        public Product Product { get; set; }  = null!;
        //
        [Required]
        public required decimal UnitPrice { get; set; }
        [Required]
        public required int Discount { get; set; }
        [Required]
        public required int Quantity { get; set; }
        [Required]
        public required DateTime CreateAt { get; set; }
        public DateTime? UpdateDate { get; set; }

    }
}