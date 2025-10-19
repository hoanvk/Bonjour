using Bonjour.Dtos;
using Bonjour.Models;
using Bonjour.Requests;
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
    public async Task<IActionResult> Create(CreateShipmentRequest shipment)
    {
        logger.LogInformation("Create shipment");
        var _shipment = new Shipment()
        {
            Carrier = shipment.Carrier,
            Consignee = shipment.Consignee,
            Departure = shipment.Departure,
            Status = shipment.Status
        };
        dbContext.Shipments.Add(_shipment);
        await dbContext.SaveChangesAsync();
        return Ok("Saved to db");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CreateShipmentRequest shipment)
    {
        logger.LogInformation("Edit shipment");
        var _shipment = await dbContext.Shipments.FindAsync(id);
        _shipment.Carrier = shipment.Carrier;
        _shipment.Consignee = shipment.Consignee;
        _shipment.Departure = shipment.Departure;
        _shipment.Status = shipment.Status;
        dbContext.Shipments.Update(_shipment);
        await dbContext.SaveChangesAsync();
        return Ok("Saved to db");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var _shipment = await dbContext.Shipments.FindAsync(id);
        if (_shipment == null)
        {
            return BadRequest("Shipment not found");
        }
        var _shipmentProducts = await dbContext.ShipmentProducts.Where(p => p.ShipmentId == id).ToListAsync();
        if (_shipmentProducts != null && _shipmentProducts.Any())
        {
            dbContext.ShipmentProducts.RemoveRange(_shipmentProducts);
        }
        dbContext.Shipments.Remove(_shipment);
        await dbContext.SaveChangesAsync();
        return Ok("Removed from db");
    }

}