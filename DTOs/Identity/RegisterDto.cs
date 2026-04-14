using System.ComponentModel.DataAnnotations;

namespace MyApp.DTOs.Identity;

public class RegisterDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = null!;

    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; } = null!;
}
