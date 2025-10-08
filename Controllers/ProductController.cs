using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Bonjour.Models;
using OfficeOpenXml;
using QRCoder;
using Microsoft.EntityFrameworkCore;
using Bonjour.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace Bonjour.Controllers;

[Authorize]
public class ProductController : Controller
{
    private readonly IConfiguration configuration;
    private readonly ILogger<HomeController> logger;
    private readonly ApplicationDbContext dbContext;

    public ProductController(IConfiguration configuration, ILogger<HomeController> logger, ApplicationDbContext dbContext)
    {
        this.configuration = configuration;
        this.logger = logger;
        this.dbContext = dbContext;
    }

    [HttpGet("/Shipment/{id}/Product")]
    public async Task<IActionResult> Index(int id)
    {
        var _products = await dbContext.Products.Where(product => product.ShipmentId == id).ToListAsync();
        ViewBag.id = id;
        return View("Index", _products);
    }

    [HttpPost("/Shipment/{id}/Product")]
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
                            product.ShipmentId = id;
                            dbContext.Products.Add(product);
                            for (int i = 0; i < product.Quantity; i++)
                            {
                                var _productDetail = new ProductDetails()
                                {
                                    Product = product,
                                    Key = "qrcode",
                                    Value = Guid.NewGuid().ToString(),
                                };
                                dbContext.ProductDetails.Add(_productDetail);
                            }
                        }

                        await dbContext.SaveChangesAsync();
                        var _model = await dbContext.Products.Join(dbContext.ProductDetails,
                            product => product.Id,
                            productDetail => productDetail.ProductId,
                            (product, productDetail) => new { product, productDetail })
                                .Where(t => t.product.ShipmentId == id)
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
                                    string directory = Path.Combine("storage", "qrcode", $"{_item.product.ShipmentId}", _item.product.Code);
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
                    logger.LogError(ex, "Error when save to db");
                }
                fileInfo.Delete();
            }
            return Ok("Files uploaded successfully!");
        }
        return BadRequest("No files selected.");
    }
    [HttpGet("/Product/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var _product = await dbContext.Products.FindAsync(id);
        return View("Details", _product);
    }

    [HttpPut("/Product/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var _productDetail = await dbContext.ProductDetails.FindAsync(id);
        if (_productDetail == null)
        {
            return BadRequest($"Id {id} is not valid.");
        }
        var _product = await dbContext.Products.FindAsync(_productDetail.ProductId);
        _product.Delivery++;
        dbContext.Products.Update(_product);
        await dbContext.SaveChangesAsync();
        return Ok("Scanned");
    }

    [HttpGet("/Shipment/{id}/Product/Scan")]
    public async Task<IActionResult> Scan(int id)
    {
        var _products = await dbContext.Products.Where(product => product.ShipmentId == id).ToListAsync();
        ViewBag.id = id;
        return View("Scan", _products);
    }

    [HttpGet("/Shipment/{id}")]
    public async Task<IActionResult> QrCode(int id)
    {
        var model = await dbContext.Products.Join(dbContext.ProductDetails,
        product => product.Id,
        productDetail => productDetail.ProductId,
        (product, productDetail) => new { product, productDetail }).Where(model => model.product.ShipmentId == id && model.productDetail.Key == "qrcode").ToListAsync();
        var products = model.Select(item => new ProductDto(item.productDetail.Id, item.product.Code, item.product.Name, $"/QrCode/{item.product.ShipmentId}/{item.product.Code}/{item.productDetail.Value}.png"));
        ViewBag.id = id;
        return View("QrCode", products);
    }
}