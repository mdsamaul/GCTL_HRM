using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.RosterScheduleEntry;
using GCTL.Core.ViewModels.RosterScheduleReport;
using GCTL.Service.ManualEntryApprovalService;
using GCTL.Service.RosterScheduleEntry;
using GCTL.Service.RosterScheduleReport;
using GCTL.UI.Core.ViewModels.RosterScheduleEntry;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Security.Policy;
namespace GCTL.UI.Core.Controllers
{
    public class RosterScheduleReportController : BaseController
    {
        private readonly IRosterScheduleReportServices rosterScheduleReportServices;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RosterScheduleReportController(IRosterScheduleReportServices rosterScheduleReportServices,
            IWebHostEnvironment webHostEnvironment
            )
        {
            this.rosterScheduleReportServices = rosterScheduleReportServices;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var hasPermission = await rosterScheduleReportServices.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }
            RosterScheduleEntryViewModel model = new RosterScheduleEntryViewModel()
            {
                PageUrl = Url.Action(nameof(IndexAsync))
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> getAllFilterEmp([FromBody] RosterReportFilterDto filterDto)
        {
            var result = await rosterScheduleReportServices.GetRosterDataAsync(filterDto);
            if (result != null)
            {
                return Json(new { isSuccess = true, message = "successed data load", data = result });
            }
            return Json(new { isSuccess = false, message = "Data load Failed" });
        }
        [HttpPost]
        public async Task<IActionResult> getAllPdfFilterEmp([FromBody] RosterReportFilterDto filterDto)
        {
            //filterDto.ToAudit(LoginInfo);
            filterDto.ToAudit(LoginInfo);
            var result = await rosterScheduleReportServices.GetRosterDataPdfAsync(filterDto);
            if (result != null)
            {
                return Json(new { isSuccess = true, message = "successed data load", data = result });
            }
            return Json(new { isSuccess = false, message = "Data load Failed" });
        }

        [HttpPost]
        public async Task<IActionResult> DownloadExcel([FromBody] List<RosterReportFilterResultDto> rosterData)
        {
            if (rosterData == null || rosterData.Count == 0)
            {
                return BadRequest(new { message = "No data found to generate Excel." });
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Roster Report");

                var headers = new string[]
                {
            "SN", "Code", "Name", "Designation", "Branch", "Date", "Day", "Shift",
            "Remark", "Approval Status", "Approved By", "App. Datetime"
                };

                var company = rosterData.FirstOrDefault()?.CompanyName ?? "Company Name";
                var title = "Roster Schedule Report";
                var from = rosterData.FirstOrDefault()?.FromDate?.ToString() ?? "";
                var to = rosterData.FirstOrDefault()?.ToDate?.ToString() ?? "";
                var fromDate = string.IsNullOrWhiteSpace(from) && string.IsNullOrWhiteSpace(to)
                    ? "Date"
                    : $"Date: {from}-{to}";

                worksheet.Cells[1, 1].Value = company;
                worksheet.Cells[1, 1, 1, headers.Length].Merge = true;
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 1].Style.Font.Size = 16;
                worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[2, 1].Value = title;
                worksheet.Cells[2, 1, 2, headers.Length].Merge = true;
                worksheet.Cells[2, 1].Style.Font.Bold = true;
                worksheet.Cells[2, 1].Style.Font.Size = 12;
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells[3, 1].Value = fromDate;
                worksheet.Cells[3, 1, 3, headers.Length].Merge = true;
                worksheet.Cells[3, 1].Style.Font.Size = 11;
                worksheet.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                // Company name row  height 
                worksheet.Row(1).Height = 35;
                //worksheet.Row(2).Height = 25;

                try
                {
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "DP_logo.png");
                    if (System.IO.File.Exists(imagePath))
                    {
                        var image = worksheet.Drawings.AddPicture("CompanyLogo", new FileInfo(imagePath));

                        image.SetPosition(0, 2, 0, 2);
                        image.SetSize(150, 50);
                    }
                }catch(Exception ex)
                {
                    Console.WriteLine($"Image loading error: {ex.Message}");
                }
                int rowIndex = 4;

                var departmentGroups = rosterData.GroupBy(emp => emp.DepartmentName ?? "Unknown");

                foreach (var deptGroup in departmentGroups)
                {
                    worksheet.Cells[rowIndex, 1].Value = "Department: " + deptGroup.Key;
                    worksheet.Cells[rowIndex, 1, rowIndex, headers.Length].Merge = true;
                    worksheet.Cells[rowIndex, 1].Style.Font.Bold = true;
                    worksheet.Cells[rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[rowIndex, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[rowIndex, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);

                    rowIndex++;

                    for (int i = 0; i < headers.Length; i++)
                    {
                        var cell = worksheet.Cells[rowIndex, i + 1];
                        cell.Value = headers[i];
                        cell.Style.Font.Bold = true;
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                        // Add border
                        cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }

                    rowIndex++;

                    int sn = 1;
                    foreach (var emp in deptGroup)
                    {
                        worksheet.Cells[rowIndex, 1].Value = sn++;
                        worksheet.Cells[rowIndex, 2].Value = emp.Code ?? "";
                        worksheet.Cells[rowIndex, 3].Value = emp.Name ?? "";
                        worksheet.Cells[rowIndex, 4].Value = emp.DesignationName ?? "";
                        worksheet.Cells[rowIndex, 5].Value = emp.BranchName ?? "";
                        worksheet.Cells[rowIndex, 6].Value = emp.ShowDate ?? "";
                        worksheet.Cells[rowIndex, 7].Value = emp.DayName ?? "";
                        worksheet.Cells[rowIndex, 8].Value = emp.ShiftName ?? "";
                        worksheet.Cells[rowIndex, 9].Value = emp.Remark ?? "";
                        worksheet.Cells[rowIndex, 10].Value = emp.ApprovalStatus ?? "";
                        worksheet.Cells[rowIndex, 11].Value = emp.ApprovedBy ?? "";
                        worksheet.Cells[rowIndex, 12].Value = emp.ShowApprovalDatetime ?? "";

                        for (int i = 1; i <= headers.Length; i++)
                        {
                            var cell = worksheet.Cells[rowIndex, i];
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            // Add border
                            cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        }

                        rowIndex++;
                    }

                    worksheet.Cells[rowIndex, 1, rowIndex, headers.Length].Merge = true;
                    rowIndex++;
                }

                // Apply border to header, titles, and merged cells
                for (int r = 1; r < rowIndex; r++)
                {
                    for (int c = 1; c <= headers.Length; c++)
                    {
                        var cell = worksheet.Cells[r, c];
                        cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        cell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                }

                worksheet.Cells.AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "RosterScheduleReport.xlsx");
            }
        }


    }
}
