using GCTL.Core.Data;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.ManualAttendanceBulk;
using GCTL.Data.Models;
using GCTL.Service.ManualAttendanceBulks;
using GCTL.UI.Core.ViewModels.ManualAttendanceBulk;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace GCTL.UI.Core.Controllers
{
    public class ManualAttendanceBulkController : BaseController
    {
        #region Services & Repositories
        private readonly IManualAttendanceBulkService _manualAttendanceBulkService;
        private readonly IRepository<CoreCompany> _companyRepository;
        private readonly IRepository<HrmDefDepartment> _departmentRepository;
        private readonly IRepository<CoreBranch> _branchRepository;
        private readonly IRepository<HrmDefDesignation> _designationRepository;
        private readonly IRepository<HrmAtdAttendanceType> _attendanceTypeRepository;
        private readonly IRepository<HrmEmployee> _employeeRepository;

        public ManualAttendanceBulkController(
            IManualAttendanceBulkService manualAttendanceBulkService,
            IRepository<CoreCompany> companyRepository,
            IRepository<HrmAtdAttendanceType> attendanceTypeRepository,
            IRepository<HrmEmployee> employeeRepository,
            IRepository<HrmDefDepartment> departmentRepository,
            IRepository<CoreBranch> branchRepository,
            IRepository<HrmDefDesignation> designationRepository)
        {
            _manualAttendanceBulkService = manualAttendanceBulkService;
            _companyRepository = companyRepository;
            _attendanceTypeRepository = attendanceTypeRepository;
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _branchRepository = branchRepository;
            _designationRepository = designationRepository;
        }
        #endregion


        #region Index, GetById, & GetAll
        public async Task<IActionResult> Index(string code)
        {
            var hasPermission = await _manualAttendanceBulkService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            ManualAttendanceBulkPageViewModel model = new ManualAttendanceBulkPageViewModel();
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
            ViewBag.DepartmentDD = new SelectList(_departmentRepository.All(), "DepartmentCode", "DepartmentName");
            ViewBag.BranchDD = new SelectList(_branchRepository.All(), "BranchCode", "BranchName");
            ViewBag.DesignationDD = new SelectList(_designationRepository.All(), "DesignationCode", "DesignationName");
            ViewBag.AttendanceTypeDD = new SelectList(_attendanceTypeRepository.All(), "AttendanceTypeCode", "AttendanceTypeName");
            ViewBag.AttendanceTypeDDTwo = new SelectList(_attendanceTypeRepository.All(), "AttendanceTypeCode", "AttendanceTypeName");
            ViewBag.ListTypeDD = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "All Employees", Text = "All Employees" },
                new SelectListItem { Value = "Late Employees", Text = "Late Employees" },
                new SelectListItem { Value = "Absent Employees", Text = "Absent Employees" },
                new SelectListItem { Value = "Out Missing", Text = "Out Missing" }
            }, "Value", "Text");

            ViewBag.ActivityStatusDD = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "01", Text = "Active" },
                new SelectListItem { Value = "02", Text = "In-active" }
            }, "Value", "Text");

            return View(model);
        }
        #endregion


        #region GetAll/Table Data



        [HttpPost]
        public async Task<IActionResult> GetAll(string companyId)
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
            var length = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "10");
            var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
            var sortColumnName = Request.Form[$"columns[{sortColumnIndex}][data]"].FirstOrDefault();
            var sortDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            var (totalRecords, data) = await _manualAttendanceBulkService.GetAllPagedAsync(
                companyId, start, length, sortColumnName, sortDirection, searchValue);

            return Json(new
            {
                draw = draw,
                recordsFiltered = totalRecords,
                recordsTotal = totalRecords,
                data = data
            });
        }
        #endregion


        #region GetEmployeeByCompany
        [HttpGet]
        public async Task<IActionResult> GetEmployeeByCompany(string companyId)
        {
            try
            {
                List<ManualAttendanceBulkSetupViewModel> result;

                if (companyId == null)
                {
                    result = await _manualAttendanceBulkService.GetEmployeeByCompanyId(null);
                }
                else
                {
                    result = await _manualAttendanceBulkService.GetEmployeeByCompanyId(companyId);
                }

                if (result == null || result.Count == 0)
                {
                    return Json(new { message = "No data found" });
                }

                return PartialView("_EmployeeList", result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        #endregion


        #region Create/Setup
        [HttpPost]
        public async Task<IActionResult> Setup(ManualAttendanceBulkSetupViewModel model, string selectedEmployeeIds)
        {
            try
            {
                model.ToAudit(LoginInfo, model.AutoId > 0);

                var hasPermission = await _manualAttendanceBulkService.SavePermissionAsync(LoginInfo.AccessCode);
                if (!hasPermission)
                {
                    return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                }
                var employeeIds = JsonConvert.DeserializeObject<List<string>>(selectedEmployeeIds);

                model.ToAudit(LoginInfo);
                await _manualAttendanceBulkService.SaveAsync(model, employeeIds);
                return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = model.ManualCode, companyCode = model.CompanyCode });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        #endregion


        #region Delete
        //[HttpPost]
        //public async Task<IActionResult> Delete(string ids, string selectedEmployeeIds, string attendanceTypeCode, string fromDate, string toDate)
        //{
        //    if (string.IsNullOrEmpty(ids) && string.IsNullOrEmpty(selectedEmployeeIds))
        //    {
        //        return BadRequest(new { isSuccess = false, message = "No IDs or Employee IDs provided for deletion." });
        //    }

        //    // Convert the comma-separated string to lists if the respective parameter is not empty
        //    var idsList = string.IsNullOrEmpty(ids) ? new List<string>() : ids.Split(',').ToList();
        //    var employeeIdsList = string.IsNullOrEmpty(selectedEmployeeIds) ? new List<string>() : selectedEmployeeIds.Split(',').ToList();

        //    var hasPermission = await _manualAttendanceBulkService.DeletePermissionAsync(LoginInfo.AccessCode);
        //    if (!hasPermission)
        //    {
        //        return Json(new { isSuccess = false, message = "You have no access." });
        //    }

        //    bool success = await _manualAttendanceBulkService.DeleteAsync(idsList, employeeIdsList, attendanceTypeCode, fromDate, toDate);
        //    if (success)
        //    {
        //        return Json(new { isSuccess = true, message = "Successfully Deleted." });
        //    }
        //    else
        //    {
        //        return Json(new { isSuccess = false, message = "Deletion failed. Some entities may still exist." });
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> Delete(string selectedEmployeeIds, string attendanceTypeCode, string fromDate, string toDate, bool isBothInOutEntry)
        {
            if (string.IsNullOrEmpty(selectedEmployeeIds))
                return BadRequest(new { isSuccess = false, message = "No Employee IDs provided for deletion." });

            if (string.IsNullOrEmpty(fromDate) || string.IsNullOrEmpty(toDate))
                return BadRequest(new { isSuccess = false, message = "Date range is required." });

            var hasPermission = await _manualAttendanceBulkService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
                return Json(new { isSuccess = false, message = "You have no access." });

            var employeeIdsList = selectedEmployeeIds.Split(',').ToList();


            DeleteHistoryViewModel Dmodel = new DeleteHistoryViewModel();
            Dmodel.ToAudit(LoginInfo);
            Dmodel.CompanyCode = LoginInfo.CompanyCode;
            bool success = await _manualAttendanceBulkService.DeleteAsync(
                employeeIdsList,
                attendanceTypeCode,
                fromDate,
                toDate,
                isBothInOutEntry,
                Dmodel
            );

            return Json(success
                ? new { isSuccess = true, message = "Successfully Deleted." }
                : new { isSuccess = false, message = "Deletion failed. No matching records found." });
        }
        #endregion


        #region GetBranchByCompany
        [HttpGet]
        public async Task<IActionResult> GetBranchByCompany(string companyId)
        {
            var result = await _manualAttendanceBulkService.GetBranchByCompanyId(companyId);
            return Json(result);
        }
        #endregion


        #region GetCompanyTableDataById
        [HttpGet]
        public async Task<IActionResult> GetCompanyTableDataById(string companyId)
        {
            var result = await _manualAttendanceBulkService.GetCompanyDataById(companyId);

            if (result == null || !result.Any())
            {
                return PartialView("_Grid", new List<ManualAttendanceBulkSetupViewModel>()); // Empty list
            }

            return PartialView("_Grid", result);
        }
        #endregion


        #region GetDepartmentByCompany 
        [HttpGet]
        public async Task<IActionResult> GetDepartmentByCompany(string companyId)
        {
            var result = await _manualAttendanceBulkService.GetDepartmentByCompanyId(companyId);
            return Json(result);
        }
        #endregion


        #region GetDepartmentByBranchId
        [HttpGet]
        public async Task<IActionResult> GetDepartmentByBranchId(string branchId)
        {
            var result = await _manualAttendanceBulkService.GetDepartmentByBranchId(branchId);
            return Json(result);
        }
        #endregion


        #region GetDesignationByBranch
        [HttpGet]
        public async Task<IActionResult> GetDesignationByBranch(string branchId)
        {
            var result = await _manualAttendanceBulkService.GetDesignationByBranchId(branchId);
            return Json(result);
        }
        #endregion


        #region GetEmployeeByBranch
        [HttpGet]
        public async Task<IActionResult> GetEmployeeByBranch(string companyId, string branchId)
        {
            try
            {
                List<ManualAttendanceBulkSetupViewModel> result;

                if (branchId == null)
                {
                    result = await _manualAttendanceBulkService.GetEmployeeByBranchId(companyId, null);
                }
                else
                {
                    result = await _manualAttendanceBulkService.GetEmployeeByBranchId(companyId, branchId);
                }

                if (result == null || result.Count == 0)
                {
                    return Json(new { message = "No data found" });
                }

                return PartialView("_EmployeeList", result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        #endregion


        #region GetEmployeeByDepartment
       

        [HttpGet]
        public async Task<IActionResult> GetEmployeeByDepartment(string companyId, string branchId, [FromQuery] List<string> departmentId, string selectedListType, string selectedActiveStatus="01")
        {
            try
            {
                List<ManualAttendanceBulkSetupViewModel> result;

                if (departmentId == null || departmentId.Count == 0)
                {
                    result = await _manualAttendanceBulkService.GetEmployeeByDepartmentId(companyId, branchId, null, null, null);
                }
                else
                {
                    result = await _manualAttendanceBulkService.GetEmployeeByDepartmentId(companyId, branchId, departmentId, selectedListType, selectedActiveStatus);
                }

                if (result == null || result.Count == 0)
                {
                    return Json(new { message = "No data found" });
                }

                return PartialView("_EmployeeList", result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        #endregion


        #region GetEmployeeByDesignation
        

        [HttpGet]
        public async Task<IActionResult> GetEmployeeByDesignation(string companyId, string branchId, [FromQuery] List<string> departmentId, [FromQuery] List<string> designationId, string selectedListType, string selectedActiveStatus = "01")
        {
            try
            {
                List<ManualAttendanceBulkSetupViewModel> result;

               
                    result = await _manualAttendanceBulkService.GetEmployeeByDesignationId(companyId, branchId, departmentId, designationId, selectedListType, selectedActiveStatus);
               

                if (result == null || result.Count == 0)
                {
                    return Json(new { message = "No data found" });
                }

                return PartialView("_EmployeeList", result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        #endregion


        #region GetDesignationByCompany
        [HttpGet]
        public async Task<IActionResult> GetDesignationByCompany(string companyId)
        {
            var result = await _manualAttendanceBulkService.GetDesignationByCompanyId(companyId);
            return Json(result);
        }
        #endregion


        #region GetDesignationByDepartment
        [HttpGet]
        public async Task<IActionResult> GetDesignationByDepartment([FromQuery] List<string> departmentId)
        {
            var result = await _manualAttendanceBulkService.GetDesignationByDepartmentId(departmentId);
            return Json(result);
        }
        //[HttpGet]
        //public async Task<IActionResult> GetDesignationByDepartment(string departmentId)
        //{
        //    var result = await _manualAttendanceBulkService.GetDesignationByDepartmentId(departmentId);
        //    return Json(result);
        //}
        #endregion


        #region GetEmployeeDetailsById
        [HttpGet]
        public async Task<IActionResult> GetEmployeeDetailsById(string Id)
        {
            var result = await _manualAttendanceBulkService.GetEmployeeDetailsById(Id);
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
        #endregion


        #region GetEmployeeTableDataById
        [HttpGet]
        public async Task<IActionResult> GetEmployeeTableDataById(string employeeId)
        {
            var result = await _manualAttendanceBulkService.GetEmployeeDataById(employeeId);

            if (result == null || !result.Any())
            {
                return PartialView("_Grid", new List<ManualAttendanceBulkSetupViewModel>()); // Empty list
            }

            return PartialView("_Grid", result);
        }
        #endregion
    }
}
