using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MiniAuthMvc.Attributes
{
    public class PasswordComplexityAttribute : ValidationAttribute
    {
        public int MinLength { get; set; } = 8;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var password = value as string ?? "";

            if (password.Length < MinLength)
                return new ValidationResult($"Password must be at least {MinLength} characters long.");

            if (!Regex.IsMatch(password, "[A-Z]"))
                return new ValidationResult("Password must contain at least one uppercase letter.");

            if (!Regex.IsMatch(password, "[a-z]"))
                return new ValidationResult("Password must contain at least one lowercase letter.");

            if (!Regex.IsMatch(password, @"\d"))
                return new ValidationResult("Password must contain at least one number.");

            // Non-alphanumeric special char
            if (!Regex.IsMatch(password, @"[^a-zA-Z0-9]"))
                return new ValidationResult("Password must contain at least one special character.");

            return ValidationResult.Success;
        }
    }
}
