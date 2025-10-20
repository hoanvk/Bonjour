using System.ComponentModel.DataAnnotations;

namespace Bonjour.Domain.Users;

public class User
{
    public string Name { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public IList<Role> Roles { get; set; }
}