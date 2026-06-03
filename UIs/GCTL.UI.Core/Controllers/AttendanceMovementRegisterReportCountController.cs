using GCTL.Core.Data;
using GCTL.Core.ViewModels.AttendanceMovementRegisterReportCount;
using GCTL.Core.ViewModels.AttendanceMovementRegisterReportDto;
using GCTL.Data.Models;
using GCTL.Service.AttendanceMovementRegisterReportCountService;
using GCTL.Service.AttendanceMovementRegisterReportService;
using GCTL.UI.Core.ViewModels.AttendanceMovementRegisterReportContViewModel;
using GCTL.UI.Core.ViewModels.AttendanceMovementRegisterReportViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Globalization;

namespace GCTL.UI.Core.Controllers
{
    public class AttendanceMovementRegisterReportCountController : BaseController
    {
        private readonly IAttendanceMovementRegisterReportCountService attendanceCountService;
        private readonly IRepository<HrmPayMonth> monthRepo;

        public AttendanceMovementRegisterReportCountController(
            IAttendanceMovementRegisterReportCountService attendanceCountService,
            IRepository<HrmPayMonth> monthRepo
            )
        {
            this.attendanceCountService = attendanceCountService;
            this.monthRepo = monthRepo;
        }

        private void InjectLoginInfo(AttendanceMovementRegisterReportCountFilterData req)
        {
            req.AccessCode = LoginInfo.AccessCode;
            req.EmployeeId = LoginInfo.EmployeeId;
        }

        public async Task<IActionResult> Index()
        {
            var hasPermission = await attendanceCountService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }
            ViewBag.MonthList = new SelectList(monthRepo.All().Select(e => new { id = e.MonthId, name = e.MonthName }).ToList(), "id", "name");
            ViewBag.AccessCode = LoginInfo.AccessCode;
            AttendanceMovementRegisterReportCountViewModel model = new AttendanceMovementRegisterReportCountViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetAttendanceMachineData([FromBody] AttendanceMovementRegisterReportCountFilterData filter)
        {
            try
            {
                InjectLoginInfo(filter);
                var baseUrl = $"{Request.Scheme}://{Request.Host.Value}";
                var result = await attendanceCountService.GetAttendanceMachineDataAsync(filter, baseUrl, LoginInfo);
                if (result != null)
                {
                    return Json(new { isSuccess = true, message = "successed data load", data = result });
                }
                return Json(new { isSuccess = false, message = "Data load Failed" });

            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = "Data load Failed" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetFilters([FromBody] AttendanceMovementRegisterReportCountFilterData filter)
        {
            try
            {
                var result = await attendanceCountService.GetAttendanceMachineDataFiltersAsync(filter);
                if (result != null)
                {
                    return Json(new { isSuccess = true, message = "successed data load", data = result });
                }
                return Json(new { isSuccess = false, message = "Data load Failed" });

            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = "Data load Failed" });
            }
        }


        [HttpPost]
        public async Task<IActionResult> ExcelDownload([FromBody] AttendanceMovementRegisterReportCountFilterData filter)
        {
            try
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host.Value}";
                var result = await attendanceCountService.GetAttendanceMachineDataAsync(filter, baseUrl, LoginInfo);

                if (result == null || !result.Any())
                {
                    return Json(new { isSuccess = false, message = "No data found to export." });
                }

                return ExportAttendanceMovementRegisterExcelAsync(result.ToList(), filter);
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = "Data load failed: " + ex.Message });
            }
        }

        public IActionResult ExportAttendanceMovementRegisterExcelAsync(List<DepartmentAndDateGroupedData> groupedData, AttendanceMovementRegisterReportCountFilterData filter)
        {
            try
            {
                if (groupedData == null || !groupedData.Any())
                {
                    return BadRequest(new { success = false, message = "No data to export." });
                }

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var package = new ExcelPackage();
                var ws = package.Workbook.Worksheets.Add("Movement Register");

                int row = 1;

                // Company Name
                var firstEmp = groupedData.First().Employees.First();
                string companyName = firstEmp.CompanyName ?? "Company Name";
                string reportTitle = "Attendance Movement Register Report";
                //string dateRange = $"Date: {filter.FromDate:dd/MM/yyyy} - {filter.ToDate:dd/MM/yyyy}";
                string dateRange = string.Empty;

                if (filter.FromDate != null && filter.ToDate != null)
                {
                    if (filter.FromDate == filter.ToDate)
                    {
                        dateRange = $"{filter.FromDate:dd/MM/yyyy}";
                    }
                    else
                    {
                        dateRange = $"Date: {filter.FromDate:dd/MM/yyyy} - {filter.ToDate:dd/MM/yyyy}";
                    }
                }
                else if (filter.FromDate == null && filter.ToDate == null
                         && filter.MonthIDs != null && filter.MonthIDs.Any()
                         && filter.YearIDs != null && filter.YearIDs.Any())
                {
                    // Month names mapping
                    string[] monthNames = new[]
                    {
        "January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    };

                    int monthId = filter.MonthIDs.First();
                    int yearId = filter.YearIDs.First();

                    dateRange = $"{monthNames[monthId - 1]}, {yearId}";
                }
                else if (filter.FromDate == null && filter.ToDate == null
                         && filter.YearIDs != null && filter.YearIDs.Any())
                {
                    int yearId = filter.YearIDs.First();
                    dateRange = $"{yearId}";
                }

                ws.Cells[row, 1, row, 8].Merge = true;
                ws.Cells[row, 1].Value = companyName;
                ws.Cells[row, 1].Style.Font.Bold = true;
                ws.Cells[row, 1].Style.Font.Size = 16;
                ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                row++;

                ws.Cells[row, 1, row, 8].Merge = true;
                ws.Cells[row, 1].Value = reportTitle;
                ws.Cells[row, 1].Style.Font.Bold = true;
                ws.Cells[row, 1].Style.Font.Size = 14;
                ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                row++;

                ws.Cells[row, 1, row, 8].Merge = true;
                ws.Cells[row, 1].Value = dateRange;
                ws.Cells[row, 1].Style.Font.Size = 12;
                ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                row += 1;

                // Process Department Wise Groups
                foreach (var dept in groupedData)
                {
                    // Department Header
                    ws.Cells[row, 1, row, 8].Merge = true;
                    ws.Cells[row, 1].Value = $"Department : {dept.DepartmentName}";
                    ws.Cells[row, 1].Style.Font.Bold = true;
                    ws.Cells[row, 1].Style.Font.Size = 12;
                    row += 1;

                    // Table Header
                    string[] headers = { "Employee ID", "Name", "Designation", "Branch", "Date", "Time", "Machine Id", "Location" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        ws.Cells[row, i + 1].Value = headers[i];
                        ws.Cells[row, i + 1].Style.Font.Bold = true;
                        ws.Cells[row, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[row, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        ws.Cells[row, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[row, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }
                    row++;

                    //// Employee Rows

                    foreach (var emp in dept.Employees)
                    {
                        ws.Cells[row, 1].Value = emp.EmployeeID;
                        ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        ws.Cells[row, 2].Value = emp.FullName;

                        ws.Cells[row, 3].Value = emp.DesignationName;

                        ws.Cells[row, 4].Value = emp.BranchName;
                        ws.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        // Date Format
                        if (emp.Date != null)
                        {
                            ws.Cells[row, 5].Value = ((DateTime)emp.Date).ToString("dd/MM/yyyy");
                            ws.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        }

                        // Time Format
                        if (emp.Time != null)
                        {
                            DateTime dt = (DateTime)emp.Time;
                            ws.Cells[row, 6].Value = dt.ToString("hh:mm:ss tt");
                            ws.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        }

                        ws.Cells[row, 7].Value = emp.MachineId;
                        ws.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        // Google Map Link
                        if (!string.IsNullOrEmpty(emp.Latitude) && !string.IsNullOrEmpty(emp.Longitude))
                        {
                            string url = $"https://www.google.com/maps?q={emp.Latitude},{emp.Longitude}";
                            var cell = ws.Cells[row, 8];
                            cell.Hyperlink = new Uri(url);
                            cell.Value = "View Location";
                            cell.Style.Font.UnderLine = true;
                            cell.Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                        }
                        else
                        {
                            ws.Cells[row, 8].Value = "";
                        }

                        // Borders
                        for (int i = 1; i <= 8; i++)
                        {
                            ws.Cells[row, i].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        }

                        row++;
                    }

                    row += 2;
                }

                ws.Cells.AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string fileName = $"MovementRegister.xlsx";

                return File(stream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ViewDetails(string employeeId, string date)
        {
            if (string.IsNullOrWhiteSpace(employeeId))
                return BadRequest("Employee ID is required");

            if (string.IsNullOrWhiteSpace(date))
                return BadRequest("Date is required");

            if (!DateTime.TryParseExact(date, "dd-MM-yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                return BadRequest("Invalid date format. Expected dd-MM-yyyy.");
            }

            var requestDto = new EmployeeMovementRequestDto
            {
                EmployeeId = employeeId,
                Date = parsedDate   // ✅ now a DateTime
            };

            var pdfBytes = await attendanceCountService.GetEmployeeMovementPdfAsync(requestDto);

            if (pdfBytes == null || pdfBytes.Length == 0)
                return NotFound("No movement data found for this employee and date.");

            var fileName = $"Movement_{employeeId}_{parsedDate:yyyyMMdd}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }


        //   [HttpGet]
        //   [AllowAnonymous]
        //   public async Task<IActionResult> ViewDetails(string employeeId, string date)
        //   {
        //       if (string.IsNullOrWhiteSpace(employeeId))
        //           return BadRequest("Employee ID is required");

        //       if (string.IsNullOrWhiteSpace(date))
        //           return BadRequest("Date is required");

        //       if (!DateTime.TryParseExact(date, "dd-MM-yyyy",
        //CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
        //       {
        //           return BadRequest("Invalid date format. Expected dd-MM-yyyy.");
        //       }

        //       var requestDto = new EmployeeMovementRequestDto
        //       {
        //           EmployeeId = employeeId,
        //           Date = parsedDate.ToString("dd/MM/yyyy")
        //       };

        //       var pdfBytes = await attendanceCountService.GetEmployeeMovementPdfAsync(requestDto);

        //       if (pdfBytes == null || pdfBytes.Length == 0)
        //           return NotFound("No movement data found for this employee and date.");

        //       var fileName = $"Movement_{employeeId}_{parsedDate:yyyyMMdd}.pdf";
        //       return File(pdfBytes, "application/pdf", fileName);
        //   }



    }
}
