using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.PaymentType;
using GCTL.Service.Common;
using GCTL.Service.PaymentType;
using GCTL.Service.StyleInformation;
using GCTL.UI.Core.ViewModels.PaymentType;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GCTL.UI.Core.Controllers
{
    public class PaymentTypeController : BaseController
    {
        #region Service & Repository
        public readonly IPaymentTypeService paymentTypeService;
        private readonly ICommonService commonService;

        string strMaxNO = string.Empty;

        public PaymentTypeController(
            IPaymentTypeService paymentTypeService,
            ICommonService commonService
            
            )
        {
            this.paymentTypeService = paymentTypeService;
            this.commonService = commonService;
        }

        #endregion

        #region Index
        public async Task<IActionResult> Index(bool child = false)
        {
            var hasPermission = await paymentTypeService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var model = new PaymentTypePageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {

                var list = await paymentTypeService.GetAllAsync();
                model.PaymentList = list ?? new List<PaymentTypeSetupViewModel>();

                commonService.FindMaxNo(ref strMaxNO, "PaymentTypeID", "Sales_Def_PaymentType", 3);



                model.Setup = new PaymentTypeSetupViewModel
                {
                    PaymentTypeId = strMaxNO
                };

            }
            catch (Exception ex)
            {

                model.PaymentList = new List<PaymentTypeSetupViewModel>();
                model.Setup = new PaymentTypeSetupViewModel();
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
            PaymentTypeSetupViewModel model = new PaymentTypeSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "PaymentTypeID", "Sales_Def_PaymentType", 3);

            if (!string.IsNullOrEmpty(id))
            {

                model = await paymentTypeService.GetByIdAsync(id);
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {

                model.PaymentTypeId = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }

        #endregion

        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(PaymentTypeSetupViewModel modelVM)
        {
            try
            {

                if (await paymentTypeService.IsExistAsync(modelVM.PaymentType, modelVM.PaymentTypeId))
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
                    var hasSavePermission = await paymentTypeService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await paymentTypeService.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = modelVM.PaymentTypeId });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await paymentTypeService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await paymentTypeService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Updated Successfully.", lastCode = modelVM.PaymentTypeId });
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

            var hasPermission = await paymentTypeService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success = await paymentTypeService.DeleteTab(ids);
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
                var list = await paymentTypeService.GetAllAsync();
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
        //    PaymentTypePageViewModel model = new PaymentTypePageViewModel
        //    {
        //        Setup = new PaymentTypeSetupViewModel()
        //    };
        //    return View(model);
        //}

        #endregion
    }
}
