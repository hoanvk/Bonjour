using Bonjour.Domain.Helpers;
using Bonjour.Domain.Products;
using Microsoft.AspNetCore.Mvc;

namespace Bonjour.ViewComponents;

public class ProductStatusViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(string status)
    {
        return View(new ProductStatus(status));
    }
}