using GCTL.Core.Data;
using GCTL.Core.ViewModels.CoreBankAccountInformations;
using GCTL.Core.ViewModels.SalesDefBankBranchInfos;
using GCTL.Data.Models;
using GCTL.Service.BankBranchInformations;
using GCTL.Service.Common;
using GCTL.Service.CoreBankAccountInformations;
using GCTL.UI.Core.ViewModels.CoreBankAccountInformations;
using GCTL.UI.Core.ViewModels.SalesDefBankBranchInfos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GCTL.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using GCTL.Core.ViewModels.Designations;
using GCTL.Service.Designations;
namespace GCTL.UI.Core.Controllers
{
    public class BankAccountInfoController : BaseController
    {
        private readonly ICommonService commonService;
        private readonly IRepository<SalesDefBankInfo> bankRepository;
        private readonly ICoreBankAccountInformationService bankAccountInformationRepository;
        private readonly IRepository<SalesDefBankBranchInfo> branchBankRepostory;
        string maxNo = "";
        public BankAccountInfoController(ICommonService commonService, IRepository<SalesDefBankBranchInfo> branchBankRepostory, IRepository<SalesDefBankInfo> bankRepository, ICoreBankAccountInformationService bankAccountInformationRepository)
        {
            this.commonService = commonService;
            this.bankRepository = bankRepository;
            this.bankAccountInformationRepository = bankAccountInformationRepository;
            this.branchBankRepostory = branchBankRepostory;
        }



        #region GettALLById
        public async Task<IActionResult> Index(string? id)
        {
            //var hasPermission = await bankAccountInformationRepository.PagePermissionAsync(LoginInfo.AccessCode);
            //if (!hasPermission)
            //{
            //    return RedirectToAction("Login", "Accounts");
            //}
            //Get all
            CoreBankAccountInformationPageViewModel model = new CoreBankAccountInformationPageViewModel();
            var list = await bankAccountInformationRepository.GetAllAsync();
            model.TableList = list ?? new List<CoreBankAccountInformationSetupViewModel>();
            //Get By Id
            if (!string.IsNullOrEmpty(id))
            {

                model.Setup = await bankAccountInformationRepository.GetByIdAsync(id);
            }
            else
            {
               
                commonService.FindMaxNo(ref maxNo, "AccInfoID", "Core_BankAccountInformation", 2);
                model.Setup = new CoreBankAccountInformationSetupViewModel
                {
                    AccInfoId = maxNo 
                };
            }
            ViewBag.BankDD = new SelectList(bankRepository.All(), "BankId", "BankName");
            ViewBag.BranchDD = new SelectList(branchBankRepostory.All(), "BankBranchId", "BankBranchName");
            model.PageUrl = Url.Action(nameof(Index));

            return View(model);


        }
        #endregion
        //
      

        //
        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Setup(CoreBankAccountInformationSetupViewModel modelVM)
        {
            try
            {

                if (await bankAccountInformationRepository.IsExistAsync( modelVM.AccountName, modelVM.AccInfoId, modelVM.AccountNo, modelVM.BankId, modelVM.BranchId))
                {
                    return Json(new { isSuccess = false, message = $"Already Exists!", isDuplicate = true });
                }



                if (string.IsNullOrEmpty(modelVM.AccInfoId))
                {
                    modelVM.AccInfoId = await bankAccountInformationRepository.GenerateNexCode();
                }


                modelVM.ToAudit(LoginInfo, modelVM.AutoId > 0);
                if (modelVM.AutoId == 0)
                {
                    await bankAccountInformationRepository.SaveAsync(modelVM, LoginInfo.CompanyCode);
                    return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = modelVM.AccInfoId });
                    //var hasSavePermission = await bankAccountInformationRepository.SavePermissionAsync(LoginInfo.AccessCode);
                    //if (hasSavePermission)
                    //{
                    //    await bankAccountInformationRepository.SaveAsync(modelVM);
                    //    return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = modelVM.AccInfoId });
                    //}
                    //else
                    //{
                    //    return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    //}
                }
                else
                {
                    await bankAccountInformationRepository.UpdateAsync(modelVM);
                    return Json(new { isSuccess = true, message = "Updated Successfully.", lastCode = modelVM.AccInfoId });
                    //var hasUpdatePermission = await bankAccountInformationRepository.UpdatePermissionAsync(LoginInfo.AccessCode);
                    //if (hasUpdatePermission)
                    //{
                    //    await bankAccountInformationRepository.UpdateAsync(modelVM);
                    //    return Json(new { isSuccess = true, message = "Updated Successfully.", lastCode = modelVM.AccInfoId });
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
                    var result = bankAccountInformationRepository.DeleteLeaveType(id);

                }

                return Json(new { isSuccess = true, message = "Data Deleted Successfully." });

                //var hasPermission = await bankAccountInformationRepository.DeletePermissionAsync(LoginInfo.AccessCode);
                //if (hasPermission)
                //{

                //    foreach (var id in ids)
                //    {
                //        var result = bankAccountInformationRepository.DeleteLeaveType(id);

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
            var nexCode = await bankAccountInformationRepository.GenerateNexCode();
            return Json(nexCode);

        }
        #endregion

        #region LoadTableData
        [HttpGet]
        public async Task<IActionResult> GetTableData()
        {
            var list = await bankAccountInformationRepository.GetAllAsync();
            return PartialView("_Grid", list);
        }
        #endregion

        #region CheckAvailability
        [HttpPost]

        public async Task<JsonResult> CheckAvailability(string accountName, string typeCode, string accountNo, string bankId, string branchId)
        {
            if (await bankAccountInformationRepository.IsExistAsync(accountName, typeCode, accountNo,bankId, branchId))
            {
                return Json(new { isSuccess = true, message = $"Already Exists!." });
            }
            return Json(new { isSuccess = false });
        }
        #endregion

        #region  Branch Filterig according to bank 
        [HttpGet]
        //public async Task<JsonResult> GetBranchesByBankId(string bankId)
        //{
        //    if (string.IsNullOrEmpty(bankId))
        //    {
        //        return Json(new List<SelectListItem>());
        //    }

        //    var branches = await branchBankRepostory.All().Where(b => b.BankId == bankId)
        //        .Select(b => new SelectListItem
        //        {
        //            Value = b.BankBranchId,
        //            Text = b.BankBranchName,

        //        }).ToListAsync();

        //    return Json(branches);

        //}

        public IActionResult GetBranchesByBankId(string bankId)
        {
            var branches = branchBankRepostory.All()
                .Where(x => x.BankId == bankId)
                .Select(x => new
                {
                    value = x.BankBranchId,
                    text = x.BankBranchName,
                    address = x.Address
                }).ToList();

            return Json(branches);
        }





        #endregion

    }
}
