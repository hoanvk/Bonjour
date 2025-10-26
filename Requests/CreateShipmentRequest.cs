namespace Bonjour.Requests;

public class CreateShipmentRequest
{
    public string Carrier { get; set; }
    public string Consignee { get; set; }
    public DateTime Departure { get; set; }
}