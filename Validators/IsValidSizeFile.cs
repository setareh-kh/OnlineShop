using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Validators
{
    public class IsValidSizeFile : ValidationAttribute
    {
        private readonly int _maxSize;
        private readonly bool _isList;

        public IsValidSizeFile(int maxSize, bool isList = false)
        {
            _maxSize = maxSize;
            _isList = isList;
        }

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext)
        {
            if (value is null)
                return ValidationResult.Success;

            if (_isList)
            {
                if (value is not IEnumerable<IFormFile> files)
                    return new ValidationResult("Invalid file collection.");

                int i = 0;

                foreach (var file in files)
                {
                    if (file.Length > _maxSize)
                        return new ValidationResult(
                            $"Size of file {i} exceeds {_maxSize} bytes.");

                    i++;
                }

                return ValidationResult.Success;
            }

            if (value is not IFormFile singleFile)
                return new ValidationResult("Invalid file.");

            return singleFile.Length <= _maxSize
                ? ValidationResult.Success
                : new ValidationResult($"File size exceeds {_maxSize} bytes.");
        }
    }
}