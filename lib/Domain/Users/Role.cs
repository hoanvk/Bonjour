namespace Bonjour.Domain.Users;

public class Role
{
    public string Name { get; set; }
    public IList<Permission> Permissions { get; set; }
}