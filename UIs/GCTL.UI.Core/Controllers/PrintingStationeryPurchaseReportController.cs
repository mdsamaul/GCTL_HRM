using GCTL.Core.ViewModels.PrintingStationeryPurchaseReport;
using GCTL.Service.PrintingStationeryPurchaseReportService;
using GCTL.UI.Core.ViewModels.PrintingStationeryPurchaseReport;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace GCTL.UI.Core.Controllers
{
    public class PrintingStationeryPurchaseReportController : BaseController
    {
        private readonly IPrintingStationeryPurchaseReportService printingStationeryPurchaseReportService;

        public PrintingStationeryPurchaseReportController(
            IPrintingStationeryPurchaseReportService printingStationeryPurchaseReportService
            )
        {
            this.printingStationeryPurchaseReportService = printingStationeryPurchaseReportService;
        }
        public async Task<IActionResult> Index()
        {

            var hasPermission = await printingStationeryPurchaseReportService.PagePermissionAsync(LoginInfo.AccessCode);

            if (!hasPermission)

            {

                return RedirectToAction("Login", "Accounts");

            }
            PrintingStationeryPurchaseReportViewModel model = new PrintingStationeryPurchaseReportViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> CategoryLoadList([FromBody] PrintingStationeryPurchaseReportFilterDto model)
        {
            var result = await printingStationeryPurchaseReportService.GetAllPROCPrintingAndStationeryReport(model);
            return Json(new { data = result });
        }
        [HttpPost]
        public async Task<IActionResult> GetFilteredDropdowns([FromBody] PrintingStationeryPurchaseReportFilterDto filter)
        {
            var result = await printingStationeryPurchaseReportService.GetFilteredDropdownsAsync(filter);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> ExportToExcel([FromBody] PrintingStationeryPurchaseReportFilterDto filter)
        {
            try
            {
                var data = await printingStationeryPurchaseReportService.GetAllPROCPrintingAndStationeryReport(filter);

                ExcelPackage.LicenseContext = LicenseContext.Commercial;
                using var package = new ExcelPackage();
                var ws = package.Workbook.Worksheets.Add("Report");

                int currentRow = 1;

                // Title
                ws.Cells[currentRow, 1, currentRow, 12].Merge = true;
                ws.Cells[currentRow, 1].Value = "PRINTING & STATIONERY REPORT";
                ws.Cells[currentRow, 1].Style.Font.Size = 18;
                ws.Cells[currentRow, 1].Style.Font.Bold = true;
                ws.Cells[currentRow, 1].Style.Font.Name = "Times New Roman";
                ws.Cells[currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                currentRow++;

                // Subtitle: Date range (if both are present)
                if (filter.FromDate.HasValue && filter.ToDate.HasValue)
                {
                    ws.Cells[currentRow, 1, currentRow, 12].Merge = true;
                    ws.Cells[currentRow, 1].Value = $"Date From: {filter.FromDate:dd/MM/yyyy} to {filter.ToDate:dd/MM/yyyy}";
                    ws.Cells[currentRow, 1].Style.Font.Size = 12;
                    ws.Cells[currentRow, 1].Style.Font.Bold = false;
                    ws.Cells[currentRow, 1].Style.Font.Name = "Times New Roman";
                    ws.Cells[currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    currentRow++;
                }

                // Header Row
                var headers = new[]
                {
            "Date", "Product", "Description", "Brand", "Model",
            "Size", "Warr.", "Period", "Qty", "Unit", "Unit Price", "Total Price (BDT)"
        };

                for (int i = 0; i < headers.Length; i++)
                {
                    ws.Cells[currentRow, i + 1].Value = headers[i];
                    ws.Cells[currentRow, i + 1].Style.Font.Bold = true;
                    ws.Cells[currentRow, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                // Data Rows
                decimal grandTotal = 0;
                for (int row = 0; row < data.Count; row++)
                {
                    var item = data[row];
                    int excelRow = currentRow + row + 1;

                    ws.Cells[excelRow, 1].Value = item.ReceiveDate.ToString("dd/MM/yyyy hh:mm:ss tt");
                    ws.Cells[excelRow, 2].Value = item.ProductName;
                    ws.Cells[excelRow, 3].Value = item.Description;
                    ws.Cells[excelRow, 4].Value = item.BrandName;
                    ws.Cells[excelRow, 5].Value = item.ModelName;
                    ws.Cells[excelRow, 6].Value = item.SizeName;
                    ws.Cells[excelRow, 7].Value = item.WarrantyPeriod;
                    ws.Cells[excelRow, 8].Value = item.PeriodName;
                    ws.Cells[excelRow, 9].Value = item.ReqQty;
                    ws.Cells[excelRow, 10].Value = item.UnitTypeName;
                    ws.Cells[excelRow, 11].Value = item.PurchaseCost;
                    ws.Cells[excelRow, 12].Value = item.TotalPrice;

                    grandTotal += item.TotalPrice;
                }

                int totalRow = currentRow + data.Count + 1;
                ws.Cells[totalRow, 11].Value = "Total";
                ws.Cells[totalRow, 11].Style.Font.Bold = true;
                ws.Cells[totalRow, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                ws.Cells[totalRow, 12].Value = grandTotal;
                ws.Cells[totalRow, 12].Style.Font.Bold = true;
                ws.Cells[totalRow, 12].Style.Numberformat.Format = "#,##0.00";


                var range = ws.Cells[currentRow, 1, totalRow, 12];
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                // Auto-fit all columns
                ws.Cells[ws.Dimension.Address].AutoFitColumns();
                var fileBytes = package.GetAsByteArray();
                var fileName = $"Printing_Stationery_Report.xlsx";

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
