using System.ComponentModel.DataAnnotations;

namespace Bonjour.Requests;

public class CreateContractRequest
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Customer { get; set; }
    [Required]
    public string StartDate { get; set; }
    [Required]
    public string EndDate { get; set; }
}