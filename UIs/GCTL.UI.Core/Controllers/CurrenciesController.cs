using GCTL.Data.Models;
using GCTL.Service.Common;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using GCTL.Core.Helpers;
using GCTL.Service.Currencies;
using GCTL.UI.Core.ViewModels.Currencies;
using GCTL.Core.ViewModels.Currencies;

namespace GCTL.UI.Core.Controllers
{
    public class CurrenciesController : BaseController
    {
        private readonly ICurrencyService currencyService;
        private readonly ICommonService commonService;
        private readonly IMapper mapper;
        string strMaxNO = "";
        public CurrenciesController(ICurrencyService currencyService,
                                    ICommonService commonService,
                                    IMapper mapper)
        {
            this.currencyService = currencyService;
            this.commonService = commonService;
            this.mapper = mapper;
        }

        public IActionResult Index()
        {
            CurrencyPageViewModel model = new CurrencyPageViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };
            commonService.FindMaxNo(ref strMaxNO, "CurrencyId", "CA_Def_Currency", 3);
            model.Setup = new CurrencySetupViewModel
            {
                CurrencyId = strMaxNO
            };
            return View(model);
        }


        public IActionResult Setup(string id)
        {
            CurrencySetupViewModel model = new CurrencySetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "CurrencyId", "CA_Def_Currency", 3);
            var result = currencyService.GetCurrency(id);
            if (result != null)
            {
                model = mapper.Map<CurrencySetupViewModel>(result);
                model.Code = id;
            }
            else
            {
                model.CurrencyId = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Setup(CurrencySetupViewModel model)
        {
            if (currencyService.IsCurrencyExist(model.CurrencyName, model.CurrencyId))
            {
                return Json(new { isSuccess = false, message = "Already Exists" });
            }

            if (ModelState.IsValid)
            {
                CaDefCurrency currency = currencyService.GetCurrency(model.CurrencyId) ?? new CaDefCurrency();
                model.ToAudit(LoginInfo, model.Tc > 0);
                mapper.Map(model, currency);
                currencyService.SaveCurrency(currency);
                return Json(new { isSuccess = true, message = "Saved Successfully" });
            }

            return Json(new { success = false, message = ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage });
        }

        public ActionResult Grid()
        {
            var resutl = currencyService.GetCurrencies();
            return Json(new { data = resutl });
        }


        [HttpPost]
        public ActionResult Delete(string id)
        {
            bool success = false;
            foreach (var item in id.Split(",", StringSplitOptions.RemoveEmptyEntries))
            {
                success = currencyService.DeleteCurrency(item);
            }

            return Json(new { success = success, message = "Deleted Successfully" });
        }


        [HttpPost]
        public JsonResult CheckAvailability(string name, string code)
        {
            if (currencyService.IsCurrencyExist(name, code))
            {
                return Json(new { isSuccess = true, message = "Already Exists" });
            }

            return Json(new { isSuccess = false });
        }
    }
}