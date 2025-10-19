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
public class ProductController : Controller
{
    private readonly IConfiguration configuration;
    private readonly ILogger<HomeController> logger;
    private readonly ApplicationDbContext dbContext;

    public ProductController(IConfiguration configuration, ILogger<HomeController> logger, ApplicationDbContext dbContext)
    {
        this.configuration = configuration;
        this.logger = logger;
        this.dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var _products = await dbContext.Products.ToListAsync();
        return View("Index", _products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var _product = await dbContext.Products.FindAsync(id);
        return View("Details", _product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var _productDetail = await dbContext.ProductDetails.FindAsync(id);
        if (_productDetail == null)
        {
            return BadRequest($"Id {id} is not valid.");
        }
        var _product = await dbContext.Products.FindAsync(_productDetail.ProductId);
        _product.Delivery++;
        dbContext.Products.Update(_product);
        await dbContext.SaveChangesAsync();
        return Ok("Scanned");
    }
}