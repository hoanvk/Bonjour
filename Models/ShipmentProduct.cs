using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bonjour.Models;

public class ShipmentProduct
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [ForeignKey("Shipment")]
    public int? ShipmentId { get; set; }
    [ForeignKey("Product")]
    public int? ProductId { get; set; }
    public int? Loading { get; set; }
    public int? Unloading { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Shipment? Shipment { get; set; }
    public Product? Product { get; set; }
}