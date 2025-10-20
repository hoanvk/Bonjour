using Bonjour.Dtos;
using Bonjour.Models;
using Bonjour.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Bonjour.Controllers;

[Authorize]
public class ContractController : Controller
{
    private readonly ILogger<ContractController> logger;
    private readonly ApplicationDbContext dbContext;

    public ContractController(ILogger<ContractController> logger, ApplicationDbContext dbContext)
    {
        this.logger = logger;
        this.dbContext = dbContext;
    }
    public async Task<IActionResult> Index()
    {
        var contracts = await dbContext.Contracts.ToListAsync();
        return View(contracts);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateContractRequest contract)
    {
        logger.LogInformation("Create contract");
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }
        var _contract = new Contract()
        {
            Name = contract.Name,
            Customer = contract.Customer,
            StartDate = DateTime.Parse(contract.StartDate),
            EndDate = DateTime.Parse(contract.EndDate)
        };
        dbContext.Contracts.Add(_contract);
        await dbContext.SaveChangesAsync();
        return Ok("Created contract successfully");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CreateContractRequest contract)
    {
        logger.LogInformation("Edit contract");
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }
        var _contract = await dbContext.Contracts.FindAsync(id);
        if (_contract == null)
        {
            ModelState.AddModelError("Id", "Contract not found");
            return UnprocessableEntity(ModelState);
        }
        _contract.Name = contract.Name;
        _contract.Customer = contract.Customer;
        _contract.StartDate = DateTime.Parse(contract.StartDate);
        _contract.EndDate = DateTime.Parse(contract.EndDate);
        dbContext.Contracts.Update(_contract);
        await dbContext.SaveChangesAsync();
        return Ok("Edited contract successfully");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var _contract = await dbContext.Contracts.FindAsync(id);
        if (_contract == null)
        {
            return BadRequest("Contract not found");
        }
        var _productDetails = await dbContext.ProductDetails
            .Where(p => dbContext.Products
                .Any(q => q.ContractId == id && p.ProductId == q.Id)).ToListAsync();
        if (_productDetails != null && _productDetails.Any())
        {
            dbContext.ProductDetails.RemoveRange(_productDetails);
        }

        var _products = await dbContext.Products.Where(p => p.ContractId == id).ToListAsync();
        if (_products != null && _products.Any())
        {
            dbContext.Products.RemoveRange(_products);
        }
        dbContext.Contracts.Remove(_contract);
        await dbContext.SaveChangesAsync();
        return Ok("Removed contract successfully");
    }

}