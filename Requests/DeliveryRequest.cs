using System.ComponentModel.DataAnnotations;

namespace Bonjour.Requests;

public class DeliveryRequest
{
    [Required]
    public string message { get; set; }
}