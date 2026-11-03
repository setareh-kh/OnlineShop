using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Validators
{
    public class AllowedExtnsions(string[] extensions,  bool isList = false) : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
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
                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

                    if (!extensions.Contains(ext))
                        return new ValidationResult($"File {i} format is not valid.");

                    i++;
                }

                return ValidationResult.Success;
            }

            if (value is not IFormFile singleFile)
                return new ValidationResult("Invalid file.");

            var extension = Path.GetExtension(singleFile.FileName).ToLowerInvariant();

            return extensions.Contains(extension)
                ? ValidationResult.Success
                : new ValidationResult("File format is not valid.");
        }
    }
}