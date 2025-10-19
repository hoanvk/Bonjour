using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bonjour.Models;

public class RoleHasPermission
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [ForeignKey("Role")]
    public int RoleId { get; set; }
    [ForeignKey("Permission")]
    public int PermissionId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Role Role { get; set; }
    public Permission Permission { get; set; }
}