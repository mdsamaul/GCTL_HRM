using GCTL.Core.ViewModels.BankInformations;
using GCTL.Service.BankInformations;
using GCTL.Service.Common;
using GCTL.UI.Core.ViewModels.BankInformations;
using Microsoft.AspNetCore.Mvc;
using GCTL.Core.Helpers;
namespace GCTL.UI.Core.Controllers
{
    public class BankInformationsController : BaseController
    {

        private readonly IBankInformationsService service;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;
        public BankInformationsController(IBankInformationsService service, ICommonService commonService)
        {
            this.service = service;
            this.commonService = commonService;
        }


        #region GettALLById
        public async Task<IActionResult> Index(string? id)
        {
            var hasPermission = await service.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            BankInformationsPageViewModel model = new BankInformationsPageViewModel();
            commonService.FindMaxNo(ref strMaxNO, "BankId", "SALES_Def_BankInfo", 2);

            if (!string.IsNullOrEmpty(id))
            {

                model.Setup = await service.GetByIdAsync(id)
;
            }
            else
            {
                model.Setup.BankId = strMaxNO;
            }


            model.PageUrl = Url.Action(nameof(Index));

            return View(model);



        }
        #endregion


        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Setup(BankInformationsSetupViewModel modelVM)
        {
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                //    return Json(new { isSuccess = false, message = "Validation failed", errors });
                //}

                if (await service.IsExistAsync(modelVM.BankName, modelVM.BankId))
                {
                    return Json(new { isSuccess = false, message = $"Already  Exists!", isDuplicate = true });
                }


                if (string.IsNullOrEmpty(modelVM.BankId))
                {
                    modelVM.BankId = await service.GenerateNextCode();
                }


                modelVM.ToAudit(LoginInfo, modelVM.AutoId > 0);
                if (modelVM.AutoId == 0)
                {
                    var hasSavePermission = await service.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await service.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Saved Successfully", lastCode = modelVM.BankId });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await service.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await service.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Updated Successfully", lastCode = modelVM.BankId });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to update", noUpdatePermission = true });
                    }
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        #endregion


        #region Delete
        [HttpPost]
        public async Task<IActionResult> Delete(List<string> ids)
        {
            try
            {
                var hasPermission = await service.DeletePermissionAsync(LoginInfo.AccessCode);
                if (hasPermission)
                {
                    foreach (var id in ids)
                    {
                        var result = service.DeleteBank(id)
;
                    }

                    return Json(new { isSuccess = true, message = "Data Deleted Successfully" });
                }
                else
                {
                    return Json(new { isSuccess = false, message = "You have no access" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting leave type: {ex.Message}");

                return StatusCode(500, new { isSuccess = false, message = ex.Message });
            }
        }
        #endregion


        #region NeaxtCode
        [HttpGet]
        public async Task<IActionResult> GenerateNextCode()
        {
            var nextCode = await service.GenerateNextCode();
            return Json(nextCode);
        }

        #endregion


        #region TabeleLodaing

        [HttpGet]
        public async Task<IActionResult> GetTableData()
        {
            try
            {
                var list = await service.GetAllAsync();
                return PartialView("_Grid", list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        #endregion


        #region CheckAvailability

        [HttpPost]
        public async Task<JsonResult> CheckAvailability(string name, string typeCode)
        {

            if (await service.IsExistAsync(name, typeCode))
            {
                return Json(new { isSuccess = true, message = $"Already exists!." });
            }

            return Json(new { isSuccess = false });
        }




        #endregion
    }
}