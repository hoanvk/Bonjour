using Bonjour.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Bonjour.Lib.Services;

public class RealTimeHub : Hub
{
    private readonly ILogger<RealTimeHub> logger;
    private readonly ApplicationDbContext dbContext;

    public RealTimeHub(ILogger<RealTimeHub> logger, ApplicationDbContext dbContext)
    {
        this.logger = logger;
        this.dbContext = dbContext;
    }

    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public async Task SendNotification(string message)
    {
        logger.LogInformation(message);
        Product _product = new Product()
        {
            Id = 0,
            Delivery = 0
        };
        var _productDetail = await dbContext.ProductDetails.FirstOrDefaultAsync(x => x.Value == message && x.Key == "qrcode");
        if (_productDetail == null)
        {
            logger.LogError($"Product {message} not found");
            return;
        }
        var _qrcodeScanned = await dbContext.ProductDetails.FirstOrDefaultAsync(x => x.ProductId == _productDetail.ProductId
                && x.Key == "qrcode_scanned" && x.Value == _productDetail.Value);
        if (_qrcodeScanned != null)
        {
            logger.LogError($"Product {message} was scanned");
            return;
        }
        dbContext.ProductDetails.Add(new ProductDetails()
        {
            ProductId = _productDetail.ProductId,
            Key = "qrcode_scanned",
            Value = _productDetail.Value
        });
        _product = await dbContext.Products.FindAsync(_productDetail.ProductId);
        _product.Delivery++;
        dbContext.Products.Update(_product);
        await dbContext.SaveChangesAsync();
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        };

        await Clients.All.SendAsync("ReceiveNotification",
            JsonConvert.SerializeObject(new { _product.Id, _product.Quantity, _product.Delivery }, settings));
    }
}