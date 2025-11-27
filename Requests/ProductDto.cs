namespace Bonjour.Dtos;

public class ProductDto
{
    public ProductDto(int id, string contract, string category, string name, string qrCode)
    {
        Id = id;
        Contract = contract;
        Category = category;
        Name = name;
        QrCode = qrCode;
    }

    public int Id { get; set; }
    public string Contract { get; set; }
    public string Category { get; set; }
    public string Name { get; set; }
    public string QrCode { get; set; }

}