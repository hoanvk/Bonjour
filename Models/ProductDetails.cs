using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bonjour.Models;

public class ProductDetails
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [ForeignKey("Product")]
    public int ProductId { get; set; }
    public int SequenceNo { get; set; }
    [MaxLength(100)]
    public string ShortId { get; set; }
    [MaxLength(20)]
    public string Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Product Product { get; set; }
}