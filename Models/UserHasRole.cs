using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bonjour.Models;

public class UserHasRole
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [ForeignKey("User")]
    public int UserId { get; set; }
    [ForeignKey("Role")]
    public int RoleId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Role Role { get; set; }
    public User User { get; set; }
}