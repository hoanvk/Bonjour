using Bonjour.Models;
using Bonjour.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Bonjour.Controllers;

[Authorize(Policy = "Admin")]
public class RoleController : Controller
{
    private readonly ApplicationDbContext dbContext;
    private readonly ILogger<RoleController> logger;
    public RoleController(ApplicationDbContext dbContext, ILogger<RoleController> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var _roles = await dbContext.Roles.ToListAsync();
        var _permissions = await dbContext.Permissions.ToListAsync();
        ViewBag.Permissions = _permissions;
        return View(_roles);
    }
    public async Task<IActionResult> Edit(int id)
    {
        var _roleHasPermissions = await dbContext.RoleHasPermissions.Where(m => m.RoleId == id).ToListAsync();
        return Ok(JsonConvert.SerializeObject(_roleHasPermissions));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateRoleRequest role)
    {
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }
        if (dbContext.Roles.Any(r => r.Name == role.Name))
        {
            ModelState.AddModelError(nameof(CreateRoleRequest.Name), "Role name is exists");
            return UnprocessableEntity(ModelState);
        }
        var _role = new Role()
        {
            Name = role.Name
        };
        dbContext.Roles.Add(_role);
        var roleHasPermissions = JsonConvert.DeserializeObject<CreateRoleHasPermissionRequest[]>(role.Permissions);
        foreach (var roleHasPermission in roleHasPermissions)
        {
            var _roleHasPermission = new RoleHasPermission();
            _roleHasPermission.Role = _role;
            _roleHasPermission.PermissionId = roleHasPermission.PermissionId;
            _roleHasPermission.Action = roleHasPermission.Action;
            dbContext.RoleHasPermissions.Add(_roleHasPermission);
        }
        await dbContext.SaveChangesAsync();
        return Ok(JsonConvert.SerializeObject(_role));
    }
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var _role = await dbContext.Roles.FindAsync(id);
        if (_role == null)
        {
            ModelState.AddModelError("Id", "Role id not found");
            return UnprocessableEntity(ModelState);
        }
        var _userHasRoles = await dbContext.UserHasRoles.Where(m => m.RoleId == id).ToListAsync();
        if (_userHasRoles != null && _userHasRoles.Any())
        {
            dbContext.UserHasRoles.RemoveRange(_userHasRoles);
        }
        var _roleHasPermissions = await dbContext.RoleHasPermissions.Where(m => m.RoleId == id).ToListAsync();
        if (_roleHasPermissions != null && _roleHasPermissions.Any())
        {
            dbContext.RoleHasPermissions.RemoveRange(_roleHasPermissions);
        }
        dbContext.Roles.Remove(_role);
        await dbContext.SaveChangesAsync();
        return Ok("Deleted successfully");
    }
}