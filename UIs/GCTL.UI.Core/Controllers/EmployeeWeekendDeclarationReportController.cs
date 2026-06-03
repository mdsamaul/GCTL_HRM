using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.HRM_EmployeeWeekendDeclaration;
using GCTL.Core.ViewModels.RosterScheduleReport;
using GCTL.Service.EmployeeWeekendDeclaration;
using GCTL.Service.EmployeeWeekendDeclarationReport;
using GCTL.Service.RosterScheduleReport;
using GCTL.UI.Core.ViewModels.HRM_EmployeeWeekendDeclaration;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using static QuestPDF.Helpers.Colors;
namespace GCTL.UI.Core.Controllers
{
    public class EmployeeWeekendDeclarationReportController : BaseController
    {
        private readonly IEmployeeWeekendDeclarationReportServices employeeWeekendDeclarationReportServices;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmployeeWeekendDeclarationReportController( IEmployeeWeekendDeclarationReportServices employeeWeekendDeclarationReportServices,
            IWebHostEnvironment webHostEnvironment
            )
        {
            this.employeeWeekendDeclarationReportServices = employeeWeekendDeclarationReportServices;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> IndexAsync()
        {     

            var hasPermission = await employeeWeekendDeclarationReportServices.PagePermissionAsync(LoginInfo.AccessCode);

            if (!hasPermission)

            {

                return RedirectToAction("Login", "Accounts");

            }
            HRM_EmployeeWeekendDeclarationViewModel model = new HRM_EmployeeWeekendDeclarationViewModel()
            {
                PageUrl = Url.Action(nameof(IndexAsync)),
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> getAllFilterEmp([FromBody] EmployeeFilterDto filterDto)
        {
            var result = await employeeWeekendDeclarationReportServices.GetRosterDataAsync(filterDto);
            if (result == null) {
                return Json(new {isSuccess = false, message = "Data Load Failed"});
            }
            return Json(new {isSuccess= true, message = "Successed data load", data = result});
        }

        [HttpPost]
        public async Task<IActionResult> getAllPdfFilterEmp([FromBody] EmployeeFilterDto filterDto)
        {
            //filterDto.ToAudit(LoginInfo);
            filterDto.ToAudit(LoginInfo);
         
            var result = await employeeWeekendDeclarationReportServices.GetRosterDataPdfAsync(filterDto);
            if (result != null)
            {
                return Json(new { isSuccess = true, message = "successed data load", data = result });
            }
            return Json(new { isSuccess = false, message = "Data load Failed" });
        }
        [HttpPost]
        public async Task<IActionResult> DownloadExcel([FromBody] List<RosterReportFilterResultDto> employees)
        {
            if (employees == null || employees.Count == 0)
                return BadRequest(new { message = "No data found to generate Excel." });

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Weekend Declaration Report");

                var headers = new[] { "SN", "Employee ID", "Name", "Designation", "Branch", "Date", "Day", "Remarks" };

                string companyName = employees.FirstOrDefault()?.CompanyName ?? "Company Name";
                string reportTitle = "Employee Weekend Declaration Report";
                string fromDate = employees.FirstOrDefault()?.FromDate ?? "";
                string toDate = employees.FirstOrDefault()?.ToDate ?? "";
                string userName = employees.FirstOrDefault()?.Luser ?? "";
                string dateRange = $"Date: {fromDate} - {toDate}";

                // Title Rows
                worksheet.Cells[1, 1].Value = companyName;
                worksheet.Cells[1, 1, 1, headers.Length].Merge = true;
                worksheet.Cells[1, 1].Style.Font.Size = 14;
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[2, 1].Value = reportTitle;
                worksheet.Cells[2, 1, 2, headers.Length].Merge = true;
                worksheet.Cells[2, 1].Style.Font.Size = 12;
                worksheet.Cells[2, 1].Style.Font.Bold = true;
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells[3, 1].Value = dateRange;
                worksheet.Cells[3, 1, 3, headers.Length].Merge = true;
                worksheet.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Row(1).Height = 35;
                try
                {
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "DP_logo.png");
                    if (System.IO.File.Exists(imagePath))
                    {
                        var image = worksheet.Drawings.AddPicture("CompanyLogo", new FileInfo(imagePath));
                        image.SetPosition(0, 2, 0, 2);
                        image.SetSize(150, 50);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Image loading error: {ex.Message}");
                }


                int rowIndex = 4;

                // Group by department
                var departmentGroups = employees.GroupBy(e => e.DepartmentName ?? "Unknown");

                foreach (var dept in departmentGroups)
                {
                    // Department title
                    // Department header row
                    worksheet.Cells[rowIndex, 1, rowIndex, headers.Length].Merge = true;
                    worksheet.Cells[rowIndex, 1].Value = $"Department: {dept.Key}";

                    // Styling entire merged range
                    var deptRange = worksheet.Cells[rowIndex, 1, rowIndex, headers.Length];
                    deptRange.Style.Font.Bold = true;
                    deptRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    deptRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);

                    // Apply border to all sides of merged range
                    deptRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    deptRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    deptRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    deptRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    rowIndex++;


                    // Header row
                    for (int i = 0; i < headers.Length; i++)
                    {
                        var cell = worksheet.Cells[rowIndex, i + 1];
                        cell.Value = headers[i];
                        cell.Style.Font.Bold = true;
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                        cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }
                    rowIndex++;

                    // Employee rows
                    int sn = 1;
                    foreach (var emp in dept)
                    {
                        worksheet.Cells[rowIndex, 1].Value = sn++;
                        worksheet.Cells[rowIndex, 2].Value = emp.Code ?? "";
                        worksheet.Cells[rowIndex, 3].Value = emp.Name ?? "";
                        worksheet.Cells[rowIndex, 4].Value = emp.DesignationName ?? "";
                        worksheet.Cells[rowIndex, 5].Value = emp.BranchName ?? "";
                        worksheet.Cells[rowIndex, 6].Value = emp.ShowDate ?? "";
                        worksheet.Cells[rowIndex, 7].Value = emp.DayName ?? "";
                        worksheet.Cells[rowIndex, 8].Value = emp.Remark ?? "";

                        for (int i = 1; i <= headers.Length; i++)
                        {
                            worksheet.Cells[rowIndex, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIndex, i].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        }

                        rowIndex++;
                    }

                    rowIndex++;
                }

                //// Footer
                //int totalColumns = headers.Length;
                //int midColumn = totalColumns / 2;
                //worksheet.Cells[rowIndex, 1, rowIndex, midColumn].Merge = true;
                //worksheet.Cells[rowIndex, 1].Value = $"Printed At: {DateTime.Now}";
                //worksheet.Cells[rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                //worksheet.Cells[rowIndex, 1].Style.Font.Italic = true;

                //worksheet.Cells[rowIndex, midColumn + 1, rowIndex, totalColumns].Merge = true;
                //worksheet.Cells[rowIndex, midColumn + 1].Value = $"Printed By: {userName}";
                //worksheet.Cells[rowIndex, midColumn + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                //worksheet.Cells[rowIndex, midColumn + 1].Style.Font.Italic = true;

                rowIndex++;


                worksheet.Cells.AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                return File(stream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "EmployeeWeekendDeclaration.xlsx");
            }
        }

    }
}
