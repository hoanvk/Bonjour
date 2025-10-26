using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bonjour.Models;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
    [Required]
    [MaxLength(50)]
    public string Username { get; set; }
    public string Email { get; set; }
    [Required]
    [MaxLength(100)]
    public string Password { get; set; }
    public string Salt { get; set; }
    [ForeignKey("Role")]
    public int? RoleId { get; set; }
    public Role? Role { get; set; }
    public DateTime CreatedAt { get; set; }
}