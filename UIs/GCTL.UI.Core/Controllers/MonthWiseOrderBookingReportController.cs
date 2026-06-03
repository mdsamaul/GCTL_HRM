//using GCTL.Core.Data;
//using GCTL.Core.Helpers;
//using GCTL.Core.ViewModels.MonthWiseOrderBookingReport;
//using GCTL.Data.Models;
//using GCTL.Service.MonthWiseOrderBookingReport;
//using GCTL.UI.Core.ViewModels.MonthWiseOrderBookingReportViewModel;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using OfficeOpenXml;
//using OfficeOpenXml.Style;

//namespace GCTL.UI.Core.Controllers
//{
//    public class MonthWiseOrderBookingReportController : BaseController
//    {
//        private readonly IOrderReportService _orderReportService;
//        private readonly IRepository<RmgProdDefBuyer> buyerRepo;

//        public MonthWiseOrderBookingReportController(
//            IOrderReportService orderReportService,
//            IRepository<RmgProdDefBuyer> buyerRepo
//            )
//        {
//            _orderReportService = orderReportService;
//            this.buyerRepo = buyerRepo;
//        }

//        // View Page Load
//        public async Task<IActionResult> Index()
//        {
//            var hasPermission = await _orderReportService.PagePermissionAsync(LoginInfo.AccessCode);

//            if (!hasPermission)

//            {

//                return RedirectToAction("Login", "Accounts");

//            }
//            ViewBag.buyersList = new SelectList(buyerRepo.All().Select(x => new { id = x.BuyerId, name = x.BuyerName }), "id", "name");
//            //ViewBag.ProductList = new SelectList(productRepo.All().Select(x => new { x.ProductCode, x.ProductName }), "ProductCode", "ProductName");
//            OrderReportDataViewModel model = new OrderReportDataViewModel()
//            {
//                PageUrl = Url.Action(nameof(Index))
//            };
//            return View(model);
//        }

//        // POST: Get Order Report Data

//        public async Task<IActionResult> DownloadOrderAllStyleReport([FromBody] OrderReportRequest request)
//        {
//            try
//            {
//                request.ToAudit(LoginInfo);
//                var reportData = await _orderReportService.GetOrderReportAllStyleAsync(request, LoginInfo.CompanyCode);
//                var excelFile = GenerateExcel(reportData);

//                return File(excelFile,
//                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
//                    $"MonthWiseOrderBookingAllStyleReport.xlsx");
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(new { success = false, message = ex.Message });
//            }
//        }

//        private byte[] GenerateExcel(OrderReportAllStyleResponse reportData)
//        {
//            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

//            using var package = new ExcelPackage();
//            var worksheet = package.Workbook.Worksheets.Add("All Style Report");

//            // Header Section
//            worksheet.Cells[1, 1].Value = reportData.CompanyName;
//            worksheet.Cells[1, 1].Style.Font.Bold = true;
//            worksheet.Cells[1, 1].Style.Font.Size = 14;
//            worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

//            worksheet.Cells[2, 1].Value = reportData.ReportTitle + " " + reportData.ReportYear;
//            worksheet.Cells[2, 1].Style.Font.Bold = true;
//            worksheet.Cells[2, 1].Style.Font.Size = 12;
//            worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

//            // Get dynamic month columns
//            var monthColumns = reportData.Data.FirstOrDefault()?.MonthlyQuantities.Keys.ToList() ?? new List<string>();
//            int totalColumns = 5 + monthColumns.Count;

//            // Merge header cells
//            worksheet.Cells[1, 1, 1, totalColumns].Merge = true;
//            worksheet.Cells[2, 1, 2, totalColumns].Merge = true;

//            // Column Headers (Row 4)
//            int row = 4;
//            int col = 1;

//            worksheet.Cells[row, col++].Value = "Sl No.";
//            worksheet.Cells[row, col++].Value = "Buyer Name";
//            worksheet.Cells[row, col++].Value = "Style";
//            worksheet.Cells[row, col++].Value = "Item";
//            worksheet.Cells[row, col++].Value = "Total Order Quantity";

//            foreach (var month in monthColumns)
//            {
//                worksheet.Cells[row, col++].Value = month;
//            }

//            // Style header row
//            using (var range = worksheet.Cells[row, 1, row, totalColumns])
//            {
//                range.Style.Font.Bold = true;
//                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
//                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
//                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
//                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
//            }

//            // Data Rows
//            row = 5;
//            foreach (var item in reportData.Data)
//            {
//                col = 1;
//                if (decimal.TryParse(item.SlNo, out decimal sln))
//                {
//                    worksheet.Cells[row, col].Value = sln;
//                    worksheet.Cells[row, col].Style.Numberformat.Format = "#,##0";
//                    worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
//                }
//                col++;
//                //worksheet.Cells[row, col++].Value = item.SlNo;
//                worksheet.Cells[row, col++].Value = item.BuyerName;
//                worksheet.Cells[row, col++].Value = item.Style;
//                worksheet.Cells[row, col++].Value = item.Item;

//                // Total Order Quantity - Numeric & Right Aligned
//                if (decimal.TryParse(item.TotalOrderQuantity, out decimal totalQty))
//                {
//                    worksheet.Cells[row, col].Value = totalQty;
//                    worksheet.Cells[row, col].Style.Numberformat.Format = "#,##0";
//                    worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
//                }
//                col++;

//                // Monthly Quantities - Numeric & Right Aligned
//                foreach (var month in monthColumns)
//                {
//                    if (item.MonthlyQuantities.TryGetValue(month, out string value))
//                    {
//                        if (decimal.TryParse(value, out decimal monthQty))
//                        {
//                            worksheet.Cells[row, col].Value = monthQty;
//                            worksheet.Cells[row, col].Style.Numberformat.Format = "#,##0";
//                            worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
//                        }
//                    }
//                    else
//                    {
//                        worksheet.Cells[row, col].Value = "";
//                        worksheet.Cells[row, col].Style.Numberformat.Format = "#,##0";
//                        worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
//                    }
//                    col++;
//                }

//                row++;
//            }

//            // Auto-fit columns
//            //worksheet.Cells.AutoFitColumns();
//            worksheet.Column(1).Width = 8;   // Sl No.
//            worksheet.Column(2).Width = 20;  // Buyer Name
//            worksheet.Column(3).Width = 20;  // Style
//            worksheet.Column(4).Width = 20;  // Item
//            worksheet.Column(5).Width = 18;  // Total Order Quantity

//            int startMonthColumn = 6;
//            foreach (var month in monthColumns)
//            {
//                worksheet.Column(startMonthColumn).Width = 12;
//                startMonthColumn++;
//            }

//            // Add borders to all data
//            using (var range = worksheet.Cells[4, 1, row - 1, totalColumns])
//            {
//                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
//                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
//                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
//                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
//            }

//            return package.GetAsByteArray();
//        }

//        [HttpPost]
//        public async Task<IActionResult> DownloadOrderStyleReport([FromBody] OrderReportRequest request)
//        {
//            try
//            {
//                request.ToAudit(LoginInfo);
//                var reportData = await _orderReportService.GetOrderReportStyleAsync(request, LoginInfo.CompanyCode);
//                var excelFile = GenerateStyleExcel(reportData);
//                return File(excelFile,
//                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
//                    $"MonthWiseOrderBookingStyleReport.xlsx");
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(new { success = false, message = ex.Message });
//            }
//        }

//        private byte[] GenerateStyleExcel(OrderReportStyleResponse reportData)
//        {
//            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

//            using var package = new ExcelPackage();
//            var worksheet = package.Workbook.Worksheets.Add("Style Report");

//            // Header Section
//            worksheet.Cells[1, 1].Value = reportData.CompanyName;
//            worksheet.Cells[1, 1].Style.Font.Bold = true;
//            worksheet.Cells[1, 1].Style.Font.Size = 14;
//            worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

//            worksheet.Cells[2, 1].Value = reportData.ReportTitle + " " + reportData.ReportYear;
//            worksheet.Cells[2, 1].Style.Font.Bold = true;
//            worksheet.Cells[2, 1].Style.Font.Size = 12;
//            worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

//            // Get month columns
//            var monthColumns = reportData.MonthColumns ?? new List<string>();
//            int totalColumns = 4 + monthColumns.Count;

//            // Merge header cells
//            worksheet.Cells[1, 1, 1, totalColumns].Merge = true;
//            worksheet.Cells[2, 1, 2, totalColumns].Merge = true;

//            // Column Headers (Row 4)
//            int row = 4;
//            int col = 1;

//            //worksheet.Cells[row, col++].Value = "Sl No.";
//            worksheet.Cells[row, col++].Value = "Buyer Name";
//            worksheet.Cells[row, col++].Value = "Style";
//            worksheet.Cells[row, col++].Value = "Item";
//            worksheet.Cells[row, col++].Value = "Total Order Quantity";

//            foreach (var month in monthColumns)
//            {
//                worksheet.Cells[row, col++].Value = month;
//            }

//            // Style header row
//            using (var range = worksheet.Cells[row, 1, row, totalColumns])
//            {
//                range.Style.Font.Bold = true;
//                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
//                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
//                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
//                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
//            }

//            // Data Rows
//            row = 5;
//            foreach (var item in reportData.Data)
//            {
//                col = 1;

//                // Sl No
//                //if (decimal.TryParse(item.SlNo, out decimal sln))
//                //{
//                //    worksheet.Cells[row, col].Value = sln;
//                //    worksheet.Cells[row, col].Style.Numberformat.Format = "0";
//                //    worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
//                //}
//                //col++;

//                // Buyer Name
//                worksheet.Cells[row, col].Value = item.BuyerName;
//                worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
//                col++;

//                // Style
//                worksheet.Cells[row, col].Value = item.Style;
//                worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
//                worksheet.Cells[row, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
//                col++;

//                // Item
//                worksheet.Cells[row, col].Value = item.Item;
//                worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
//                col++;

//                // Total Order Quantity
//                if (decimal.TryParse(item.TotalOrderQuantity, out decimal totalQty))
//                {
//                    worksheet.Cells[row, col].Value = totalQty;
//                    worksheet.Cells[row, col].Style.Numberformat.Format = "#,##0";
//                    worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
//                    worksheet.Cells[row, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
//                }
//                col++;

//                // Monthly Quantities
//                foreach (var month in monthColumns)
//                {
//                    if (item.MonthlyQuantities.TryGetValue(month, out string value) &&
//                        !string.IsNullOrEmpty(value) &&
//                        decimal.TryParse(value, out decimal monthQty))
//                    {
//                        worksheet.Cells[row, col].Value = monthQty;
//                        worksheet.Cells[row, col].Style.Numberformat.Format = "#,##0";
//                        worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
//                        worksheet.Cells[row, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
//                    }
//                    else
//                    {
//                        worksheet.Cells[row, col].Value = "";
//                    }
//                    col++;
//                }

//                row++;
//            }

//            // Auto-fit columns
//            //worksheet.Cells[1, 1, row - 1, totalColumns].AutoFitColumns();           
//            worksheet.Column(1).Width = 20;   // Buyer Name
//            worksheet.Column(2).Width = 20;   // Style
//            worksheet.Column(3).Width = 25;   // Item
//            worksheet.Column(4).Width = 18;   // Total Order Quantity

//            int startMonthColumn = 5;
//            foreach (var month in monthColumns)
//            {
//                worksheet.Column(startMonthColumn).Width = 12; // Each Month Column
//                startMonthColumn++;
//            }


//            // Add borders to all data
//            using (var range = worksheet.Cells[4, 1, row - 1, totalColumns])
//            {
//                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
//                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
//                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
//                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
//            }

//            return package.GetAsByteArray();
//        }

//        [HttpPost]
//        public async Task<IActionResult> DownloadOrderStylePoReport([FromBody] OrderReportRequest request)
//        {
//            try
//            {
//                request.ToAudit(LoginInfo);
//                var reportData = await _orderReportService.GetOrderReportStylePoAsync(request, LoginInfo.CompanyCode);
//                var excelFile = GenerateStylePoExcel(reportData);
//                return File(excelFile,
//                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
//                    $"MonthWiseOrderBookingStylePoReport.xlsx");
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(new { success = false, message = ex.Message });
//            }
//        }

//        private byte[] GenerateStylePoExcel(OrderReportStylePoResponse reportData)
//        {
//            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

//            using var package = new ExcelPackage();
//            var worksheet = package.Workbook.Worksheets.Add("Style PO Report");
//            worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
//            worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

//            // Header Section
//            worksheet.Cells[1, 1].Value = reportData.CompanyName;
//            worksheet.Cells[1, 1].Style.Font.Bold = true;
//            worksheet.Cells[1, 1].Style.Font.Size = 14;
//            worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
//            worksheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

//            worksheet.Cells[2, 1].Value = reportData.ReportTitle + " " + reportData.ReportYear;
//            worksheet.Cells[2, 1].Style.Font.Bold = true;
//            worksheet.Cells[2, 1].Style.Font.Size = 12;
//            worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
//            worksheet.Cells[2, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

//            var monthColumns = reportData.MonthColumns ?? new List<string>();
//            int totalColumns = 5 + monthColumns.Count;

//            worksheet.Cells[1, 1, 1, totalColumns].Merge = true;
//            worksheet.Cells[2, 1, 2, totalColumns].Merge = true;

//            // Column Headers (Row 4)
//            int row = 4;
//            int col = 1;

//            worksheet.Cells[row, col++].Value = "Buyer Name";
//            worksheet.Cells[row, col++].Value = "Style";
//            worksheet.Cells[row, col++].Value = "Item";
//            worksheet.Cells[row, col++].Value = "P.O";
//            worksheet.Cells[row, col++].Value = "Order Quantity";

//            foreach (var month in monthColumns)
//                worksheet.Cells[row, col++].Value = month;

//            using (var range = worksheet.Cells[row, 1, row, totalColumns])
//            {
//                range.Style.Font.Bold = true;
//                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
//                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
//                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
//                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
//            }

//            // Data Rows
//            row = 5;
//            string currentBuyer = "";
//            string currentStyle = "";
//            int buyerStartRow = row;
//            int styleStartRow = row;

//            foreach (var item in reportData.Data)
//            {
//                col = 1;

//                // Buyer Name
//                if (currentBuyer != item.BuyerName)
//                {
//                    if (currentBuyer != "" && buyerStartRow < row)
//                        worksheet.Cells[buyerStartRow, 1, row - 1, 1].Merge = true;

//                    currentBuyer = item.BuyerName;
//                    buyerStartRow = row;
//                }
//                worksheet.Cells[row, col++].Value = item.BuyerName;

//                // Style
//                if (currentStyle != item.Style || currentBuyer != item.BuyerName)
//                {
//                    if (currentStyle != "" && styleStartRow < row)
//                        worksheet.Cells[styleStartRow, 2, row - 1, 2].Merge = true;

//                    currentStyle = item.Style;
//                    styleStartRow = row;
//                }
//                worksheet.Cells[row, col++].Value = item.Style;

//                worksheet.Cells[row, col++].Value = item.Item;
//                worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
//                worksheet.Cells[row, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

//                worksheet.Cells[row, col++].Value = item.PurchaseOrder;

//                // Order Qty
//                if (decimal.TryParse(item.OrderQuantity, out decimal orderQty))
//                {
//                    worksheet.Cells[row, col].Value = orderQty;
//                    worksheet.Cells[row, col].Style.Numberformat.Format = "#,##0";
//                }
//                col++;

//                // Month Values (text allowed)
//                foreach (var monthKey in monthColumns)
//                {
//                    worksheet.Cells[row, col].Value = item.MonthlyQuantities.ContainsKey(monthKey)
//                        ? item.MonthlyQuantities[monthKey]
//                        : "";

//                    worksheet.Cells[row, col].Style.WrapText = true;
//                    worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
//                    worksheet.Cells[row, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
//                    col++;
//                }

//                row++;
//            }

//            // Merge last buyer & style blocks
//            if (buyerStartRow < row)
//                worksheet.Cells[buyerStartRow, 1, row - 1, 1].Merge = true;

//            if (styleStartRow < row)
//                worksheet.Cells[styleStartRow, 2, row - 1, 2].Merge = true;

//            worksheet.Cells.Style.WrapText = false;
//            // worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

//            // 🔥 Fixed Width Columns
//            worksheet.Column(1).Width = 18;   // Buyer Name
//            worksheet.Column(2).Width = 18;   // Style
//            worksheet.Column(3).Width = 25;   // Item
//            worksheet.Column(4).Width = 14;   // P.O
//            worksheet.Column(5).Width = 16;   // Order Quantity

//            int startCol = 6;
//            foreach (var month in monthColumns)
//            {
//                worksheet.Column(startCol).Width = 12; // Dynamic Month Column Width
//                startCol++;
//            }

//            for (int c = 1; c <= totalColumns; c++)
//                worksheet.Column(c).Width += 2;
//            for (int c = 1; c <= totalColumns; c++)
//                worksheet.Column(c).Style.WrapText = true;

//            for (int r = 4; r < row; r++)
//                for (int c = 1; c <= totalColumns; c++)
//                    worksheet.Cells[r, c].Style.Border.BorderAround(ExcelBorderStyle.Thin);

//            return package.GetAsByteArray();
//        }


//        // ============ CONTROLLER ============
//        [HttpPost]
//        public async Task<IActionResult> DownloadOrderStylePoCSReport([FromBody] OrderReportRequest request)
//        {
//            try
//            {
//                request.ToAudit(LoginInfo);
//                var reportData = await _orderReportService.GetOrderReportStylePoCSAsync(request, LoginInfo.CompanyCode);
//                var excelFile = GenerateStylePoCSExcel(reportData);
//                return File(excelFile,
//                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
//                    $"MonthWiseOrderBookingStylePOColorSizeReport.xlsx");
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(new { success = false, message = ex.Message });
//            }
//        }

//        // ============ EXCEL GENERATION ============
//        private byte[] GenerateStylePoCSExcel(OrderReportStylePoCSResponse reportData)
//        {
//            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

//            using var package = new ExcelPackage();
//            var worksheet = package.Workbook.Worksheets.Add("Style PO Color Size Report");

//            var monthColumns = reportData.MonthColumns ?? new List<string>();
//            int totalColumns = 5 + (monthColumns.Count * 3);

//            // Headers
//            worksheet.Cells[1, 1].Value = reportData.CompanyName;
//            worksheet.Cells[1, 1].Style.Font.Bold = true;
//            worksheet.Cells[1, 1].Style.Font.Size = 14;
//            worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
//            worksheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
//            worksheet.Cells[1, 1, 1, totalColumns].Merge = true;

//            worksheet.Cells[2, 1].Value = reportData.ReportTitle + " " + reportData.ReportYear;
//            worksheet.Cells[2, 1].Style.Font.Bold = true;
//            worksheet.Cells[2, 1].Style.Font.Size = 12;
//            worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
//            worksheet.Cells[2, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
//            worksheet.Cells[2, 1, 2, totalColumns].Merge = true;

//            // Row 4: Main headers
//            int row = 4;
//            int col = 1;

//            worksheet.Cells[row, col++].Value = "Buyer Name";
//            worksheet.Cells[row, col++].Value = "Style";
//            worksheet.Cells[row, col++].Value = "Item";
//            worksheet.Cells[row, col++].Value = "P.O";
//            worksheet.Cells[row, col++].Value = "Order Quantity";

//            foreach (var month in monthColumns)
//            {
//                worksheet.Cells[row, col, row, col + 2].Merge = true;
//                worksheet.Cells[row, col].Value = month;
//                worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
//                worksheet.Cells[row, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
//                col += 3;
//            }

//            using (var range = worksheet.Cells[row, 1, row, totalColumns])
//            {
//                range.Style.Font.Bold = true;
//                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
//                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
//                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
//                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
//            }

//            // Row 5: Sub-headers
//            row = 5;
//            worksheet.Cells[4, 1, 5, 1].Merge = true; // Buyer Name
//            worksheet.Cells[4, 2, 5, 2].Merge = true; // Style
//            worksheet.Cells[4, 3, 5, 3].Merge = true; // Item
//            worksheet.Cells[4, 4, 5, 4].Merge = true; // P.O
//            worksheet.Cells[4, 5, 5, 5].Merge = true; // Order Quantity

//            col = 6;
//            foreach (var month in monthColumns)
//            {
//                worksheet.Cells[row, col++].Value = "Col.";
//                worksheet.Cells[row, col++].Value = "Size";
//                worksheet.Cells[row, col++].Value = "Qty";
//            }

//            using (var range = worksheet.Cells[row, 6, row, totalColumns])
//            {
//                range.Style.Font.Bold = true;
//                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
//                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
//                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
//                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
//            }

//            // Data rows
//            row = 6;
//            string currentBuyer = "";
//            string currentStyle = "";
//            int buyerStartRow = row;
//            int styleStartRow = row;

//            foreach (var item in reportData.Data)
//            {
//                int itemStartRow = row;
//                int maxRows = 1;

//                // Calculate max rows needed
//                foreach (var month in monthColumns)
//                {
//                    if (item.MonthlyData.ContainsKey(month))
//                    {
//                        int monthRows = item.MonthlyData[month].Count;
//                        if (monthRows > maxRows) maxRows = monthRows;
//                    }
//                }

//                // Buyer Name
//                if (currentBuyer != item.BuyerName)
//                {
//                    if (currentBuyer != "" && buyerStartRow < row)
//                    {
//                        worksheet.Cells[buyerStartRow, 1, row - 1, 1].Merge = true;
//                        worksheet.Cells[buyerStartRow, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
//                    }
//                    currentBuyer = item.BuyerName;
//                    buyerStartRow = row;
//                }
//                worksheet.Cells[row, 1].Value = item.BuyerName;
//                worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

//                // Style
//                if (currentStyle != item.Style || currentBuyer != item.BuyerName)
//                {
//                    if (currentStyle != "" && styleStartRow < row)
//                    {
//                        worksheet.Cells[styleStartRow, 2, row - 1, 2].Merge = true;
//                        worksheet.Cells[styleStartRow, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
//                    }
//                    currentStyle = item.Style;
//                    styleStartRow = row;
//                }
//                worksheet.Cells[row, 2].Value = item.Style;
//                worksheet.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

//                // Item
//                worksheet.Cells[row, 3, row + maxRows - 1, 3].Merge = true;
//                worksheet.Cells[row, 3].Value = item.Item;
//                worksheet.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
//                worksheet.Cells[row, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

//                // P.O
//                worksheet.Cells[row, 4, row + maxRows - 1, 4].Merge = true;
//                worksheet.Cells[row, 4].Value = item.PurchaseOrder;
//                worksheet.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
//                worksheet.Cells[row, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

//                // Order Quantity
//                worksheet.Cells[row, 5, row + maxRows - 1, 5].Merge = true;
//                if (decimal.TryParse(item.OrderQuantity, out decimal orderQty))
//                {
//                    worksheet.Cells[row, 5].Value = orderQty;
//                    worksheet.Cells[row, 5].Style.Numberformat.Format = "#,##0";
//                }
//                worksheet.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
//                worksheet.Cells[row, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

//                // Monthly data with color merging
//                col = 6;
//                foreach (var month in monthColumns)
//                {
//                    if (item.MonthlyData.ContainsKey(month) && item.MonthlyData[month].Any())
//                    {
//                        var details = item.MonthlyData[month];

//                        // Group by color to merge
//                        var colorGroups = new Dictionary<string, List<int>>();
//                        for (int i = 0; i < details.Count; i++)
//                        {
//                            string color = details[i].Color ?? "";
//                            if (!colorGroups.ContainsKey(color))
//                                colorGroups[color] = new List<int>();
//                            colorGroups[color].Add(i);
//                        }

//                        // Write data and merge same colors
//                        for (int i = 0; i < maxRows; i++)
//                        {
//                            if (i < details.Count)
//                            {
//                                worksheet.Cells[row + i, col].Value = details[i].Color;
//                                worksheet.Cells[row + i, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

//                                worksheet.Cells[row + i, col + 1].Value = details[i].Size;
//                                worksheet.Cells[row + i, col + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

//                                if (decimal.TryParse(details[i].Quantity, out decimal qty))
//                                {
//                                    worksheet.Cells[row + i, col + 2].Value = qty;
//                                    worksheet.Cells[row + i, col + 2].Style.Numberformat.Format = "#,##0";
//                                }
//                                worksheet.Cells[row + i, col + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
//                            }
//                        }

//                        // Merge color cells
//                        foreach (var colorGroup in colorGroups)
//                        {
//                            if (colorGroup.Value.Count > 1)
//                            {
//                                int startIdx = colorGroup.Value.First();
//                                int endIdx = colorGroup.Value.Last();
//                                worksheet.Cells[row + startIdx, col, row + endIdx, col].Merge = true;
//                                worksheet.Cells[row + startIdx, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
//                            }
//                        }
//                    }
//                    col += 3;
//                }

//                row += maxRows;
//            }

//            // Merge last buyer and style
//            if (buyerStartRow < row)
//            {
//                worksheet.Cells[buyerStartRow, 1, row - 1, 1].Merge = true;
//                worksheet.Cells[buyerStartRow, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
//            }
//            if (styleStartRow < row)
//            {
//                worksheet.Cells[styleStartRow, 2, row - 1, 2].Merge = true;
//                worksheet.Cells[styleStartRow, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
//            }

//            //worksheet.Cells[1, 1, row - 1, totalColumns].AutoFitColumns();
//            // Set Fixed Column Widths
//            worksheet.Column(1).Width = 20;   // Buyer Name
//            worksheet.Column(2).Width = 20;   // Style
//            worksheet.Column(3).Width = 25;   // Item
//            worksheet.Column(4).Width = 12;   // P.O
//            worksheet.Column(5).Width = 18;   // Order Quantity

//            int startCol = 6;
//            foreach (var month in monthColumns)
//            {
//                worksheet.Column(startCol).Width = 10;   // Color
//                worksheet.Column(startCol + 1).Width = 10; // Size
//                worksheet.Column(startCol + 2).Width = 12; // Qty
//                startCol += 3;
//            }


//            // Borders
//            for (int r = 4; r < row; r++)
//            {
//                for (int c = 1; c <= totalColumns; c++)
//                {
//                    worksheet.Cells[r, c].Style.Border.Top.Style = ExcelBorderStyle.Thin;
//                    worksheet.Cells[r, c].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
//                    worksheet.Cells[r, c].Style.Border.Left.Style = ExcelBorderStyle.Thin;
//                    worksheet.Cells[r, c].Style.Border.Right.Style = ExcelBorderStyle.Thin;
//                }
//            }

//            return package.GetAsByteArray();
//        }










//        [HttpPost]
//        public async Task<IActionResult> DownloadOrderAllStylePdfReport([FromBody] OrderReportRequest request)
//        {
//            try
//            {
//                request.ToAudit(LoginInfo);
//                var reportData = await _orderReportService.GetOrderReportAllStyleAsync(request, LoginInfo.CompanyCode);

//                // Return the full report data as JSON
//                return Json(reportData);
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(new { success = false, message = ex.Message });
//            }
//        }
//        [HttpPost]
//        public async Task<IActionResult> DownloadOrderStylePdfReport([FromBody] OrderReportRequest request)
//        {
//            try
//            {
//                request.ToAudit(LoginInfo);
//                var reportData = await _orderReportService.GetOrderReportStyleAsync(request, LoginInfo.CompanyCode);
//                return Json(reportData);
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(new { success = false, message = ex.Message });
//            }
//        }



//        [HttpPost]
//        public async Task<IActionResult> DownloadOrderStylePoPdfReport([FromBody] OrderReportRequest request)
//        {
//            try
//            {
//                request.ToAudit(LoginInfo);
//                var reportData = await _orderReportService.GetOrderReportStylePoAsync(request, LoginInfo.CompanyCode);
//                return Json(reportData);
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(new { success = false, message = ex.Message });
//            }
//        }


//        [HttpPost]
//        public async Task<IActionResult> DownloadOrderStylePoCSPdfReport([FromBody] OrderReportRequest request)
//        {
//            try
//            {
//                request.ToAudit(LoginInfo);
//                var reportData = await _orderReportService.GetOrderReportStylePoCSAsync(request, LoginInfo.CompanyCode);
//                return Json(reportData);
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(new { success = false, message = ex.Message });
//            }
//        }

//        // GET: Styles Dropdown
//        [HttpGet]
//        public async Task<IActionResult> GetStyles()
//        {
//            var styles = await _orderReportService.GetStylesAsync();
//            return Json(styles);
//        }

//        // GET: Colors Dropdown
//        [HttpGet]
//        public async Task<IActionResult> GetColors()
//        {
//            var colors = await _orderReportService.GetColorsAsync();
//            return Json(colors);
//        }

//        // GET: Sizes Dropdown
//        [HttpGet]
//        public async Task<IActionResult> GetSizes()
//        {
//            var sizes = await _orderReportService.GetSizesAsync();
//            return Json(sizes);
//        }
//    }
//}
