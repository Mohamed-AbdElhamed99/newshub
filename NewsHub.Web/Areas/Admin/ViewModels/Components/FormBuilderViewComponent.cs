using Microsoft.AspNetCore.Mvc;

namespace NewsHub.Web.Areas.Admin.ViewModels.Components;

public class FormBuilderViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(FormViewModel model)
    {
        return View(model);
    }
}