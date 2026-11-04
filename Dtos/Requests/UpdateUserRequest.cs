using System.ComponentModel.DataAnnotations;
using OnlineShop.Validators;

namespace OnlineShop.Dtos.Requests
{
    public class UpdateUserRequest
    {
        public  string? Name { get; set; }
        [AllowedExtnsions([ ".jpg", ".jpeg",".png", ".bmp", ".gif", ".tga", ".tiff", ".jfif"])]
        [IsValidSizeFile(2 * 1024 * 1024)]
        public  IFormFile? Image { get; set; }
    }
}