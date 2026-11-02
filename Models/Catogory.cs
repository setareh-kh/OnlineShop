using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public class Category : ISqlEntity
    {
        public int Id { get; set; }
        [Required, MaxLength(250)]
        public required string Name { get; set; }
        [Required, MaxLength(500)]
        public required string Description { get; set; }
    }
}