using Bonjour.Domain.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Bonjour.ViewComponents;

public class DateTimeDisplayViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(DateTime? datetime)
    {
        return View(new LocalDateTime(datetime));
    }
}