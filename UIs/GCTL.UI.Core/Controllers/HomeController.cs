using GCTL.Core.Helpers;
using GCTL.Service.Common;
using GCTL.UI.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace GCTL.UI.Core.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICommonService commonService;

        public HomeController(ILogger<HomeController> logger,
                              ICommonService commonService)
        {
            _logger = logger;
            this.commonService = commonService;
        }

        [Route("Dashboard")]
        public IActionResult Index()
        {
            return View();
        }

        //[Route("")]
        //public IActionResult Login()
        //{
        //    LoginPageViewModel model = new LoginPageViewModel();
        //    return View(model);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Login(LoginPageViewModel model)
        //{

        //    return RedirectToAction(nameof(Index));
        //}

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}