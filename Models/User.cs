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
    public DateTime CreatedAt { get; set; }
}