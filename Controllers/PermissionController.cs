using Bonjour.Models;
using Bonjour.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Bonjour.Controllers;

public class PermissionController : Controller
{
    private readonly IConfiguration configuration;
    private readonly ILogger<PermissionController> logger;
    private readonly ApplicationDbContext dbContext;

    public PermissionController(IConfiguration configuration, ILogger<PermissionController> logger, ApplicationDbContext dbContext)
    {
        this.configuration = configuration;
        this.logger = logger;
        this.dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        var _permissions = await dbContext.Permissions.ToListAsync();
        return View(_permissions);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePermissionRequest permission)
    {
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }
        var _permission = new Permission();
        _permission.Name = permission.Name;
        dbContext.Permissions.Add(_permission);
        await dbContext.SaveChangesAsync();
        return Ok("Created permission successfully!");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var _permission = await dbContext.Permissions.FindAsync(id);
        if (_permission == null)
        {
            ModelState.AddModelError("Id", "Permission not found");
            return UnprocessableEntity(ModelState);
        }
        dbContext.Permissions.Remove(_permission);
        await dbContext.SaveChangesAsync();
        return Ok("Deleted permission successfully!");
    }
}