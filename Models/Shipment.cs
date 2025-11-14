using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bonjour.Models;

public class Shipment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [MaxLength(255)]
    public string Carrier { get; set; }
    [MaxLength(255)]
    public string Consignee { get; set; }
    public DateTime Departure { get; set; }
    [MaxLength(20)]
    public string? Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}