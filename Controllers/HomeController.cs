using System.Diagnostics;
using Bonjour;
using Microsoft.AspNetCore.Mvc;
using Bonjour.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Localization;

namespace Bonjour.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> logger;
    private readonly ApplicationDbContext dbContext;
    private readonly IStringLocalizer<SharedResources> _localizer;
    public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext, IStringLocalizer<SharedResources> localizer)
    {
        this.logger = logger;
        this.dbContext = dbContext;
        _localizer = localizer;
    }

    public IActionResult Index()
    {
        logger.LogInformation(_localizer["Dashboard"]);
        return View();
    }

    public IActionResult Loading()
    {
        return View();
    }

    public IActionResult Unloading()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        logger.LogWarning(exceptionHandlerPathFeature.Error.Message);
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
