using GCTL.Core.Data;
using GCTL.Core.ViewModels.PaymentTerms;
using GCTL.Data.Models;
using GCTL.Service.Common;
using GCTL.Core.Helpers;
using GCTL.Service.PaymentTerms;
using GCTL.UI.Core.ViewModels.PaymentTerms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GCTL.UI.Core.Controllers
{
    public class PaymentTermsController : BaseController
    {
        #region Service & Repository
        public readonly IPaymentTermsService paymentTermsService;
        private readonly IRepository<SalesDefPaymentType> paymentTyperepository;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;

        public PaymentTermsController(
            IPaymentTermsService paymentTermsService,
            IRepository<SalesDefPaymentType> paymentTyperepository,
            ICommonService commonService
            
            )
        {
            this.paymentTermsService = paymentTermsService;
            this.paymentTyperepository = paymentTyperepository;
            this.commonService = commonService;
        }

        #endregion

        #region Index
        public async Task<IActionResult> Index(bool child = false)
        {
            var hasPermission = await paymentTermsService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var model = new PaymentTermsPageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {

                var list = await paymentTermsService.GetAllAsync();
                model.PaymentTermsList = list ?? new List<PaymentTermsSetupViewModel>();

                commonService.FindMaxNo(ref strMaxNO, "PaymentTermsId", "Sales_Def_PaymentTerms", 3);

                ViewBag.TypeDD = new SelectList(paymentTyperepository.All(), "PaymentTypeId", "PaymentType");


                model.Setup = new PaymentTermsSetupViewModel
                {
                    PaymentTermsId = strMaxNO
                };

            }
            catch (Exception ex)
            {

                model.PaymentTermsList = new List<PaymentTermsSetupViewModel>();
                model.Setup = new PaymentTermsSetupViewModel();
                Console.WriteLine("Error" + ex.Message);
            }


            if (child)
                return PartialView(model);

            return View(model);
        }

        #endregion

        #region Setup

        public async Task<IActionResult> Setup(string id)
        {
            PaymentTermsSetupViewModel model = new PaymentTermsSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "PaymentTermsId", "Sales_Def_PaymentTerms", 3);

            ViewBag.TypeDD = new SelectList(paymentTyperepository.All(), "PaymentTypeId", "PaymentType");

            if (!string.IsNullOrEmpty(id))
            {

                model = await paymentTermsService.GetByIdAsync(id);
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {

                model.PaymentTermsId = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }

        #endregion

        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(PaymentTermsSetupViewModel modelVM)
        {
            try
            {
                if (await paymentTermsService.IsExistAsync(modelVM.PaymentTermsName, modelVM.PaymentTermsId))
                {
                    return Json(new { isSuccess = false, message = $"Already Exists!", isDuplicate = true });
                }


                if (!ModelState.IsValid)
                {

                    var errorMessage = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                    return Json(new { isSuccess = false, message = errorMessage });
                }

                modelVM.ToAudit(LoginInfo, modelVM.Tc > 0);
                if (modelVM.Tc == 0)
                {
                    var hasSavePermission = await paymentTermsService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await paymentTermsService.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = modelVM.PaymentTermsId });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await paymentTermsService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await paymentTermsService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Updated Successfully.", lastCode = modelVM.PaymentTermsId });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to update.", noUpdatePermission = true });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error:{ex.Message}");
                return RedirectToAction("Login", "Accounts");

            }
        }

        #endregion

        #region Delete

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] List<string> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return BadRequest(new { success = false, message = "No IDs provided for delete." });
            }

            var hasPermission = await paymentTermsService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success = await paymentTermsService.DeleteTab(ids);
            if (success)
            {
                return Json(new { success = true, message = "Deleted Successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Deletion failed." });
            }
        }

        #endregion

        #region TabeleLodaing

        [HttpGet]
        public async Task<IActionResult> GetTableData()
        {
            try
            {
                var list = await paymentTermsService.GetAllAsync();
                return PartialView("_Grid", list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        #endregion

        #region Chake Degian

        //public IActionResult Index()
        //{
        //    PaymentTermsPageViewModel model = new PaymentTermsPageViewModel
        //    {
        //        Setup = new PaymentTermsSetupViewModel()
        //    };
        //    return View(model);
        //}

        #endregion
    }
}
