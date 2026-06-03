using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.HRM_EmployeeWeekendDeclaration;
using GCTL.Core.ViewModels.PFAssignEntry;
using GCTL.Core.ViewModels.RosterScheduleReport;
using GCTL.Service.EmployeeWeekendDeclarationReport;
using GCTL.Service.PFAssignEntry;
using GCTL.Service.PFAssignEntryReport;
using GCTL.UI.Core.ViewModels.PFAssignEntry;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace GCTL.UI.Core.Controllers
{
    public class PFAssignEntryReportController : BaseController
    {
        private readonly IPFAssignEntryReportServices pFAssignEntryReportServices;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PFAssignEntryReportController(IPFAssignEntryReportServices pFAssignEntryReportServices,
            IWebHostEnvironment webHostEnvironment)
        {
            this.pFAssignEntryReportServices = pFAssignEntryReportServices;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var hasPermission = await pFAssignEntryReportServices.PagePermissionAsync(LoginInfo.AccessCode);

            if (!hasPermission)

            {

                return RedirectToAction("Login", "Accounts");

            }
            PFAssignEntryViewModel model = new PFAssignEntryViewModel()
            {
                PageUrl = Url.Action(nameof(Index)),
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> getAllFilterEmp([FromBody] PFAssignEntryFilterDto filterDto)
        {
            var result = await pFAssignEntryReportServices.GetPFBaseAndFilteredDataAsync(filterDto);
            if (result == null)
            {
                return Json(new { isSuccess = false, message = "Data Load Failed" });
            }
            return Json(new { isSuccess = true, message = "Successed data load", data = result });
        }

        [HttpPost]
        public async Task<IActionResult> getAllPdfFilterEmp([FromBody] PFAssignEntryFilterDto filterDto)
        {
            //filterDto.ToAudit(LoginInfo);
            filterDto.ToAudit(LoginInfo);

            var result = await pFAssignEntryReportServices.GetPFDataPdfAsync(filterDto);
            if (result != null)
            {
                return Json(new { isSuccess = true, message = "successed data load", data = result });
            }
            return Json(new { isSuccess = false, message = "Data load Failed" });
        }


        [HttpPost]
        public async Task<IActionResult> DownloadExcel([FromBody] List<PFAssignEntryFilterResultDto> employees)
        {
            if (employees == null || employees.Count == 0)
                return BadRequest(new { message = "No data found to generate Excel." });

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("PF Assign Report");

                var headers = new[] { "SN", "Employee ID", "Name", "Designation", "Branch", "PF Approval", "Effective Date", "Remarks" };

                string companyName = employees.FirstOrDefault()?.Company ?? "Company Name";
                string reportTitle = "PF Assign Report";
                string userName = employees.FirstOrDefault()?.Luser ?? "";

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

                // Company name row  height 
                worksheet.Row(1).Height = 35;
                //worksheet.Row(2).Height = 25;

                // Add Company Logo in the same row as company name
                try
                {
                    // wwwroot folder to image path
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "DP_logo.png");

                    // Check if image exists
                    if (System.IO.File.Exists(imagePath))
                    {
                        // Image add in company name on row 
                        var image = worksheet.Drawings.AddPicture("CompanyLogo", new FileInfo(imagePath));

                        // Image position - Row 1 on left side
                        image.SetPosition(0, 2, 0, 2); 
                        image.SetSize(150, 50); 

                        // Alternative: Right side Images
                        // image.SetPosition(0, 2, headers.Length - 1, 20);
                    }
                }
                catch (Exception ex)
                {
                  
                    Console.WriteLine($"Image loading error: {ex.Message}");
                }

                int rowIndex = 4;

                // Group by department
                var departmentGroups = employees.GroupBy(e => e.Department ?? "Unknown");

                foreach (var dept in departmentGroups)
                {
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
                        worksheet.Cells[rowIndex, 4].Value = emp.Designation ?? "";
                        worksheet.Cells[rowIndex, 5].Value = emp.Branch ?? "";
                        worksheet.Cells[rowIndex, 6].Value = emp.PFApprove ?? "";
                        worksheet.Cells[rowIndex, 7].Value = emp.ShowDate ?? "";
                        //worksheet.Cells[rowIndex, 8].Value = emp.dayName ?? "";
                        worksheet.Cells[rowIndex, 8].Value = emp.Remarks ?? "";

                        for (int i = 1; i <= headers.Length; i++)
                        {
                            var cell = worksheet.Cells[rowIndex, i];

                         
                            if (i == 3 || i == 4 || i == 5 || i == 9) 
                            {
                                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                cell.Style.Indent = (int).5; 
                            }
                            else
                            {
                                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            }

                            cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            // Alternate row coloring
                            if (sn % 2 == 0)
                            {
                                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                            }
                        }

                        rowIndex++;
                    }

                    rowIndex++;
                }

                // Footer
                //int totalColumns = headers.Length;
                //int midColumn = totalColumns / 2;
                //worksheet.Cells[rowIndex, 1, rowIndex, midColumn].Merge = true;
                //worksheet.Cells[rowIndex, 1].Value = $"Downloaded At: {DateTime.Now.ToString("dd/MM/yyyy | hh:mm:ss tt")}";

                //worksheet.Cells[rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                //worksheet.Cells[rowIndex, 1].Style.Font.Italic = true;

                //worksheet.Cells[rowIndex, midColumn + 1, rowIndex, totalColumns].Merge = true;
                //worksheet.Cells[rowIndex, midColumn + 1].Value = $"Downloaded By: {userName}";
                //worksheet.Cells[rowIndex, midColumn + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                //worksheet.Cells[rowIndex, midColumn + 1].Style.Font.Italic = true;

                rowIndex++;

                // Auto fit columns
                worksheet.Cells.AutoFitColumns();

                // Stream create করুন
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                return File(stream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "PF_Assign_Report.xlsx");
            }
        }       
    }
}
