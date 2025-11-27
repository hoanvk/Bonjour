namespace Bonjour.Domain.Shipments;

public class ShipmentProductDto
{
    public ShipmentProductDto(int id, string category, string name, int loaded, int unloaded, int weight, DateTime createdAt, DateTime? updatedAt)
    {
        Id = id;
        Category = category;
        Name = name;
        Loaded = loaded;
        Unloaded = unloaded;
        Weight = weight;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public int Id { get; set; }
    public string Category { get; set; }
    public string Name { get; set; }
    public int Loaded { get; set; }
    public int Unloaded { get; set; }
    public int Weight { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

}