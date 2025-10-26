namespace Bonjour.Domain.Shipments;

public class ShipmentProductDto
{
    public ShipmentProductDto(int id, string code, string name, int loaded, int unloaded, DateTime createdAt, DateTime? updatedAt)
    {
        Id = id;
        Code = code;
        Name = name;
        Loaded = loaded;
        Unloaded = unloaded;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public int Loaded { get; set; }
    public int Unloaded { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

}