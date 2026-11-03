using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace OnlineShop.Validators
{
    public class IsValidMobileNumber : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var mobileNumber = value as string;
            if (string.IsNullOrEmpty(mobileNumber)) return ValidationResult.Success;
            const string pattern = @"^(\+98|0)?9\d{9}$";//@"^09[0|1|2|3][0-9]{8}$";
            var reg = new Regex(pattern);
            if (!reg.IsMatch(mobileNumber))
                return new ValidationResult("The MobileNumber is not Valid");
            else return ValidationResult.Success;
        }
    }
}