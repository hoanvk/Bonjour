using System.ComponentModel.DataAnnotations;

namespace Bonjour.Requests;

public class EditUserRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
    [Required]
    [MaxLength(50)]
    public string Username { get; set; }
    [Required]
    public string Roles { get; set; }

}