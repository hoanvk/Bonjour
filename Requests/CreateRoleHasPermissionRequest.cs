using System.ComponentModel.DataAnnotations;

namespace Bonjour.Requests;

public class CreateRoleHasPermissionRequest
{
    [Required]
    public int PermissionId { get; set; }
    public string Action { get; set; }
}