using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using OnlineShop.Validators;

namespace OnlineShop.Dtos.Requests
{
    public class AddProductRequest
    {
        [Required(ErrorMessage ="please enter product's name"), MaxLength(250)]
        public required string Name { get; set; }
        [Required(ErrorMessage ="please enter product's price")]
        public required decimal Price { get; set; }
        [Required(ErrorMessage ="please enter Quantity")]
        [DefaultValue(2)]
        public required int Quantity { get; set; }
        [Required]
        [Range(0, 100, ErrorMessage = "nummer should be percent")]
        [DefaultValue(0)]
        public required int Discount { get; set; }
        [Required]
        public required bool Active { get; set; }
        [Required]
        public required int CategoryId { get; set; }
        [Required(ErrorMessage ="please upload file")]
        [AllowedExtnsions([ ".jpg", ".jpeg",".png", ".bmp", ".gif", ".tga", ".tiff", ".jfif"],true)]
        [IsValidSizeFile(2 * 1024 * 1024, true)]
        public List<IFormFile>? Images { get; set; }
        
    }
}