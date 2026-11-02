using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public class Order : ISqlEntity
    {
        public int Id { get; set; }
        [Required]
        public required decimal TotalPrice { get; set; }
        [Required]
        public required DateTime CreateAt { get; set; }
        public DateTime? UpdateDate { get; set; }
        //Fk 
        [Required]
        public required int UserId { get; set; }
        public User User { get; set; } = null!;
        //
        public ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();

    }
}