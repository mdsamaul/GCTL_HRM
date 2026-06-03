using GCTL.Core.Reports;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class PreviewController : Controller
    {
        public IActionResult Viewer(string name)
        {
            PreviewModel model = new PreviewModel();
            model.Name = name;
            return View(model);
        }
    }
}