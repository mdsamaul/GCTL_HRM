using GCTL.Core.ViewModels.ProductStockHistoryReport;
using GCTL.Service.ProductStockHistoryReport;
using GCTL.UI.Core.ViewModels.ProductStockHistoryReport;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace GCTL.UI.Core.Controllers
{
    public class ProductStockHistoryReportController : BaseController
    {
        private readonly IProductStockHistoryReportService productStockHistoryReportService;

        public ProductStockHistoryReportController(
            IProductStockHistoryReportService productStockHistoryReportService
            )
        {
            this.productStockHistoryReportService = productStockHistoryReportService;
        }
        public async Task<IActionResult> Index()
        {
            var hasPermission = await productStockHistoryReportService.PagePermissionAsync(LoginInfo.AccessCode);

            if (!hasPermission)

            {

                return RedirectToAction("Login", "Accounts");

            }
            ProductStockHistoryReportViewModel model = new ProductStockHistoryReportViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> GetFilteredStock([FromBody] StockReportFilterViewModel filter)
        {
            var data = await productStockHistoryReportService.GetStockReportAsync(filter);
            return Json(new { data = data });
        }
        [HttpPost]
        public async Task<IActionResult> GetFilteredDropdowns([FromBody] StockReportFilterViewModel filter)
        {
            var result = await productStockHistoryReportService.GetFilteredDropdownAsync(filter);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> ExportToExcel([FromBody] StockReportFilterViewModel filter)
        {
            try
            {
                var data = await productStockHistoryReportService.GetStockReportAsync(filter);
                if (data.Count == 0)
                {
                    return Json(new { isIsuccess = false });
                }
                ExcelPackage.LicenseContext = LicenseContext.Commercial;
                using var package = new ExcelPackage();
                var ws = package.Workbook.Worksheets.Add("Product Stock History Report");

                int currentRow = 1;

                // Title with border
                ws.Cells[currentRow, 1, currentRow, 14].Merge = true;
                ws.Cells[currentRow, 1].Value = "Product Stock History Report";
                ws.Cells[currentRow, 1].Style.Font.Size = 18;
                ws.Cells[currentRow, 1].Style.Font.Bold = true;
                ws.Cells[currentRow, 1].Style.Font.Name = "Times New Roman";
                ws.Cells[currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[currentRow, 1, currentRow, 14].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells[currentRow, 1, currentRow, 14].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                ws.Cells[currentRow, 1, currentRow, 14].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells[currentRow, 1, currentRow, 14].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                currentRow++;

                // Subtitle (Date Range) with border
                if (filter.FromDate.HasValue && filter.ToDate.HasValue)
                {
                    ws.Cells[currentRow, 1, currentRow, 14].Merge = true;
                    ws.Cells[currentRow, 1].Value = $"Date From: {filter.FromDate:dd/MM/yyyy} to {filter.ToDate:dd/MM/yyyy}";
                    ws.Cells[currentRow, 1].Style.Font.Size = 12;
                    ws.Cells[currentRow, 1].Style.Font.Bold = false;
                    ws.Cells[currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[currentRow, 1, currentRow, 14].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[currentRow, 1, currentRow, 14].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    ws.Cells[currentRow, 1, currentRow, 14].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[currentRow, 1, currentRow, 14].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    currentRow++;
                }

                // Headers
                var headers = new[]
                {
            "Product Code", "Product", "Description", "Brand", "Model", "Size",
            "Unit", "Unit Price", "Opening Qty", "Received Qty",
            "Stock Qty", "Issued Qty", "Balance Qty", "Stock Value (BDT)"
        };

                for (int i = 0; i < headers.Length; i++)
                {
                    ws.Cells[currentRow, i + 1].Value = headers[i];
                    ws.Cells[currentRow, i + 1].Style.Font.Bold = true;
                    ws.Cells[currentRow, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[currentRow, i + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells[currentRow, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[currentRow, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    ws.Cells[currentRow, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                // Data Rows
                decimal totalStockValue = 0;
                for (int i = 0; i < data.Count; i++)
                {
                    var row = currentRow + 1 + i;
                    var item = data[i];

                    ws.Cells[row, 1].Value = item.ProductCode;
                    ws.Cells[row, 2].Value = item.ProductName;
                    ws.Cells[row, 3].Value = item.Description;
                    ws.Cells[row, 4].Value = item.BrandName;
                    ws.Cells[row, 5].Value = item.ModelName;
                    ws.Cells[row, 6].Value = item.SizeName;
                    ws.Cells[row, 7].Value = item.UnitName;
                    ws.Cells[row, 8].Value = item.UnitPrice;
                    ws.Cells[row, 9].Value = decimal.TryParse(item.OpeningQty, out var openingQty) ? (openingQty % 1 == 0 ? (int)openingQty : openingQty) : (object)null;
                    ws.Cells[row, 10].Value = decimal.TryParse(item.ReceivedQty, out var receivedQty) ? (receivedQty % 1 == 0 ? (int)receivedQty : receivedQty) : (object)null;
                    ws.Cells[row, 11].Value = decimal.TryParse(item.StockQty, out var stockQty) ? (stockQty % 1 == 0 ? (int)stockQty : stockQty) : (object)null;
                    ws.Cells[row, 12].Value = decimal.TryParse(item.IssuedQty, out var issuedQty) ? (issuedQty % 1 == 0 ? (int)issuedQty : issuedQty) : (object)null;
                    ws.Cells[row, 13].Value = decimal.TryParse(item.BalanceQty, out var balanceQty) ? (balanceQty % 1 == 0 ? (int)balanceQty : balanceQty) : (object)null;


                    ws.Cells[row, 14].Value = item.StockValue;

                    totalStockValue += (decimal)item.StockValue;

                    for (int col = 1; col < 14; col++)
                    {
                        ws.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        ws.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[row, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }

                    ws.Cells[row, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    ws.Cells[row, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    ws.Cells[row, 14].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }

                // Total Row
                var totalRow = currentRow + data.Count + 1;
                ws.Cells[totalRow, 1, totalRow, 13].Merge = true;
                ws.Cells[totalRow, 1].Value = "Total:";
                ws.Cells[totalRow, 1].Style.Font.Bold = true;
                ws.Cells[totalRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                ws.Cells[totalRow, 1, totalRow, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                ws.Cells[totalRow, 14].Value = totalStockValue;
                ws.Cells[totalRow, 14].Style.Font.Bold = true;
                ws.Cells[totalRow, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                ws.Cells[totalRow, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                // Footer: Print DateTime
                var footerRow = totalRow + 2;
                ws.Cells[footerRow, 1, footerRow, 14].Merge = true;
                ws.Cells[footerRow, 1].Value = $"Print Datetime: {DateTime.Now:dd/MM/yyyy hh:mm tt}";
                ws.Cells[footerRow, 1].Style.Font.Size = 10;
                ws.Cells[footerRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                // AutoFit All Columns
                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                // Return file
                var fileBytes = package.GetAsByteArray();
                var fileName = "Product_Stock_History_Report.xlsx";

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
