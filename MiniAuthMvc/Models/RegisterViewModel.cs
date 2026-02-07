using System.ComponentModel.DataAnnotations;
using MiniAuthMvc.Attributes;

namespace MiniAuthMvc.Models
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = "";

        [Required]
        [EmailAddress]
        [StringLength(120)]
        public string Email { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        [PasswordComplexity(MinLength = 8)]
        public string Password { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = "";
    }
}
