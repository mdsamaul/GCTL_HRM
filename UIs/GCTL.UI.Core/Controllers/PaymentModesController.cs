using GCTL.Core.ViewModels.PaymentModes;
using GCTL.Data.Models;
using GCTL.Service.Common;
using GCTL.Service.PaymentModes;
using GCTL.UI.Core.ViewModels.PaymentModes;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using GCTL.Core.Helpers;

namespace GCTL.UI.Core.Controllers
{
    public class PaymentModesController : BaseController
    {
        private readonly IPaymentModeService paymentModeService;
        private readonly ICommonService commonService;
        private readonly IMapper mapper;
        string strMaxNO = "";
        public PaymentModesController(IPaymentModeService paymentModeService,
                                      ICommonService commonService,
                                      IMapper mapper)
        {
            this.paymentModeService = paymentModeService;
            this.commonService = commonService;
            this.mapper = mapper;
        }

        public IActionResult Index()
        {
            PaymentModePageViewModel model = new PaymentModePageViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };
            commonService.FindMaxNo(ref strMaxNO, "PaymentModeId", "Sales_Def_PaymentMode", 3);
            model.Setup = new PaymentModeSetupViewModel
            {
                PaymentModeId = strMaxNO
            };
            return View(model);
        }


        public IActionResult Setup(string id)
        {
            PaymentModeSetupViewModel model = new PaymentModeSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "PaymentModeId", "Sales_Def_PaymentMode", 3);
            var result = paymentModeService.GetPaymentMode(id);
            if (result != null)
            {
                model = mapper.Map<PaymentModeSetupViewModel>(result);
                model.Code = id;
            }
            else
            {
                model.PaymentModeId = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Setup(PaymentModeSetupViewModel model)
        {
            if (paymentModeService.IsPaymentModeExist(model.PaymentModeName, model.PaymentModeId))
            {
                return Json(new { isSuccess = false, message = "Already Exists" });
            }

            if (ModelState.IsValid)
            {
                SalesDefPaymentMode paymentMode = paymentModeService.GetPaymentMode(model.PaymentModeId) ?? new SalesDefPaymentMode();
                model.ToAudit(LoginInfo,  !string.IsNullOrWhiteSpace(model.Lip));
                mapper.Map(model, paymentMode);
                paymentMode.CompanyCode = "";
                paymentMode.EmployeeId = "";
                paymentModeService.SavePaymentMode(paymentMode);
                return Json(new { isSuccess = true, message = "Saved Successfully" });
            }

            return Json(new { success = false, message = ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage });
        }

        public ActionResult Grid()
        {
            var resutl = paymentModeService.GetPaymentModes();
            return Json(new { data = resutl });
        }


        [HttpPost]
        public ActionResult Delete(string id)
        {
            bool success = false;
            foreach (var item in id.Split(",", StringSplitOptions.RemoveEmptyEntries))
            {
                success = paymentModeService.DeletePaymentMode(item);
            }

            return Json(new { success = success, message = "Deleted Successfully" });
        }


        [HttpPost]
        public JsonResult CheckAvailability(string name, string code)
        {
            if (paymentModeService.IsPaymentModeExist(name, code))
            {
                return Json(new { isSuccess = true, message = "Already Exists" });
            }

            return Json(new { isSuccess = false });
        }
    }
}