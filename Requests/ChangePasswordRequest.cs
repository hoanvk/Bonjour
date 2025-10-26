using System.ComponentModel.DataAnnotations;

namespace Bonjour.Requests;

public class ChangePasswordRequest
{

    [Required]
    [MaxLength(100)]
    public string Password { get; set; }
    [Required]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}