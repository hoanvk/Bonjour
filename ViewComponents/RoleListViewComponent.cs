using Bonjour.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bonjour.ViewComponents;

public class RoleListViewComponent : ViewComponent
{
    private readonly ApplicationDbContext dbContext;

    public RoleListViewComponent(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var _roles = await dbContext.Roles.ToListAsync();
        return View(_roles);
    }
}