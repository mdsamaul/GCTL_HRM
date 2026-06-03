using GCTL.Core.ViewModels.ProductIssueReport;
using GCTL.Service.ProductIssueReports;
using GCTL.UI.Core.ViewModels.ProductIssueReport;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace GCTL.UI.Core.Controllers
{
    public class ProductIssueReportController : BaseController
    {
        private readonly IProductIssueReportService productIssueReportService;

        public ProductIssueReportController(
            IProductIssueReportService productIssueReportService
            )
        {
            this.productIssueReportService = productIssueReportService;
        }
        public async Task<IActionResult> Index()
        {
            var hasPermission = await productIssueReportService.PagePermissionAsync(LoginInfo.AccessCode);

            if (!hasPermission)

            {

                return RedirectToAction("Login", "Accounts");

            }
            ProductIssueReportViewModel model = new ProductIssueReportViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ProductIssueReportLoadList([FromBody] ProductIssueReportFilterViewModel filter)
        {
            var result = await productIssueReportService.GetProductIssueReportAsync(filter);
            return Json(new { data = result });
        }
        [HttpPost]
        public async Task<IActionResult> GetFilteredDropdowns([FromBody] ProductIssueReportFilterViewModel filter)
        {
            var result = await productIssueReportService.GetProductIssueDropdownAsync(filter);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> ExportToExcel([FromBody] ProductIssueReportFilterViewModel filter)
        {
            try
            {
                var data = await productIssueReportService.GetProductIssueReportAsync(filter);

                ExcelPackage.LicenseContext = LicenseContext.Commercial;
                using var package = new ExcelPackage();
                var ws = package.Workbook.Worksheets.Add("Product Issue Report");

                int currentRow = 1;

                // Report Title
                ws.Cells[currentRow, 1, currentRow, 9].Merge = true;
                ws.Cells[currentRow, 1].Value = "Product Issue Report";
                ws.Cells[currentRow, 1].Style.Font.Size = 18;
                ws.Cells[currentRow, 1].Style.Font.Bold = true;
                ws.Cells[currentRow, 1].Style.Font.Name = "Times New Roman";
                ws.Cells[currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                currentRow++;

                // Subtitle: Date Range
                if (filter.FromDate.HasValue && filter.ToDate.HasValue)
                {
                    ws.Cells[currentRow, 1, currentRow, 9].Merge = true;
                    ws.Cells[currentRow, 1].Value = $"Date From: {filter.FromDate:dd/MM/yyyy} to {filter.ToDate:dd/MM/yyyy}";
                    ws.Cells[currentRow, 1].Style.Font.Size = 12;
                    ws.Cells[currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    currentRow++;
                }

                // Table Header
                var headers = new[]
                {
            "Product", "Brand", "Model", "Size", "Issue Qty",
            "Unit", "Department", "Employee", "Floor"
        };

                for (int i = 0; i < headers.Length; i++)
                {
                    ws.Cells[currentRow, i + 1].Value = headers[i];
                    ws.Cells[currentRow, i + 1].Style.Font.Bold = true;
                    ws.Cells[currentRow, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[currentRow, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                // Data Rows
                for (int i = 0; i < data.Count; i++)
                {
                    var row = currentRow + 1 + i;
                    var item = data[i];

                    ws.Cells[row, 1].Value = item.ProductName;
                    ws.Cells[row, 2].Value = item.BrandName;
                    ws.Cells[row, 3].Value = item.ModelName;
                    ws.Cells[row, 4].Value = item.SizeName;
                    ws.Cells[row, 5].Value = item.IssueQty;
                    ws.Cells[row, 6].Value = item.UnitTypeName;
                    ws.Cells[row, 7].Value = item.DepartmentName;
                    ws.Cells[row, 8].Value = item.FullName;
                    ws.Cells[row, 9].Value = item.FloorName;

                    for (int col = 1; col <= 9; col++)
                    {
                        ws.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        ws.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                }

                // Footer: Print DateTime
                var footerRow = currentRow + data.Count + 2;
                ws.Cells[footerRow, 1, footerRow, 9].Merge = true;
                ws.Cells[footerRow, 1].Value = $"Print DateTime: {DateTime.Now:dd/MM/yyyy hh:mm tt}";
                ws.Cells[footerRow, 1].Style.Font.Size = 10;
                ws.Cells[footerRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                // AutoFit Columns
                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                var fileBytes = package.GetAsByteArray();
                var fileName = $"Product_Issue_Report.xlsx";

                return File(fileBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception)
            {
                throw;
            }
        }



    }
}
