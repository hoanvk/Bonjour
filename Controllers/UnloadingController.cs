using Bonjour.Domain.Products;
using Bonjour.Domain.Shipments;
using Bonjour.Models;
using Bonjour.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Bonjour.Controllers;

public class UnloadingController : Controller
{
    private readonly IConfiguration configuration;
    private readonly ILogger<UnloadingController> logger;
    private readonly ApplicationDbContext dbContext;

    public UnloadingController(IConfiguration configuration, ILogger<UnloadingController> logger, ApplicationDbContext dbContext)
    {
        this.configuration = configuration;
        this.logger = logger;
        this.dbContext = dbContext;
    }

    [HttpPost("/Unloading/{id}/Product/Create")]
    public async Task<IActionResult> Create(int id, [FromBody] DeliveryRequest request)
    {

        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }
        var _shipment = await dbContext.Shipments.FindAsync(id);
        if (_shipment == null)
        {
            ModelState.AddModelError("id", $"Shipment {id} not found");
            return UnprocessableEntity(ModelState);
        }
        string message = request.message;
        var _productDetail = await dbContext.ProductDetails.FirstOrDefaultAsync(x => x.ShortId == message);
        if (_productDetail == null)
        {
            logger.LogError($"Product {message} not found");
            ModelState.AddModelError("message", $"Product {message} not found");
            return UnprocessableEntity(ModelState);
        }

        var productStatus = new ProductStatus(_productDetail.Status);
        var shipmentStatus = new ShipmentStatus(_shipment.Status);
        if (shipmentStatus != ShipmentStatus.IN_TRANSIT)
        {
            ModelState.AddModelError("message", $"Shipment {id} should be in IN_TRANSIT status");
            return UnprocessableEntity(ModelState);
        }
        var _product = await dbContext.Products.FindAsync(_productDetail.ProductId);
        var _shipmentProduct = await dbContext.ShipmentProducts.FirstOrDefaultAsync(sp => sp.ProductId == _product.Id && sp.ShipmentId == id);
        if (productStatus == ProductStatus.LOADED)
        {
            _productDetail.Status = ProductStatus.UNLOADED.Code;
            _shipmentProduct.Unloaded = _shipmentProduct.Unloaded + 1;
            _shipmentProduct.UpdatedAt = DateTime.Now;
            dbContext.ShipmentProducts.Update(_shipmentProduct);
        }
        _productDetail.UpdatedAt = DateTime.Now;
        dbContext.ProductDetails.Update(_productDetail);
        _product.UpdatedAt = DateTime.Now;
        dbContext.Products.Update(_product);
        await dbContext.SaveChangesAsync();

        return Ok(JsonConvert.SerializeObject(new
        {
            _product.Id,
            _product.Code,
            _product.Name,
            _shipmentProduct.Loaded,
            _shipmentProduct.Unloaded,
            CreatedAt = _shipmentProduct.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdatedAt = _shipmentProduct.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss")
        }));
    }

    [HttpGet("/Unloading/{id}/Product/Scan")]
    [Authorize(Roles = "Unloading")]
    public IActionResult Scan(int? id)
    {
        int shipmentId = 0;
        if (!id.HasValue)
        {
            var _pendingShipment = dbContext.Shipments.Where(s => s.Status == ShipmentStatus.IN_TRANSIT.Code).OrderBy(s => s.Id).FirstOrDefault();
            if (_pendingShipment == null)
            {
                return RedirectToAction("Index", "Shipment");
            }
            shipmentId = _pendingShipment.Id;
        }
        else
        {
            shipmentId = id.Value;
        }
        return View("Scan", shipmentId);
    }
}