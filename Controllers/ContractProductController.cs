using Microsoft.AspNetCore.Mvc;
using Bonjour.Models;
using OfficeOpenXml;
using QRCoder;
using Microsoft.EntityFrameworkCore;
using Bonjour.Dtos;
using Microsoft.AspNetCore.Authorization;
using Bonjour.Domain.Products;
using Bonjour.Domain.Helpers;
using Bonjour.Domain.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Bonjour.Controllers;

[Authorize]
public class ContractProductController : Controller
{
    private readonly IConfiguration configuration;
    private readonly ILogger<ContractProductController> logger;
    private readonly ApplicationDbContext dbContext;
    private IMediator mediator;

    public ContractProductController(IConfiguration configuration, ILogger<ContractProductController> logger, ApplicationDbContext dbContext, IMediator mediator)
    {
        this.configuration = configuration;
        this.logger = logger;
        this.dbContext = dbContext;
        this.mediator = mediator;
    }
    [HttpGet]
    public async Task<IActionResult> Index(int id)
    {
        var _products = await dbContext.Products.Where(product => product.ContractId == id).ToListAsync();
        ViewBag.ContractId = id;
        return View("Index", _products);
    }

    [HttpPost]
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
                await mediator.Send(new ImportContractProductRequest(id, filePath));
            }
            return Ok("Files uploaded successfully!");
        }
        ModelState.AddModelError("files", "No files selected");
        return UnprocessableEntity(ModelState);
    }

    [HttpGet]
    public async Task<IActionResult> Print(int id)
    {
        var model = await dbContext.Products.Join(dbContext.ProductDetails,
        product => product.Id,
        productDetail => productDetail.ProductId,
        (product, productDetail) => new { product, productDetail }).Where(model => model.product.ContractId == id).ToListAsync();
        var products = model.Select(item => new ProductDto(item.productDetail.Id, $"{item.product.Code}-{item.productDetail.SequenceNo}", item.product.Name, $"/QrCode/{item.product.ContractId}/{item.product.Code}/{item.productDetail.ShortId}.png"));
        ViewBag.ContractId = id;
        return View(products);
    }
}