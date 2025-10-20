using System.ComponentModel.DataAnnotations;

namespace Bonjour.Requests;

public class CreatePermissionRequest
{
    [Required]
    public string Name { get; set; }

}