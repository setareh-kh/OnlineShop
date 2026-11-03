using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Validators
{
    public class IsValidPercent:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not int number) return new ValidationResult("nummer should be percent");
            double num=Convert.ToInt32(value);
            return (num>=0 && num<=100) ? ValidationResult.Success:new ValidationResult("the number is not valid "); 
        }
    }
}