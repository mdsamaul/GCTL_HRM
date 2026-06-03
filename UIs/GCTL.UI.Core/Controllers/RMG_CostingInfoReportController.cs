using GCTL.Core.ViewModels.RMG_CostingInfoReport;
using GCTL.Service.EmployeeOfficialInfoReport;
using GCTL.Service.RMG_CostingInfoReport;
using GCTL.UI.Core.ViewModels.RMG_CostingInfoReport;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Threading.Tasks;

namespace GCTL.UI.Core.Controllers
{
    public class RMG_CostingInfoReportController : BaseController
    {
        private readonly IRMG_CostingInfoReportService _costingReportService;

        public RMG_CostingInfoReportController(IRMG_CostingInfoReportService costingReportService)
        {
            _costingReportService = costingReportService;
        }

        public async Task<IActionResult> Index()
        {
            var hasPermission = await _costingReportService.PagePermissionAsync(LoginInfo.AccessCode);

            if (!hasPermission)

            {

                return RedirectToAction("Login", "Accounts");

            }

            var model = new RMG_CostingInfoReportViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> GetAllFilterData([FromBody] CostingFilterRequest request)
        {
            try
            {
                var result = await _costingReportService.GetFilterDataAsync(request);
                return Json(new { isSuccess = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = ex.Message });
            }
        }

        // 2.  Costing Full Report ( Single View )
        [HttpGet]
        public async Task<IActionResult> GetCostingReport(string costingId, string integraJobNo, string purchaseOrder, string productId)
        {
            try
            {
                var data = await _costingReportService.GetCostingReportAsync(costingId, integraJobNo, purchaseOrder, productId);
                if (data == null)
                    return NotFound(new { message = "Costing report not found" });

                return Json(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // 3. Multiple Costing  Full Data (PDF + Excel )
        [HttpPost]
        public async Task<IActionResult> GetAllPdfFilterData([FromBody] CostingFilterRequest request)
        {
            try
            {
                var result = await _costingReportService.GetFilteredReportsAsync(request);

                if (result == null || !result.Any())
                    return Json(new { isSuccess = false, message = "No data found for selected filters." });

                return Json(new { isSuccess = true, data = new { costingReports = result } });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DownloadExcel([FromBody] List<CostingReportForExcel> costingReports)
        {
            try
            {
                if (costingReports == null || !costingReports.Any())
                    return BadRequest("No data to export");

                var excelBytes = GenerateExcelReport(costingReports);
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Costing_Report.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        private byte[] GenerateExcelReport(List<CostingReportForExcel> reports)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Costing Report");
                int currentRow = 1;

                // Header
                worksheet.Cells[currentRow, 1, currentRow, 14].Merge = true;
                worksheet.Cells[currentRow, 1].Value = "Costing Report";
                worksheet.Cells[currentRow, 1].Style.Font.Size = 16;
                worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                currentRow += 2;

                foreach (var data in reports)
                {
                    // Left Section
                    int leftStartRow = currentRow;
                    AddLeftSection(worksheet, ref currentRow, data, leftStartRow);

                    // Right Section
                    currentRow = leftStartRow;
                    AddRightSection(worksheet, ref currentRow, data, leftStartRow);

                    currentRow = Math.Max(currentRow, leftStartRow + 8);
                    currentRow += 2;

                    // Color & Size Breakup
                    AddColorSizeBreakup(worksheet, ref currentRow, data);
                    currentRow += 2;

                    // Main Details Table
                    AddDetailsTable(worksheet, ref currentRow, data);
                    currentRow += 0;

                    // Summary Section
                    AddSummarySection(worksheet, ref currentRow, data);
                    currentRow += 3;
                }

                // Footer
                currentRow += 2;
                worksheet.Cells[currentRow, 1].Value = $"Print Date Time: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                return package.GetAsByteArray();
            }
        }

        private void AddLeftSection(ExcelWorksheet ws, ref int row, CostingReportForExcel data, int startRow)
        {
            ws.Cells[row, 1].Value = "Costing ID";
            ws.Cells[row, 2].Value = $": {data.CostingId}";
            row++;

            ws.Cells[row, 1].Value = "Entry Date & Time";
            ws.Cells[row, 2].Value = $": {FormatDateTime(data.EntryDateTime)}";
            row++;

            ws.Cells[row, 1].Value = "Issued By";
            ws.Cells[row, 2].Value = $": {data.IssuedBy}";
            row++;

            ws.Cells[row, 1].Value = "Checked by";
            ws.Cells[row, 2].Value = $": {data.CheckedName}";
            row++;

            ws.Cells[row, 1].Value = "Remarks";
            ws.Cells[row, 2].Value = $": {data.Remarks}";
        }

        private void AddRightSection(ExcelWorksheet ws, ref int row, CostingReportForExcel data, int startRow)
        {
            ws.Cells[row, 5].Value = "Buyer";
            ws.Cells[row, 6].Value = $": {data.BuyerName}";
            row++;

            ws.Cells[row, 5].Value = "Fun Job No.";
            ws.Cells[row, 6].Value = $": {data.FunJobNo}";
            row++;

            ws.Cells[row, 5].Value = "Style";
            ws.Cells[row, 6].Value = $": {data.StyleName}";
            row++;

            ws.Cells[row, 5].Value = "PO No.";
            ws.Cells[row, 6].Value = $": {data.PoNo}";
            row++;

            ws.Cells[row, 5].Value = "Product";
            ws.Cells[row, 6].Value = $": {data.ItemName}";
            row++;

            ws.Cells[row, 5].Value = "Product Description";
            ws.Cells[row, 6].Value = $": {data.ProductDescription}";
            row++;

            ws.Cells[row, 5].Value = "Ref No./Client Ord. No.";
            ws.Cells[row, 6].Value = $": {data.RefNo}";
            row++;

            ws.Cells[row, 5].Value = "Shipment Date";
            ws.Cells[row, 6].Value = $": {FormatDate(data.ShipmentDate)}";
        }

        private void AddColorSizeBreakup(ExcelWorksheet ws, ref int row, CostingReportForExcel data)
        {
            ws.Cells[row, 1].Value = "Color & Size Breckup Details :";
            ws.Cells[row, 1].Style.Font.Bold = true;
            row++;

            var headerRow = row;
            ws.Cells[headerRow, 1].Value = "Sl No.";
            ws.Cells[headerRow, 2].Value = "Color";
            ws.Cells[headerRow, 3].Value = "Size";
            ws.Cells[headerRow, 4].Value = "Quantity";

            var headerRange = ws.Cells[headerRow, 1, headerRow, 4];
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerRange.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            SetBorders(headerRange);
            row++;

            var breakups = data.ColorSizeBreakups?.Where(x => !string.IsNullOrEmpty(x.ColorName) && !string.IsNullOrEmpty(x.SizeName)).ToList();
            int totalQty = 0;

            if (breakups != null)
            {
                for (int i = 0; i < breakups.Count; i++)
                {
                    var item = breakups[i];
                    ws.Cells[row, 1].Value = i + 1;
                    ws.Cells[row, 2].Value = item.ColorName;
                    ws.Cells[row, 3].Value = item.SizeName;
                    ws.Cells[row, 4].Value = item.Quantity;

                    ws.Cells[row, 1, row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    SetBorders(ws.Cells[row, 1, row, 4]);

                    totalQty += item.Quantity;
                    row++;
                }
            }

            // Total Row
            ws.Cells[row, 1, row, 3].Merge = true;
            ws.Cells[row, 1].Value = "Total :";
            ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ws.Cells[row, 1].Style.Font.Bold = true;
            ws.Cells[row, 4].Value = totalQty;
            ws.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ws.Cells[row, 4].Style.Font.Bold = true;
            var totalRange = ws.Cells[row, 1, row, 4];
            totalRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            totalRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(240, 240, 240));
            SetBorders(totalRange);
            row++;
        }

        private void AddDetailsTable(ExcelWorksheet ws, ref int row, CostingReportForExcel data)
        {
            var headerRow = row;
            string[] headers = { "Sl No.", "Item", "Description", "Width", "Gar. Qty", "Cons./pcs.",
                "Extra (%)", "Total", "Unit", "Unit Price", "Unit", "Amount($)-SH/HKG", "Amount($)-BD", "Amount(THB)" };

            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cells[headerRow, i + 1].Value = headers[i];
            }

            var headerRange = ws.Cells[headerRow, 1, headerRow, 14];
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(200, 200, 200));
            headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            headerRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            headerRange.Style.WrapText = true;
            SetBorders(headerRange);
            row++;

            var details = data.Details.ToList();
            if (details != null)
            {
                foreach (var detail in details)
                {
                    ws.Cells[row, 1].Value = detail.Slno;
                    ws.Cells[row, 2].Value = detail.ItemName;
                    ws.Cells[row, 3].Value = detail.Description;
                    ws.Cells[row, 4].Value = detail.Width;
                    ws.Cells[row, 5].Value = detail.Quantity > 0 ? detail.Quantity : (object)"";
                    ws.Cells[row, 6].Value = detail.Consumption > 0 ? detail.Consumption : (object)"";
                    ws.Cells[row, 7].Value = detail.Extra > 0 ? $"{detail.Extra}%" : "";
                    ws.Cells[row, 8].Value = detail.Quantity * detail.Consumption;
                    ws.Cells[row, 9].Value = detail.Unit;
                    ws.Cells[row, 10].Value = detail.UnitPrice > 0 ? detail.UnitPrice : (object)"";
                    ws.Cells[row, 11].Value = detail.TotalQuantityUnit;
                    ws.Cells[row, 12].Value = detail.TotalAmountShhkg > 0 ? detail.TotalAmountShhkg : (object)"";
                    ws.Cells[row, 13].Value = detail.TotalAmountBdt > 0 ? detail.TotalAmountBdt : (object)"";
                    ws.Cells[row, 14].Value = detail.TotalAmountThb > 0 ? detail.TotalAmountThb : (object)"";

                    int[] centerCols = { 1, 4, 5, 6, 7, 8, 9, 10, 11 };
                    foreach (var col in centerCols)
                    {
                        ws.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    ws.Cells[row, 6].Style.Numberformat.Format = "0.00";
                    ws.Cells[row, 10].Style.Numberformat.Format = "0.00";
                    ws.Cells[row, 12].Style.Numberformat.Format = "#,##0.00";
                    ws.Cells[row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    ws.Cells[row, 13].Style.Numberformat.Format = "#,##0.00";
                    ws.Cells[row, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    ws.Cells[row, 14].Style.Numberformat.Format = "#,##0.00";
                    ws.Cells[row, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    SetBorders(ws.Cells[row, 1, row, 14]);
                    row++;
                }
            }
        }

        private void AddSummarySection(ExcelWorksheet ws, ref int row, CostingReportForExcel data)
        {
            var garQty = data.Details?.FirstOrDefault()?.Quantity ?? 1;
            var subTotalPerGarQty = data.SubTotalAmountShhkg / garQty;

            AddSummaryRow(ws, ref row, "Sub Total:", data.SubTotalAmountShhkg, data.SubTotalAmountBdt, data.SubTotalAmountThb, true);
            AddSummaryRow(ws, ref row, "Sub Total (Per Gar. Qty):",
                data.SubTotalAmountShhkg / garQty, data.SubTotalAmountBdt / garQty, data.SubTotalAmountThb / garQty, true);

            var damageAmt1 = subTotalPerGarQty * (data.DamagePercentage / 100);
            var damageAmt2 = (data.SubTotalAmountBdt / garQty) * (data.DamagePercentage / 100);
            var damageAmt3 = (data.SubTotalAmountThb / garQty) * (data.DamagePercentage / 100);
            AddSummaryRow(ws, ref row, "Damage(%)", damageAmt1, damageAmt2, damageAmt3, true);

            var overheadAmt1 = subTotalPerGarQty * (data.InterestOverheadPercentage / 100);
            var overheadAmt2 = (data.SubTotalAmountBdt / garQty) * (data.InterestOverheadPercentage / 100);
            var overheadAmt3 = (data.SubTotalAmountThb / garQty) * (data.InterestOverheadPercentage / 100);
            AddSummaryRow(ws, ref row, "Interest/Overhead(%)", overheadAmt1, overheadAmt2, overheadAmt3, true);

            AddSummaryRow(ws, ref row, "Total:",
                subTotalPerGarQty + damageAmt1 + overheadAmt1,
                (data.SubTotalAmountBdt / garQty) + damageAmt2 + overheadAmt2,
                (data.SubTotalAmountThb / garQty) + damageAmt3 + overheadAmt3, true, true);

            AddSimpleRow(ws, ref row, "Total Material Cost from Overseas:", data.TotalMaterialCostOverseas, "USD");
            AddSimpleRow(ws, ref row, "Total Material Cost from Bangladesh:", data.TotalMaterialCostBdt, "USD");
            AddSimpleRow(ws, ref row, "Total Material Cost from BKK +20%:", data.TotalAmountThb, "USD");
            AddSimpleRow(ws, ref row, "CM And Profit:", data.CmandProfit, "USD");
            AddSimpleRow(ws, ref row, "Handling Charge:", data.HandlingCharge, "USD");
            AddSimpleRow(ws, ref row, "Production Upcharge:", data.ProductionUpCharge, "USD");
            AddSimpleRow(ws, ref row, "FF Price:", data.Ffprice, "USD");

            ws.Cells[row, 12].Value = "Total:";
            ws.Cells[row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ws.Cells[row, 12].Style.Font.Bold = true;
            ws.Cells[row, 13].Value = garQty * data.Ffprice;
            ws.Cells[row, 13].Style.Numberformat.Format = "#,##0.00";
            ws.Cells[row, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ws.Cells[row, 13].Style.Font.Bold = true;
            ws.Cells[row, 14].Value = "USD";
            ws.Cells[row, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            SetBorders(ws.Cells[row, 12, row, 14]);
        }

        private void AddSummaryRow(ExcelWorksheet ws, ref int row, string label, decimal v1, decimal v2, decimal v3, bool bordered, bool isBold = false)
        {
            ws.Cells[row, 1, row, 11].Merge = true;
            ws.Cells[row, 1].Value = label;
            ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            if (isBold) ws.Cells[row, 1].Style.Font.Bold = true;

            ws.Cells[row, 12].Value = v1;
            ws.Cells[row, 12].Style.Numberformat.Format = label.Contains("(%)") ? "0.00" : "#,##0.00";
            ws.Cells[row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            if (isBold) ws.Cells[row, 12].Style.Font.Bold = true;

            ws.Cells[row, 13].Value = v2;
            ws.Cells[row, 13].Style.Numberformat.Format = label.Contains("(%)") ? "0.00" : "#,##0.00";
            ws.Cells[row, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            if (isBold) ws.Cells[row, 13].Style.Font.Bold = true;

            ws.Cells[row, 14].Value = v3;
            ws.Cells[row, 14].Style.Numberformat.Format = label.Contains("(%)") ? "0.00" : "#,##0.00";
            ws.Cells[row, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            if (isBold) ws.Cells[row, 14].Style.Font.Bold = true;

            if (bordered)
            {
                SetBorders(ws.Cells[row, 1, row, 14]);
            }
            row++;
        }

        private void AddSimpleRow(ExcelWorksheet ws, ref int row, string label, decimal value, string unit)
        {
            ws.Cells[row, 12].Value = label;
            ws.Cells[row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ws.Cells[row, 13].Value = value;
            ws.Cells[row, 13].Style.Numberformat.Format = "0.00";
            ws.Cells[row, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ws.Cells[row, 14].Value = unit;
            ws.Cells[row, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            SetBorders(ws.Cells[row, 12, row, 14]);
            row++;
        }

        private void SetBorders(ExcelRange range)
        {
            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        }

        private string FormatDate(DateTime? date)
        {
            return date?.ToString("dd/MM/yyyy") ?? "";
        }

        private string FormatDateTime(DateTime? date)
        {
            return date?.ToString("dd/MM/yyyy HH:mm:ss") ?? "";
        }
    }
}
