using GCTL.Core.Data;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.ManualAttendance;
using GCTL.Data.Models;
using GCTL.Service.ManualAttendances;
using GCTL.UI.Core.ViewModels.ManualAttendance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GCTL.UI.Core.Controllers
{
    public class ManualAttendanceController : BaseController
    {
        #region Services & Repositories
        private readonly IManualAttendanceService _manualAttendanceService;
        private readonly IRepository<CoreCompany> _companyRepository;
        private readonly IRepository<HrmAtdAttendanceType> _attendanceTypeRepository;
        private readonly IRepository<HrmEmployee> _employeeRepository;

        public ManualAttendanceController(
            IManualAttendanceService manualAttendanceService,
            IRepository<CoreCompany> companyRepository,
            IRepository<HrmAtdAttendanceType> attendanceTypeRepository,
            IRepository<HrmEmployee> employeeRepository)
        {
            _manualAttendanceService = manualAttendanceService;
            _companyRepository = companyRepository;
            _attendanceTypeRepository = attendanceTypeRepository;
            _employeeRepository = employeeRepository;
        }
        #endregion


        #region Index, GetById, & GetAll
        public async Task<IActionResult> Index(string code)
        {
            var hasPermission = await _manualAttendanceService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            ManualAttendancePageViewModel model = new ManualAttendancePageViewModel();
            model.AddUrl = Url.Action(nameof(Setup));
            model.PageUrl = Url.Action(nameof(Index));

            var companies = await _companyRepository.AllAsync();
            if (companies.Count() == 1)
            {
                model.Setup.CompanyCode = companies.FirstOrDefault().CompanyCode;
            }
            else
            {
                model.Setup.CompanyCode = null;
            }

            ViewBag.CompanyDD = new SelectList(_companyRepository.All(), "CompanyCode", "CompanyName");
            ViewBag.EmployeeDD = new SelectList(_manualAttendanceService.EmployeeSelection(), "Code", "Name");
            ViewBag.AttendanceTypeDD = new SelectList(_attendanceTypeRepository.All(), "AttendanceTypeCode", "AttendanceTypeName");
            ViewBag.AttendanceTypeDDTwo = new SelectList(_attendanceTypeRepository.All(), "AttendanceTypeCode", "AttendanceTypeName");


            return View(model);
        }
        #endregion


        #region GetAllAsync

        [HttpPost]
        public async Task<IActionResult> GetAll()
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
            var length = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "10");
            var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
            var sortColumnName = Request.Form[$"columns[{sortColumnIndex}][data]"].FirstOrDefault();
            var sortDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            var (totalRecords, data) = await _manualAttendanceService.GetAllAsync(
                start, length, sortColumnName, sortDirection, searchValue);

            return Json(new
            {
                draw = draw,
                recordsFiltered = totalRecords,
                recordsTotal = totalRecords,
                data = data
            });
        }

        #endregion


        #region Create / Setup
        [HttpPost]
        public async Task<IActionResult> Setup(ManualAttendanceSetupViewModel model)
        {
            try
            {
                model.ToAudit(LoginInfo, model.AutoId > 0);

                if (string.IsNullOrEmpty(model.ManualCode))
                {
                    model.ManualCode = await _manualAttendanceService.GenerateNextCode();
                }

                var hasPermission = await _manualAttendanceService.SavePermissionAsync(LoginInfo.AccessCode);
                if (!hasPermission)
                {
                    return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                }

                var result = await _manualAttendanceService.SaveAsync(model);
                return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = model.ManualCode, result = result.SavedRecord });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        #endregion

        #region Delete
       
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] DeleteRequestModel request)
        {
            if ((request.Ids == null || !request.Ids.Any()) &&
                (request.SelectedEmployeeIds == null || !request.SelectedEmployeeIds.Any()))
            {
                return BadRequest(new { isSuccess = false, message = "No IDs or Employee IDs provided for deletion." });
            }

            var hasPermission = await _manualAttendanceService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
                return Json(new { isSuccess = false, message = "You have no access." });

            DeleteHistoryViewModel deleteModel = new DeleteHistoryViewModel();
            deleteModel.ToAudit(LoginInfo);
            deleteModel.CompanyCode = LoginInfo.CompanyCode;
            var result = await _manualAttendanceService.DeleteAsync(
                request.Ids,
                request.SelectedEmployeeIds,
                request.AttendanceTypeCode,
                request.FromDate,
                request.ToDate,
                request.IsBothInOutEntry,
                deleteModel
            );

            return Json(result.IsSuccess
                ? new { isSuccess = true, message = "Successfully Deleted.", deletedRecord = result.DeletedRecord }
                : new { isSuccess = false, message = "Deletion failed." });
        }
        #endregion


        #region GetEmployeeByCompany
        [HttpGet]
        public async Task<IActionResult> GetEmployeeByCompany(string companyId)
        {
            var result = await _manualAttendanceService.GetEmployeeByCompany(companyId);
            if (result != null)
            {
                return Json(result);
            }
            else
            {
                return Json(null);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetCompanyTableDataById(string companyId)
        {
            var result = await _manualAttendanceService.GetCompanyDataById(companyId);

            if (result == null || !result.Any())
            {
                return PartialView("_Grid", new List<ManualAttendanceSetupViewModel>()); // Empty list
            }

            return PartialView("_Grid", result);
        }
        #endregion


        #region GetEmployeeDetailsById & GetEmployeeTableDataById


        [HttpGet]
        public async Task<IActionResult> GetEmployeeDetailsById(string id)
        {
            var result = await _manualAttendanceService.GetEmployeeDetailsById(id);
            if (result != null)
            {
                return Json(new
                {
                    employeeId = result.EmployeeId,
                    employeeFullName = result.EmployeeFullName,
                    designationName = result.DesignationName,
                    departmentName = result.DepartmentName
                });
            }
            else
            {
                return Json(null);
            }
        }


        [HttpPost]
        public async Task<IActionResult> GetEmployeeTableDataById(string employeeId)
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
            var length = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "10");
            var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
            var sortColumnName = Request.Form[$"columns[{sortColumnIndex}][data]"].FirstOrDefault();
            var sortDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            var (totalRecords, data) = await _manualAttendanceService.GetEmployeeDataByIdAsync(
                employeeId, start, length, searchValue, sortColumnName, sortDirection);

            return Json(new
            {
                draw = draw,
                recordsFiltered = totalRecords,
                recordsTotal = totalRecords,
                data = data
            });
        }


        #endregion

        [HttpPost]
        public async Task<IActionResult> SandRTimeByEmployee([FromBody] SandRRequest request)
        {
            try
            {
                var result = await _manualAttendanceService.SandRTimeByEmployeeAsync(request.EmployeeId, request.FromDate);

                return Json(new { result });
            }
            catch (Exception)
            {

                throw;
            }
        }
    }

}
