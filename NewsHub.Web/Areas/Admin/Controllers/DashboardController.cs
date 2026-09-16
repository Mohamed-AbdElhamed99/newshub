using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using NewsHub.Web.Resources;
using System.Reflection;
using Microsoft.Extensions.Localization;
namespace NewsHub.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly IStringLocalizer<SharedResource> _localizer;

    public DashboardController(IStringLocalizer<SharedResource> localizer)
    {
        _localizer = localizer;
    }
    
    public IActionResult Index()
    {
        var localizer = HttpContext.RequestServices
            .GetRequiredService<IStringLocalizer<SharedResource>>();

        Console.WriteLine($"Assembly: {typeof(SharedResource).Assembly.FullName}");
        Console.WriteLine($"BaseName: {localizer.GetType().FullName}");

        var value = localizer["Dashboard"];

        Console.WriteLine($"Value: {value.Value}");
        Console.WriteLine($"ResourceNotFound: {value.ResourceNotFound}");
        
        var assembly = typeof(SharedResource).Assembly;

        Console.WriteLine($"Assembly Name: {assembly.GetName().Name}");
        Console.WriteLine($"Resource Names:");

        foreach (var resource in assembly.GetManifestResourceNames())
        {
            Console.WriteLine(resource);
        }
        
        return View();
    }
}