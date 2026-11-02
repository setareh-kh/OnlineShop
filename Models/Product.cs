using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public class Product:ISqlEntity
    {
        public int Id { get; set; }
        [Required, MaxLength(250)]
        public required string Name { get; set; }
        [Required]
        public required decimal Price { get; set; }
        [Required]
        public required int Quantity { get; set; }
        [Required]
        public required int Discount { get; set; }
        [Required]
        public required bool Available { get; set; }
        [Required]
        public required bool Active { get; set; }
        [Required]
        public required DateTime CreateAt { get; set; }
        public  DateTime? UpdateDate { get; set; }
        //Fk 
        [Required]
        public required int CategoryId { get; set; }
        public Category? Category { get; set; }

    }
}