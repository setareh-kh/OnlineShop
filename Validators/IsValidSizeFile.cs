using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Validators
{
    public class IsValidSizeFile(int maxSize,  bool isList = false) : ValidationAttribute
    {
        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext)
        {
            if (value is null)
                return ValidationResult.Success;

            if (isList)
            {
                if (value is not IEnumerable<IFormFile> files)
                    return new ValidationResult("Invalid file collection.");

                int i = 0;

                foreach (var file in files)
                {
                    if (file.Length > maxSize)
                        return new ValidationResult(
                            $"Size of file {i} exceeds {maxSize} bytes.");

                    i++;
                }

                return ValidationResult.Success;
            }

            if (value is not IFormFile singleFile)
                return new ValidationResult("Invalid file.");

            return singleFile.Length <= maxSize
                ? ValidationResult.Success
                : new ValidationResult($"File size exceeds {maxSize} bytes.");
        }
    }
}