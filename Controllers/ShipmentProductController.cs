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
            m.Product.Weight.GetValueOrDefault(0),
            m.ShipmentProduct.CreatedAt,
            m.ShipmentProduct.UpdatedAt
        )).ToListAsync();
        ViewBag.ShipmentId = id;
        return View("Index", _products);
    }

    [HttpGet("/Shipment/{id}/Product/Export")]
    public async Task<IActionResult> Export(int id)
    {
        logger.LogInformation("Export shipment products");
        var fileContent = await mediator.Send(new ExportShipmentProductRequest(id));
        var fileName = $"shipment_{id}_products.xlsx";
        return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}