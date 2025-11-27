using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bonjour.Models;

public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [ForeignKey("Contract")]
    public int? ContractId { get; set; }
    [MaxLength(100)]
    public string Category { get; set; }
    [MaxLength(255)]
    public string Name { get; set; }
    public int Quantity { get; set; }
    public int Delivery { get; set; }
    public int? Weight { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Contract? Contract { get; set; }
}