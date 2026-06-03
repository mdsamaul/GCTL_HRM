using GCTL.Core.Data;
using GCTL.Core.ViewModels.HolidayTypes;
using GCTL.Core.ViewModels.HrmAtdHolidays;
using GCTL.Data.Models;
using GCTL.Core.Helpers;
using GCTL.Service.Common;
using GCTL.Service.HolidayTypes;
using GCTL.Service.HrmAtdHolidays;
using GCTL.UI.Core.ViewModels.HolidayTypes;
using GCTL.UI.Core.ViewModels.HrmAtdHolidays;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace GCTL.UI.Core.Controllers
{
    public class HrmAtdHolidayInformationController : BaseController
    {
        private readonly IHrmAtdHolidayService service;
        private readonly ICommonService commonService;
        private readonly IRepository<HrmDefHolidayType> holidayType;
        private readonly IRepository<HrmDefDesignation> designationRepository;
        public HrmAtdHolidayInformationController(IHrmAtdHolidayService service, IRepository<HrmDefDesignation> designationRepository,IRepository<HrmDefHolidayType> holidayType, ICommonService commonService)
        {
            this.service = service;
            this.commonService = commonService;
            this.holidayType = holidayType;
            this.designationRepository = designationRepository;
        }

        #region GettALLById
        public async Task<IActionResult> Index(string? id)
        {
            var hasPermission = await service.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }
            //Get all
            HrmAtdHolidayPageViewModel model = new HrmAtdHolidayPageViewModel();
            var list = await service.GetAllAsync();
            model.HrmAdtHolidayList = list ?? new List<HrmAtdHolidaySetupViewModel>();
            //Get By Id
            if (!string.IsNullOrEmpty(id))
            {

                model.Setup = await service.GetByIdAsync(id);
            }

          

            model.PageUrl = Url.Action(nameof(Index));
            //ViewBag.Designations = new SelectList(designationRepository.All(), "DesignationCode", "DesignationName");
            //ViewBag.HolidayTypeCodeDD = new SelectList(holidayType.All(), "HolidayTypeCode", "HolidayType",model.Setup.HolidayTypeCode);
           //ViewBag.HolidayTypeCodeDD = new SelectList(service.DropSelection(), "Code", "Name");
            return View(model);



        }
        #endregion

        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Setup(HrmAtdHolidaySetupViewModel modelVM)
        {
            try
            {


                //if (await service.IsExistAsync(modelVM.HolidayName, modelVM.HolidayCode, modelVM.HolidayTypeCode, modelVM.FromDate.ToString("MM/dd/yyyy"), modelVM.ToDate.ToString("MM/dd/yyyy")))
                //{
                //    return Json(new { isSuccess = false, message = $"Already  Exists!", isDuplicate = true });
                //}

               
                    if (string.IsNullOrEmpty(modelVM.HolidayCode))
                    {
                        modelVM.HolidayCode = await service.GenerateNextCode();
                    }


                    modelVM.ToAudit(LoginInfo, modelVM.AutoId > 0);
                    if (modelVM.AutoId == 0)
                    {
                        var hasSavePermission = await service.SavePermissionAsync(LoginInfo.AccessCode);
                        if (hasSavePermission)
                        {
                            await service.SaveAsync(modelVM);
                            return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = modelVM.HolidayCode });
                        }
                        else
                        {
                            return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                        }
                    }
                    else
                    {

                        var hasUpdatePermission = await service.UpdatePermissionAsync(LoginInfo.AccessCode);
                        if (hasUpdatePermission)
                        {
                            await service.UpdateAsync(modelVM);
                            return Json(new { isSuccess = true, message = "Updated Successfully.", lastCode = modelVM.HolidayCode });
                        }
                        else
                        {
                            return Json(new { isSuccess = false, message = "You have no access to update.", noUpdatePermission = true });
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

                var hasPermission = await service.DeletePermissionAsync(LoginInfo.AccessCode);
                if (hasPermission)
                {

                    foreach (var id in ids)
                    {
                        var result = service.DeleteLeaveType(id);

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

        #region CheckAvailability

        [HttpPost]
        public async Task<JsonResult> CheckAvailability(string name, string code, string holidayTypeCode,string fromDate ,string toDate)
        {

            if (await service.IsExistAsync(name, code, holidayTypeCode, fromDate,toDate))
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
