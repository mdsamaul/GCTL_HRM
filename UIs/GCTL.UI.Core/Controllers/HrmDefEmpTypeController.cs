using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.HRMATDAttendanceTypes;
using GCTL.Core.ViewModels.HrmDefEmpTypes;
using GCTL.Core.ViewModels.HRMDefExamTitles;
using GCTL.Core.ViewModels.HrmDefInstitutes;
using GCTL.Service;
using GCTL.Service.Common;
using GCTL.Service.Grades;
using GCTL.Service.HRMATDAttendanceTypes;
using GCTL.Service.HrmDefEmpTypes;
using GCTL.UI.Core.ViewModels.HRMATDAttendanceTypes;
using GCTL.UI.Core.ViewModels.HrmDefEmpTypes;
using GCTL.UI.Core.ViewModels.HRMDefExamTitles;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class HrmDefEmpTypeController : BaseController
    {
        private readonly IHrmDefEmpTypeService service;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;
        public HrmDefEmpTypeController(IHrmDefEmpTypeService service, ICommonService commonService)
        {
            this.service = service;
            this.commonService = commonService;
        }


        #region GettALLById
        public async Task<IActionResult> Index(bool child = false)
        {
            var hasPermission = await service.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var model = new HrmDefEmpTypePageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {
                var list = await service.GetAllAsync();
                model.tableDataList = list ?? new List<HrmDefEmpTypeSetupViewModel>();

                commonService.FindMaxNo(ref strMaxNO, "EmpTypeCode", "HRM_Def_EmpType", 2);
                model.Setup = new HrmDefEmpTypeSetupViewModel
                {
                    EmpTypeCode = strMaxNO
                };

            }
            catch (Exception ex)
            {
                model.tableDataList = new List<HrmDefEmpTypeSetupViewModel>();
                model.Setup = new HrmDefEmpTypeSetupViewModel();
                Console.WriteLine("Error" + ex.Message);
            }

            if (child)
                return PartialView(model);

            return View(model);
        }




        public async Task<IActionResult> Setup(string id)
        {
            HrmDefEmpTypeSetupViewModel model = new HrmDefEmpTypeSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "EmpTypeCode", "HRM_Def_EmpType", 2);

            if (!string.IsNullOrEmpty(id))
            {
                model = await service.GetByIdAsync(id);
                if (model == null)
                {
                    return NotFound();
                }
            }
            else
            {
                model.EmpTypeCode = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }
        #endregion


        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Setup(HrmDefEmpTypeSetupViewModel modelVM)
        {
            try
            {
                if (await service.IsExistAsync(modelVM.EmpTypeName, modelVM.EmpTypeCode))
                {
                    return Json(new { isSuccess = false, message = $"Already  Exists!", isDuplicate = true });
                }


                if (string.IsNullOrEmpty(modelVM.EmpTypeCode))
                {
                    modelVM.EmpTypeCode = await service.GenerateNextCode();
                }


                modelVM.ToAudit(LoginInfo, modelVM.AutoId > 0);
                if (modelVM.AutoId == 0)
                {
                    var hasSavePermission = await service.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await service.SaveAsync(modelVM, LoginInfo.CompanyCode);
                        return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = modelVM.EmpTypeCode });
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
                        return Json(new { isSuccess = true, message = "Updated Successfully", lastCode = modelVM.EmpTypeCode });
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
        public async Task<IActionResult> Delete([FromBody] List<string> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return BadRequest(new { success = false, message = "No IDs provided for delete." });
                }

                var hasPermission = await service.DeletePermissionAsync(LoginInfo.AccessCode);
                if (!hasPermission)
                {
                    return Json(new { success = false, message = "You have no access." });
                }

                DeleteHistoryViewModel model = new DeleteHistoryViewModel();
                model.ToAudit(LoginInfo);
                model.CompanyCode = LoginInfo.CompanyCode;

                var success = await service.DeleteTab(ids, model);
                if (success.succses)
                {
                    return Json(new { success = true, message = "Deleted Successfully." });
                }
                else
                {
                    return Json(new { success = false, message = success.messege });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
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
    }
}
