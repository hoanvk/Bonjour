using Bonjour.Domain.Helpers;

namespace Bonjour.Domain.Products;

public class ProductStatus : ValueObject
{
    public static readonly ProductStatus AVAILABLE = new ProductStatus("available");
    public static readonly ProductStatus RESERVED = new ProductStatus("reserved");
    public static readonly ProductStatus LOADED = new ProductStatus("loaded");
    public static readonly ProductStatus UNLOADED = new ProductStatus("unloaded");
    public string Code { get; set; }
    public ProductStatus(string code)
    {
        Code = code;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
    }
}