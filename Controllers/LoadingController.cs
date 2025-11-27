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

public class LoadingController : Controller
{
    private readonly IConfiguration configuration;
    private readonly ILogger<LoadingController> logger;
    private readonly ApplicationDbContext dbContext;

    public LoadingController(IConfiguration configuration, ILogger<LoadingController> logger, ApplicationDbContext dbContext)
    {
        this.configuration = configuration;
        this.logger = logger;
        this.dbContext = dbContext;
    }

    [HttpGet]
    [Authorize(Roles = "Loading")]
    public async Task<IActionResult> Index(int? id)
    {
        if (!id.HasValue)
        {
            var _pendingShipment = await dbContext.Shipments
            .Where(s => s.Status == ShipmentStatus.PENDING.Code)
            .OrderBy(s => s.Id)
            .FirstOrDefaultAsync();
            if (_pendingShipment == null)
            {
                return RedirectToAction("Index", "Shipment");
            }
            return RedirectToAction("Index", new { id = _pendingShipment.Id });
        }

        var _products = await dbContext.ShipmentProducts.Join(dbContext.Products,
         t1 => t1.ProductId,
         t2 => t2.Id,
         (t1, t2) => new { ShipmentProduct = t1, Product = t2 })
         .Where(m => m.ShipmentProduct.ShipmentId == id)
         .Select(m => new ShipmentProductDto(
             m.Product.Id,
             m.Product.Category,
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

    [HttpPost]
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
        var _productDetail = await dbContext.ProductDetails
        .FirstOrDefaultAsync(x => x.ShortId == message);
        if (_productDetail == null)
        {
            logger.LogError($"Product {message} not found");
            ModelState.AddModelError("message", $"Product {message} not found");
            return UnprocessableEntity(ModelState);
        }
        var productStatus = new ProductStatus(_productDetail.Status);
        var shipmentStatus = new ShipmentStatus(_shipment.Status);
        if (shipmentStatus != ShipmentStatus.PENDING)
        {
            ModelState.AddModelError("message", $"Shipment {id} should be in PENDING status");
            return UnprocessableEntity(ModelState);
        }
        var _product = await dbContext.Products.FindAsync(_productDetail.ProductId);
        var _shipmentProduct = await dbContext.ShipmentProducts
        .FirstOrDefaultAsync(sp => sp.ProductId == _product.Id && sp.ShipmentId == id);
        if (productStatus == ProductStatus.AVAILABLE)
        {
            _productDetail.Status = ProductStatus.LOADED.Code;
            if (_shipmentProduct == null)
            {
                _shipmentProduct = new ShipmentProduct()
                {
                    ShipmentId = id,
                    ProductId = _product.Id,
                    Loaded = 1,
                    Unloaded = 0
                };
                dbContext.ShipmentProducts.Add(_shipmentProduct);
            }
            else
            {
                _shipmentProduct.Loaded = _shipmentProduct.Loaded + 1;
                _shipmentProduct.UpdatedAt = DateTime.Now;
                dbContext.ShipmentProducts.Update(_shipmentProduct);
            }
        }
        _productDetail.UpdatedAt = DateTime.Now;
        dbContext.ProductDetails.Update(_productDetail);
        _product.UpdatedAt = DateTime.Now;
        dbContext.Products.Update(_product);
        await dbContext.SaveChangesAsync();

        return Ok(JsonConvert.SerializeObject(new
        {
            _product.Id,
            _product.Category,
            _product.Name,
            _shipmentProduct.Loaded,
            _shipmentProduct.Unloaded,
            CreatedAt = _shipmentProduct.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdatedAt = _shipmentProduct.UpdatedAt.HasValue ? _shipmentProduct.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null
        }));
    }
    [HttpGet]
    [Authorize(Roles = "Loading")]
    public IActionResult Scan(int id)
    {
        return View("Scan", id);
    }

    [HttpPost]
    [Authorize(Policy = "Loading")]
    public async Task<IActionResult> Confirm(int id)
    {
        var _shipment = await dbContext.Shipments.FindAsync(id);
        if (_shipment == null)
        {
            ModelState.AddModelError("id", "Shipment not found");
            return UnprocessableEntity(ModelState);
        }
        var shipmentStatus = new ShipmentStatus(_shipment.Status);
        if (shipmentStatus != ShipmentStatus.PENDING)
        {
            ModelState.AddModelError("id", "Shipment should be in PENDING status");
            return UnprocessableEntity(ModelState);
        }
        _shipment.Status = ShipmentStatus.IN_TRANSIT.Code;
        dbContext.Shipments.Update(_shipment);
        await dbContext.SaveChangesAsync();
        return Ok("Shipment status updated to In Transit");
    }
}