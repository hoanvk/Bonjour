using System.ComponentModel.DataAnnotations;

namespace Bonjour.Models;

public class AccountModel
{
    [MaxLength(20)]
    public string Username { get; set; }
    [MaxLength(50)]
    public string Password { get; set; }
}