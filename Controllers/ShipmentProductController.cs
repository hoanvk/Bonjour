using Microsoft.AspNetCore.Mvc;
using Bonjour.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Bonjour.Domain.Shipments;
using MediatR;

namespace Bonjour.Controllers;

[Authorize]
public class ShipmentProductController : Controller
{
    private readonly IConfiguration configuration;
    private readonly ILogger<ShipmentProductController> logger;
    private readonly ApplicationDbContext dbContext;
    private IMediator mediator;

    public ShipmentProductController(IConfiguration configuration, ILogger<ShipmentProductController> logger, ApplicationDbContext dbContext, IMediator mediator)
    {
        this.configuration = configuration;
        this.logger = logger;
        this.dbContext = dbContext;
        this.mediator = mediator;
    }

    [HttpGet("/Shipment/{id}/Product")]
    public async Task<IActionResult> Index(int id)
    {
        var _products = await dbContext.ShipmentProducts.Join(dbContext.Products,
        t1 => t1.ProductId,
        t2 => t2.Id,
        (t1, t2) => new { ShipmentProduct = t1, Product = t2 }).Where(m => m.ShipmentProduct.ShipmentId == id).Select(m => new ShipmentProductDto(
            m.ShipmentProduct.Id,
            m.Product.Code,
            m.Product.Name,
            m.ShipmentProduct.Loaded,
            m.ShipmentProduct.Unloaded,
            m.ShipmentProduct.CreatedAt,
            m.ShipmentProduct.UpdatedAt
        )).ToListAsync();
        ViewBag.ShipmentId = id;
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
                await mediator.Send(new ImportShipmentProductRequest(id, filePath));
            }
            return Ok("Files uploaded successfully!");
        }
        return BadRequest("No files selected.");
    }

    [HttpPost("/Shipment/{id}/Loaded/Confirm")]
    [Authorize(Policy = "Loading")]
    public async Task<IActionResult> ConfirmLoaded(int id)
    {
        var _shipment = await dbContext.Shipments.FindAsync(id);
        if (_shipment == null)
        {
            ModelState.AddModelError("id", "Shipment not found");
            return UnprocessableEntity(ModelState);
        }
        _shipment.Status = ShipmentStatus.IN_TRANSIT.Code;
        dbContext.Shipments.Update(_shipment);
        await dbContext.SaveChangesAsync();
        return Ok("Shipment status updated to In Transit");
    }

    [HttpPost("/Shipment/{id}/Unloaded/Confirm")]
    [Authorize(Policy = "Unloading")]
    public async Task<IActionResult> ConfirmUnloaded(int id)
    {
        var _shipment = await dbContext.Shipments.FindAsync(id);
        if (_shipment == null)
        {
            ModelState.AddModelError("id", "Shipment not found");
            return UnprocessableEntity(ModelState);
        }
        _shipment.Status = ShipmentStatus.DELIVERED.Code;
        dbContext.Shipments.Update(_shipment);
        await dbContext.SaveChangesAsync();
        return Ok("Shipment status updated to Delivered");
    }
}