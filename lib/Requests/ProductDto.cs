namespace Bonjour.Dtos;

public class ProductDto
{
    public ProductDto(int id, string code, string name, string qrCode)
    {
        Id = id;
        Code = code;
        Name = name;
        QrCode = qrCode;
    }

    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string QrCode { get; set; }
}