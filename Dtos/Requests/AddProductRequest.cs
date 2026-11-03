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
        //[IsValidPercent]//=
        [Range(0, 100, ErrorMessage = "nummer should be percent")]
        [DefaultValue(0)]
        public required int Discount { get; set; }
        [Required]
        public required bool Active { get; set; }
        [Required]
        public required int CategoryId { get; set; }
        [Required(ErrorMessage ="please upload file")]
        [AllowedExtnsions(new[] {".jpg",".jepg",".png",".bmp",".gif",".tga",".tiff",".jfif"},isList:true)]
        [IsValidSizeFile(131072,isList:true)]//13107=128KB
        public List<IFormFile>? Images { get; set; }
        
    }
}