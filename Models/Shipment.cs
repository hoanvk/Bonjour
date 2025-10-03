using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bonjour.Models;

public class Shipment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Carrier { get; set; }
    public string Consignee { get; set; }
    public DateTime Departure { get; set; }
    public DateTime CreatedAt { get; set; }
}