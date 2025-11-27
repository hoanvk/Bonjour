using Bonjour.Domain.Products;
using Bonjour.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QRCoder;

namespace Bonjour.Domain.Contracts;

public class GenerateQrCodeHandler : IRequestHandler<GenerateQrCodeRequest, int>
{
    private readonly ApplicationDbContext dbContext;
    private readonly ILogger<GenerateQrCodeHandler> logger;
    public GenerateQrCodeHandler(ApplicationDbContext dbContext, ILogger<GenerateQrCodeHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }
    public async Task<int> Handle(GenerateQrCodeRequest request, CancellationToken cancellationToken)
    {
        int success = 0;
        var _model = await dbContext.Products.Join(dbContext.ProductDetails,
                    product => product.Id,
                    productDetail => productDetail.ProductId,
                    (product, productDetail) => new { product, productDetail })
                        .Where(t => t.product.ContractId == request.ContractId && t.productDetail.Status == ProductStatus.RESERVED.Code)
                        .ToListAsync();
        using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
        {
            foreach (var _item in _model)
            {
                var _productDetail = dbContext.ProductDetails.FirstOrDefault(pd => pd.Id == _item.productDetail.Id);
                _productDetail.Status = ProductStatus.AVAILABLE.Code;
                dbContext.ProductDetails.Update(_productDetail);

                // Create QR code data with the desired error correction level (e.g., ECCLevel.Q for 25% error correction)
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(_item.productDetail.ShortId, QRCodeGenerator.ECCLevel.Q);

                // Create a PNG byte array from the QR code data
                using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
                {
                    byte[] qrCodeImageBytes = qrCode.GetGraphic(10);
                    string directory = Path.Combine("storage", "qrcode", $"{_item.product.ContractId}", _item.product.Name);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    // Save the byte array as a PNG file
                    await System.IO.File.WriteAllBytesAsync(Path.Combine(directory, $"{_item.productDetail.ShortId}.png"), qrCodeImageBytes);
                    success++;
                }
            }
            await dbContext.SaveChangesAsync();
        }
        return success;
    }
}