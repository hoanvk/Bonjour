namespace Bonjour.Lib.Services;

public class Message
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public int Loaded { get; set; }
    public int Unloaded { get; set; }
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
}