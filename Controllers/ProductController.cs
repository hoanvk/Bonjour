using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Bonjour.Models;
using OfficeOpenXml;
using QRCoder;
using Microsoft.EntityFrameworkCore;

namespace Bonjour.Controllers;

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

    public async Task<IActionResult> Index(int id)
    {
        var _products = await dbContext.Products.Where(product => product.ShipmentId == id).ToListAsync();
        ViewBag.id = id;
        return View(_products);
    }

    [HttpPost]
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

                        }

                        await dbContext.SaveChangesAsync();
                        var _products = await dbContext.Products.Where(product => product.ShipmentId == id).ToListAsync();
                        using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                        {
                            foreach (var _product in _products)
                            {
                                // Create QR code data with the desired error correction level (e.g., ECCLevel.Q for 25% error correction)
                                QRCodeData qrCodeData = qrGenerator.CreateQrCode($"{configuration["QrCodeBaseUrl"]}{Url.Action("Details", new { id = _product.Id })}", QRCodeGenerator.ECCLevel.Q);

                                // Create a PNG byte array from the QR code data
                                using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
                                {
                                    byte[] qrCodeImageBytes = qrCode.GetGraphic(10);

                                    // Save the byte array as a PNG file
                                    await System.IO.File.WriteAllBytesAsync(Path.Combine("storage", "qrcode", $"{_product.Code}_{_product.Id}.png"), qrCodeImageBytes);
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
    [HttpGet("/Product/{id}/Details")]
    public async Task<IActionResult> Details(int id)
    {
        var _product = await dbContext.Products.FindAsync(id);
        return View("Details", _product);
    }
}