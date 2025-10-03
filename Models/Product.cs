using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bonjour.Models;

public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [ForeignKey("Shipment")]
    public int ShipmentId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public int Delivery { get; set; }
    public DateTime CreatedAt { get; set; }
    public Shipment Shipment { get; set; }
}