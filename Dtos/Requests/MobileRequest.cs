using System.ComponentModel.DataAnnotations;
using OnlineShop.Validators;

namespace OnlineShop.Dtos.Requests
{
    public class MobileRequest
    {
        [Required(ErrorMessage = "please enter your MobileNumber"), MaxLength(250)]
        [IsValidMobileNumber]
        public required string Mobile { get; set; }
    }
}