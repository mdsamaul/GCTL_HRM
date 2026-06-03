using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.InstructionInformation;
using GCTL.Service.Common;
using GCTL.Service.InstructionInformation;
using GCTL.Service.SupplierType;
using GCTL.UI.Core.ViewModels.InstructionInformation;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class InstructionInformationController : BaseController
    {
        #region Service & Repository
        public readonly IInstructionInformationService instructionInformationService;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;

        public InstructionInformationController(
            IInstructionInformationService instructionInformationService,
            ICommonService commonService
            
            )
        {
            this.instructionInformationService = instructionInformationService;
            this.commonService = commonService;
        }

        #endregion

        #region Index
        public async Task<IActionResult> Index(bool child = false)
        {
            var hasPermission = await instructionInformationService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var model = new InstructionInformationPageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {

                var list = await instructionInformationService.GetAllAsync();
                model.InstructionList = list ?? new List<InstructionInformationSetupViewModel>();

                commonService.FindMaxNo(ref strMaxNO, "InstructionId", "RMG_Prod_Def_Instruction", 3);

                model.Setup = new InstructionInformationSetupViewModel
                {
                    InstructionId = strMaxNO
                };

            }
            catch (Exception ex)
            {

                model.InstructionList = new List<InstructionInformationSetupViewModel>();
                model.Setup = new InstructionInformationSetupViewModel();
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
            InstructionInformationSetupViewModel model = new InstructionInformationSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "InstructionId", "RMG_Prod_Def_Instruction", 3);

            if (!string.IsNullOrEmpty(id))
            {

                model = await instructionInformationService.GetByIdAsync(id);
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {

                model.InstructionId = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }

        #endregion

        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(InstructionInformationSetupViewModel modelVM)
        {
            try
            {

                if (await instructionInformationService.IsExistAsync(modelVM.Instruction, modelVM.InstructionId))
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
                    var hasSavePermission = await instructionInformationService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await instructionInformationService.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = modelVM.InstructionId });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await instructionInformationService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await instructionInformationService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Updated Successfully.", lastCode = modelVM.InstructionId });
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

            var hasPermission = await instructionInformationService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success = await instructionInformationService.DeleteTab(ids);
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
                var list = await instructionInformationService.GetAllAsync();
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
        //    InstructionInformationPageViewModel model = new InstructionInformationPageViewModel
        //    {
        //        Setup = new InstructionInformationSetupViewModel()
        //    };
        //    return View(model);
        //}

        #endregion
    }
}
