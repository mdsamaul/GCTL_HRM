using GCTL.Core.Data;
using GCTL.Core.ViewModels.HrmDefEmpTypes;
using GCTL.Core.ViewModels.SalesDefBankBranchInfos;
using GCTL.Data.Models;
using GCTL.Service.BankBranchInformations;
using GCTL.Service.Common;
using GCTL.UI.Core.ViewModels.HrmDefEmpTypes;
using GCTL.UI.Core.ViewModels.SalesDefBankBranchInfos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GCTL.Core.Helpers;
using GCTL.Service.BankInformations;
namespace GCTL.UI.Core.Controllers
{
    public class BankBranchInformationsController : BaseController
    {
        private readonly ISalesDefBankBranchInfosService bankBranchInfosService;
        private readonly ICommonService commonService;
        private readonly IRepository<SalesDefBankInfo> bankRepository;
        private readonly IBankInformationsService bankservice;
        public BankBranchInformationsController(ISalesDefBankBranchInfosService bankBranchInfosService, IRepository<SalesDefBankInfo> bankRepository, ICommonService commonService)
        {
            this.bankBranchInfosService = bankBranchInfosService;
            this.bankRepository = bankRepository;
            this.commonService = commonService;
        }

        #region GettALLById
        public async Task<IActionResult> Index(string? id)
        {
            //var hasPermission = await bankBranchInfosService.PagePermissionAsync(LoginInfo.AccessCode);
            //if (!hasPermission)
            //{
            //    return RedirectToAction("Login", "Accounts");
            //}
            //Get all
            SalesDefBankBranchInfoPageViewModel model = new SalesDefBankBranchInfoPageViewModel();
            var list = await bankBranchInfosService.GetAllAsync();
            model.TableList = list ?? new List<SalesDefBankBranchInfoSetupViewModel>();
            //Get By Id
            if (!string.IsNullOrEmpty(id))
            {

                model.Setup = await bankBranchInfosService.GetByIdAsync(id);
            }
            ViewBag.BankDD = new SelectList(bankRepository.All(), "BankId", "BankName");

            model.PageUrl = Url.Action(nameof(Index));

            return View(model);



        }
        #endregion


        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Setup(SalesDefBankBranchInfoSetupViewModel modelVM)
        {
            try
            {


                if (await bankBranchInfosService.IsExistAsync(modelVM.BankBranchName, modelVM.BankBranchId,modelVM.BankId))
                {
                    return Json(new { isSuccess = false, message = $"Already  Exists!", isDuplicate = true });
                }


                if (string.IsNullOrEmpty(modelVM.BankBranchId))
                {
                    modelVM.BankBranchId = await bankBranchInfosService.GenearateNextCode();
                }


                modelVM.ToAudit(LoginInfo, modelVM.AutoId > 0);
                if (modelVM.AutoId == 0)
                {
                    await bankBranchInfosService.SaveAsync(modelVM);
                    return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = modelVM.BankBranchId });
                    //var hasSavePermission = await bankBranchInfosService.SavePermissionAsync(LoginInfo.AccessCode);
                    //if (hasSavePermission)
                    //{

                    //}
                    //else
                    //{
                    //    return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    //}
                }
                else
                {
                    await bankBranchInfosService.UpdateAsync(modelVM);
                    return Json(new { isSuccess = true, message = "Updated Successfully.", lastCode = modelVM.BankBranchId });

                    //var hasUpdatePermission = await bankBranchInfosService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    //if (hasUpdatePermission)
                    //{
                       
                    //}
                    //else
                    //{
                    //    return Json(new { isSuccess = false, message = "You have no access to update.", noUpdatePermission = true });
                    //}
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
        public async Task<IActionResult> Delete([FromBody] List<string> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    var result = bankBranchInfosService.DeleteLeaveType(id);

                }

                return Json(new { isSuccess = true, message = "Data Deleted Successfully." });
                //var hasPermission = await bankBranchInfosService.DeletePermissionAsync(LoginInfo.AccessCode);
                //if (hasPermission)
                //{

                //    foreach (var id in ids)
                //    {
                //        var result = bankBranchInfosService.DeleteLeaveType(id);

                //    }

                //    return Json(new { isSuccess = true, message = "Data Deleted Successfully." });
                //}
                //else
                //{

                //    return Json(new { isSuccess = false, message = "You have no access." });
                //}
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
            var nexCode = await bankBranchInfosService.GenearateNextCode();
            return Json(nexCode);
        }
        #endregion

        #region Load table Data
        [HttpGet]
        public async Task<IActionResult> GetTableData()
        {

            try
            {
                var list = await bankBranchInfosService.GetAllAsync();
                return PartialView("_Grid", list);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
           
        }
        #endregion

        #region CheckAvailability
        [HttpPost]

        public async Task<JsonResult> CheckAvailability(string name,string typeCode,string bankId)
        {
            if(await bankBranchInfosService.IsExistAsync(name,typeCode,bankId))
            {
                return Json(new { isSuccess = true, message = $"Already Exists!." });
            }
            return Json(new { isSuccess = false });
        }

       

        #endregion


    }
}
