using Bonjour.Dtos;
using Bonjour.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bonjour.Controllers;

[Authorize]
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
        var _productDetails = await dbContext.ProductDetails
            .Where(p => dbContext.Products
                .Any(q => q.ShipmentId == id && p.ProductId == q.Id)).ToListAsync();
        dbContext.ProductDetails.RemoveRange(_productDetails);
        var _products = await dbContext.Products.Where(p => p.ShipmentId == id).ToListAsync();
        dbContext.Products.RemoveRange(_products);
        dbContext.Shipments.Remove(_shipment);
        await dbContext.SaveChangesAsync();
        return Ok("Removed from db");
    }

}