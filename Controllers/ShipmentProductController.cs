using System.Diagnostics;
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
public class ShipmentProductController : Controller
{
    private readonly IConfiguration configuration;
    private readonly ILogger<ShipmentProductController> logger;
    private readonly ApplicationDbContext dbContext;

    public ShipmentProductController(IConfiguration configuration, ILogger<ShipmentProductController> logger, ApplicationDbContext dbContext)
    {
        this.configuration = configuration;
        this.logger = logger;
        this.dbContext = dbContext;
    }

    [HttpGet("/Shipment/{id}/Product")]
    public async Task<IActionResult> Index(int id)
    {
        var _products = await dbContext.ShipmentProducts.Where(product => product.ShipmentId == id).ToListAsync();
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

                        var _shipmentProducts = await dbContext.ShipmentProducts.Where(p => p.ShipmentId == id).ToListAsync();
                        if (_shipmentProducts != null && _shipmentProducts.Any())
                        {
                            dbContext.ShipmentProducts.RemoveRange(_shipmentProducts);
                        }
                        // Loop through rows (assuming first row is header)
                        for (int rowNum = startRow + 1; rowNum <= endRow; rowNum++)
                        {
                            var _product = await dbContext.Products.FirstOrDefaultAsync(m => m.Code == worksheet.Cells[rowNum, startCol].Text);
                            if (_product == null)
                            {
                                return UnprocessableEntity("Product not found");
                            }
                            var _shipmentProduct = new ShipmentProduct();
                            // Assuming columns are in a specific order: Name, Age, City
                            _shipmentProduct.Loading = int.Parse(worksheet.Cells[rowNum, startCol + 1].Text);
                            _shipmentProduct.Unloading = int.Parse(worksheet.Cells[rowNum, startCol + 2].Text);
                            _shipmentProduct.ShipmentId = id;
                            dbContext.ShipmentProducts.Update(_shipmentProduct);
                        }
                        await dbContext.SaveChangesAsync();

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
}