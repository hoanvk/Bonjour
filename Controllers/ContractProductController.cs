using Microsoft.AspNetCore.Mvc;
using Bonjour.Models;
using OfficeOpenXml;
using QRCoder;
using Microsoft.EntityFrameworkCore;
using Bonjour.Dtos;
using Microsoft.AspNetCore.Authorization;
using Bonjour.Domain.Products;

namespace Bonjour.Controllers;

[Authorize]
public class ContractProductController : Controller
{
    private readonly IConfiguration configuration;
    private readonly ILogger<ContractProductController> logger;
    private readonly ApplicationDbContext dbContext;

    public ContractProductController(IConfiguration configuration, ILogger<ContractProductController> logger, ApplicationDbContext dbContext)
    {
        this.configuration = configuration;
        this.logger = logger;
        this.dbContext = dbContext;
    }
    [HttpGet("/Contract/{id}/Product")]
    public async Task<IActionResult> Index(int id)
    {
        var _products = await dbContext.Products.Where(product => product.ContractId == id).ToListAsync();
        ViewBag.id = id;
        return View("Index", _products);
    }

    [HttpPost("/Contract/{id}/Product")]
    public async Task<IActionResult> Import(int id, IEnumerable<IFormFile> files)
    {
        logger.LogInformation("Import files");
        if (files != null && files.Any())
        {
            foreach (var file in files)
            {
                // Process each file
                // Example: Save to wwwroot/uploads folder
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "storage", "imports");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                // For non-commercial use, you can use LicenseContext.NonCommercial
                ExcelPackage.License.SetNonCommercialPersonal("Bonjour");

                FileInfo fileInfo = new FileInfo(filePath);

                try
                {
                    using (ExcelPackage package = new ExcelPackage(fileInfo))
                    {
                        // Get the first worksheet
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                        // Determine the range of used cells
                        int startRow = worksheet.Dimension.Start.Row;
                        int endRow = worksheet.Dimension.End.Row;
                        int startCol = worksheet.Dimension.Start.Column;
                        int endCol = worksheet.Dimension.End.Column;

                        // Loop through rows (assuming first row is header)
                        for (int rowNum = startRow + 1; rowNum <= endRow; rowNum++)
                        {
                            Product product = new Product();
                            // Assuming columns are in a specific order: Name, Age, City
                            product.Code = worksheet.Cells[rowNum, startCol].Text;
                            product.Name = worksheet.Cells[rowNum, startCol + 1].Text;
                            product.Quantity = int.Parse(worksheet.Cells[rowNum, startCol + 2].Text);
                            product.Delivery = 0;
                            product.ContractId = id;
                            dbContext.Products.Add(product);
                            for (int i = 0; i < product.Quantity; i++)
                            {
                                var _productDetail = new ProductDetails()
                                {
                                    Product = product,
                                    Key = "qrcode",
                                    Value = ShortIdGenerator.Generate(12),
                                };
                                dbContext.ProductDetails.Add(_productDetail);
                            }
                        }

                        await dbContext.SaveChangesAsync();
                        var _model = await dbContext.Products.Join(dbContext.ProductDetails,
                            product => product.Id,
                            productDetail => productDetail.ProductId,
                            (product, productDetail) => new { product, productDetail })
                                .Where(t => t.product.ContractId == id && t.productDetail.Key == "qrcode")
                                .ToListAsync();
                        using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                        {
                            foreach (var _item in _model)
                            {

                                // Create QR code data with the desired error correction level (e.g., ECCLevel.Q for 25% error correction)
                                QRCodeData qrCodeData = qrGenerator.CreateQrCode(_item.productDetail.Value, QRCodeGenerator.ECCLevel.Q);

                                // Create a PNG byte array from the QR code data
                                using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
                                {
                                    byte[] qrCodeImageBytes = qrCode.GetGraphic(10);
                                    string directory = Path.Combine("storage", "qrcode", $"{_item.product.ContractId}", _item.product.Code);
                                    if (!Directory.Exists(directory))
                                    {
                                        Directory.CreateDirectory(directory);
                                    }
                                    // Save the byte array as a PNG file
                                    await System.IO.File.WriteAllBytesAsync(Path.Combine(directory, $"{_item.productDetail.Value}.png"), qrCodeImageBytes);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error importing file");
                    return UnprocessableEntity(ex.Message);
                }
                fileInfo.Delete();
            }
            return Ok("Files uploaded successfully!");
        }
        return BadRequest("No files selected.");
    }

    [HttpGet("/Contract/{id}/Product/QrCode")]
    public async Task<IActionResult> QrCode(int id)
    {
        var model = await dbContext.Products.Join(dbContext.ProductDetails,
        product => product.Id,
        productDetail => productDetail.ProductId,
        (product, productDetail) => new { product, productDetail }).Where(model => model.product.ContractId == id && model.productDetail.Key == "qrcode").ToListAsync();
        var products = model.Select(item => new ProductDto(item.productDetail.Id, item.product.Code, item.product.Name, $"/QrCode/{item.product.ContractId}/{item.product.Code}/{item.productDetail.Value}.png"));
        ViewBag.id = id;
        return View("QrCode", products);
    }
}