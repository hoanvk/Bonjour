using System.ComponentModel.DataAnnotations;

namespace Bonjour.Requests;

public class CreateRoleRequest
{
    [Required]
    public string Name { get; set; }
    public string Permissions { get; set; }
}