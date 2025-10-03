using Bonjour.Dtos;
using Bonjour.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bonjour.Controllers;

public class ShipmentController : Controller
{
    private readonly IConfiguration configuration;
    private readonly ILogger<ShipmentController> logger;
    private readonly ApplicationDbContext dbContext;

    public ShipmentController(IConfiguration configuration, ILogger<ShipmentController> logger, ApplicationDbContext dbContext)
    {
        this.configuration = configuration;
        this.logger = logger;
        this.dbContext = dbContext;
    }
    public async Task<IActionResult> Index()
    {
        var shipments = await dbContext.Shipments.ToListAsync();
        return View(shipments);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Shipment shipment)
    {
        logger.LogInformation("Create shipment");
        dbContext.Shipments.Add(shipment);
        await dbContext.SaveChangesAsync();
        return Ok("Saved to db");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var _shipment = await dbContext.Shipments.FirstOrDefaultAsync();
        if (_shipment == null)
        {
            return BadRequest("Shipment not found");
        }
        var _products = await dbContext.Products.Where(p => p.ShipmentId == id).ToListAsync();
        dbContext.Products.RemoveRange(_products);
        dbContext.Shipments.Remove(_shipment);
        await dbContext.SaveChangesAsync();
        return Ok("Removed from db");
    }

    [HttpGet("/Shipment/{id}/QrCode")]
    public async Task<IActionResult> QrCode(int id)
    {
        var _products = await dbContext.Products.Where(product => product.ShipmentId == id).ToListAsync();
        var products = _products.Select(_product => new ProductDto(_product.Id, _product.Code, _product.Name, $"/QrCode/{_product.Code}_{_product.Id}.png"));
        return View("QrCode", products);
    }
}