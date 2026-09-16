using Microsoft.AspNetCore.Mvc;

namespace NewsHub.Web.Areas.Admin.ViewModels.Components;

public class DataTableViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(TableViewModel model)
    {
        return View(model);
    }
}