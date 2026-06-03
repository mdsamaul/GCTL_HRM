//using GCTL.Core.Data;
////using GCTL.Core.ViewModels.HrmEmployeeOfficialInfo;
//using GCTL.Data.Models;
//using GCTL.Service.HrmAtdMachineDatas;
////using GCTL.UI.Core.ViewModels.HrmEmployeeOfficialInfo;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;

//namespace GCTL.UI.Core.Controllers
//{
//    public class MachineDataController : BaseController
//    {
//        private readonly IHrmAtdMachineDataService _hrmAtdMachineDataService;
//        private readonly IRepository<HrmAtdMachineData> _hrmAtdMachineDataRepository;
//        private readonly IRepository<HrmEmployee> hrmEmployeeRepository;

//        public MachineDataController(
//            IHrmAtdMachineDataService hrmAtdMachineDataService,
//            IRepository<HrmAtdMachineData> hrmAtdMachineDataRepository,
//            IRepository<HrmEmployee> hrmEmployeeRepository
//            )
//        {
//            _hrmAtdMachineDataService = hrmAtdMachineDataService;
//            _hrmAtdMachineDataRepository = hrmAtdMachineDataRepository;
//            this.hrmEmployeeRepository = hrmEmployeeRepository;
//        }

//        public IActionResult Index()
//        {
//            ViewBag.EmployeeList = new SelectList(hrmEmployeeRepository.All().Select(x => new { id = x.EmployeeId, name = x.FirstName + " " + x.LastName + " (" + x.EmployeeId + " )" }), "id", "name");
//            return View();
//        }

//        [HttpPost]
//        public async Task<IActionResult> GetDataTableData()
//        {
//            var draw = Request.Form["draw"].FirstOrDefault();
//            var start = Request.Form["start"].FirstOrDefault();
//            var length = Request.Form["length"].FirstOrDefault();
//            var searchValue = Request.Form["search[value]"].FirstOrDefault();
//            var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
//            var sortColumn = Request.Form[$"columns[{sortColumnIndex}][data]"].FirstOrDefault();
//            var sortDirection = Request.Form["order[0][dir]"].FirstOrDefault();

//            // Calculate pagination values
//            var pageSize = string.IsNullOrEmpty(length) ? 10 : Convert.ToInt32(length);
//            var page = string.IsNullOrEmpty(start) ? 1 : (Convert.ToInt32(start) / pageSize) + 1;

//            // Fetch data from service
//            var result = await _hrmAtdMachineDataService.GetPaginatedDataAsync(searchValue, page, pageSize, sortColumn, sortDirection);

//            // Prepare response for DataTables
//            var response = new
//            {
//                draw = draw,
//                recordsTotal = result.TotalRecords,
//                recordsFiltered = result.TotalRecords,
//                data = result.Data
//            };

//            return Ok(response);
//        }

//        [HttpPost]
//        public async Task<IActionResult> GetEmployeeWiseAttendance(
//            string employeeIds,
//            DateTime? fromDate,
//            DateTime? toDate,
//            int? fromMonth,
//            int? fromYear,
//            int? toMonth,
//            int? toYear)
//        {
//            try
//            {
//                var data = await _hrmAtdMachineDataService
//                    .GetFilteredAttendanceAsync(
//                        employeeIds,
//                        fromDate,
//                        toDate,
//                        fromMonth,
//                        fromYear,
//                        toMonth,
//                        toYear);

//                return Ok(new { success = true, data });
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(new { success = false, message = ex.Message });
//            }
//        }

//        [HttpPost]
//        public async Task<IActionResult> ExportAttendanceToExcel(
//            string employeeIds,
//            DateTime? fromDate,
//            DateTime? toDate,
//            int? fromMonth,
//            int? fromYear,
//            int? toMonth,
//            int? toYear)
//        {
//            try
//            {
//                var excelBytes = await _hrmAtdMachineDataService
//                    .ExportAttendanceToExcelAsync(
//                        employeeIds,
//                        fromDate,
//                        toDate,
//                        fromMonth,
//                        fromYear,
//                        toMonth,
//                        toYear);

//                var fileName = $"Attendance_Report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

//                return File(
//                    excelBytes,
//                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
//                    fileName
//                );
//            }
//            catch (InvalidOperationException ex)
//            {
//                return BadRequest(new { success = false, message = ex.Message });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { success = false, message = $"Error generating Excel: {ex.Message}" });
//            }
//        }

//    }
//}

using GCTL.Core.Data;
using GCTL.Data.Models;
using GCTL.Service.HrmAtdMachineDatas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GCTL.UI.Core.Controllers
{
    public class MachineDataController : BaseController
    {
        private readonly IHrmAtdMachineDataService _hrmAtdMachineDataService;
        private readonly IRepository<HrmAtdMachineData> _hrmAtdMachineDataRepository;
        private readonly IRepository<HrmEmployee> hrmEmployeeRepository;
        private readonly IRepository<HrmDefDepartment> hrmDepartmentRepository;
        private readonly IRepository<HrmEmployeeOfficialInfo> hrmEmployeeOfficialInfoRepository;

        public MachineDataController(
            IHrmAtdMachineDataService hrmAtdMachineDataService,
            IRepository<HrmAtdMachineData> hrmAtdMachineDataRepository,
            IRepository<HrmEmployee> hrmEmployeeRepository,
            IRepository<HrmDefDepartment> hrmDepartmentRepository,
            IRepository<HrmEmployeeOfficialInfo> hrmEmployeeOfficialInfoRepository
            )
        {
            _hrmAtdMachineDataService = hrmAtdMachineDataService;
            _hrmAtdMachineDataRepository = hrmAtdMachineDataRepository;
            this.hrmEmployeeRepository = hrmEmployeeRepository;
            this.hrmDepartmentRepository = hrmDepartmentRepository;
            this.hrmEmployeeOfficialInfoRepository = hrmEmployeeOfficialInfoRepository;
        }

        public IActionResult Index()
        {
            // Get all employees with their department information
            var employees = (from emp in hrmEmployeeRepository.All()
                             join empInfo in hrmEmployeeOfficialInfoRepository.All()
                                 on emp.EmployeeId equals empInfo.EmployeeId into empInfoJoin
                             from empInfo in empInfoJoin.DefaultIfEmpty()
                             join dept in hrmDepartmentRepository.All()
                                 on empInfo.DepartmentCode equals dept.DepartmentCode into deptJoin
                             from dept in deptJoin.DefaultIfEmpty()
                             select new
                             {
                                 EmployeeId = emp.EmployeeId,
                                 FullName = emp.FirstName + " " + emp.LastName + " (" + emp.EmployeeId + ")",
                                 DepartmentId = dept != null ? dept.DepartmentCode : "",
                                 DepartmentName = dept != null ? dept.DepartmentName : "No Department"
                             })
                             .OrderBy(x => x.DepartmentName)
                             .ThenBy(x => x.FullName)
                             .ToList();

            // Group employees by department
            var groupedEmployees = employees
                .GroupBy(x => x.DepartmentName)
                .OrderBy(g => g.Key == "No Department" ? 1 : 0) // Put "No Department" at the end
                .ThenBy(g => g.Key)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => new SelectListItem
                    {
                        Value = e.EmployeeId.ToString(),
                        Text = e.FullName
                    }).ToList()
                );

            ViewBag.EmployeeList = groupedEmployees;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetDataTableData()
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
            var sortColumn = Request.Form[$"columns[{sortColumnIndex}][data]"].FirstOrDefault();
            var sortDirection = Request.Form["order[0][dir]"].FirstOrDefault();

            // Calculate pagination values
            var pageSize = string.IsNullOrEmpty(length) ? 10 : Convert.ToInt32(length);
            var page = string.IsNullOrEmpty(start) ? 1 : (Convert.ToInt32(start) / pageSize) + 1;

            // Fetch data from service
            var result = await _hrmAtdMachineDataService.GetPaginatedDataAsync(searchValue, page, pageSize, sortColumn, sortDirection);

            // Prepare response for DataTables
            var response = new
            {
                draw = draw,
                recordsTotal = result.TotalRecords,
                recordsFiltered = result.TotalRecords,
                data = result.Data
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> GetEmployeeWiseAttendance(
            string employeeIds,
            DateTime? fromDate,
            DateTime? toDate,
            int? fromMonth,
            int? fromYear,
            int? toMonth,
            int? toYear)
        {
            try
            {
                var data = await _hrmAtdMachineDataService
                    .GetFilteredAttendanceAsync(
                        employeeIds,
                        fromDate,
                        toDate,
                        fromMonth,
                        fromYear,
                        toMonth,
                        toYear);

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportAttendanceToExcel(
            string employeeIds,
            DateTime? fromDate,
            DateTime? toDate,
            int? fromMonth,
            int? fromYear,
            int? toMonth,
            int? toYear)
        {
            try
            {
                var excelBytes = await _hrmAtdMachineDataService
                    .ExportAttendanceToExcelAsync(
                        employeeIds,
                        fromDate,
                        toDate,
                        fromMonth,
                        fromYear,
                        toMonth,
                        toYear);

                var fileName = $"Attendance_Report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(
                    excelBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error generating Excel: {ex.Message}" });
            }
        }
    }
}