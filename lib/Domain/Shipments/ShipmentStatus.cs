using Bonjour.Domain.Helpers;

namespace Bonjour.Domain.Shipments;

public class ShipmentStatus : ValueObject
{
    public static readonly ShipmentStatus PENDING = new ShipmentStatus("pending");
    public static readonly ShipmentStatus IN_TRANSIT = new ShipmentStatus("in_transit");
    public static readonly ShipmentStatus DELIVERED = new ShipmentStatus("delivered");
    public string Code { get; set; }
    public ShipmentStatus(string code)
    {
        Code = code;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
    }
}