using Bonjour.Models;
using Bonjour.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Bonjour.Controllers;

public class DeliveryController : Controller
{
    private readonly IConfiguration configuration;
    private readonly ILogger<DeliveryController> logger;
    private readonly ApplicationDbContext dbContext;

    public DeliveryController(IConfiguration configuration, ILogger<DeliveryController> logger, ApplicationDbContext dbContext)
    {
        this.configuration = configuration;
        this.logger = logger;
        this.dbContext = dbContext;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DeliveryRequest request)
    {
        string message = request.message;
        if (string.IsNullOrEmpty(message))
        {
            return BadRequest("Message is required");
        }

        var _productDetail = await dbContext.ProductDetails.FirstOrDefaultAsync(x => x.Value == message && x.Key == "qrcode");
        if (_productDetail == null)
        {
            logger.LogError($"Product {message} not found");
            return BadRequest($"Product {message} not found");
        }
        var _product = await dbContext.Products.FindAsync(_productDetail.ProductId);
        var _qrcodeScanned = await dbContext.ProductDetails.FirstOrDefaultAsync(x => x.ProductId == _productDetail.ProductId
                && x.Key == "qrcode_scanned" && x.Value == _productDetail.Value);
        if (_qrcodeScanned == null)
        {
            dbContext.ProductDetails.Add(new ProductDetails()
            {
                ProductId = _productDetail.ProductId,
                Key = "qrcode_scanned",
                Value = _productDetail.Value
            });
            _product.Delivery++;
            _product.UpdatedAt = DateTime.Now;
            dbContext.Products.Update(_product);
            await dbContext.SaveChangesAsync();
        }
        return Ok(JsonConvert.SerializeObject(new
        {
            _product.Id,
            _product.Code,
            _product.Name,
            _product.Quantity,
            _product.Delivery,
            UpdatedAt = _product.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss")
        }));
    }
}