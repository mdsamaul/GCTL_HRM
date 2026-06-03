using GCTL.Core.Data;
using GCTL.Core.ViewModels.TransportExpenseStatementReport;
using GCTL.Data.Models;
using GCTL.Service.TransportExpenseStatementReportService;
using GCTL.UI.Core.ViewModels.TransportExpenseStatementReport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace GCTL.UI.Core.Controllers
{
    public class TransportExpenseStatementReportController : BaseController
    {
        private readonly ITransportExpenseStatementReportServices transportExpenseStatementReportServices;
        private readonly IRepository<HrmPayMonth> monthRepo;

        public TransportExpenseStatementReportController(
            ITransportExpenseStatementReportServices transportExpenseStatementReportServices,
            IRepository<HrmPayMonth> monthRepo
            )
        {
            this.transportExpenseStatementReportServices = transportExpenseStatementReportServices;
            this.monthRepo = monthRepo;
        }
        public async Task<IActionResult> Index()
        {
            var hasPermission = await transportExpenseStatementReportServices.PagePermissionAsync(LoginInfo.AccessCode);

            if (!hasPermission)

            {

                return RedirectToAction("Login", "Accounts");

            }
            ViewBag.MonthList = new SelectList(monthRepo.All().Select(x => new { x.MonthId, x.MonthName }), "MonthId", "MonthName");
            TransportExpenseStatementReportViewModel model = new TransportExpenseStatementReportViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> GetFilterDropdownDataReport([FromBody] TransportExpenseStatementReportFilterDataDto FilterData)
        {
            var data = await transportExpenseStatementReportServices.GetAllTransportExpenseStatementDropdownSelectReportAsync(FilterData);

            return Ok(data);
        }
        [HttpPost]
        public async Task<IActionResult> GetFilterResultReport([FromBody] TransportExpenseStatementReportFilterDataDto FilterData)
        {
            var data = await transportExpenseStatementReportServices.GetAllTransportExpenseStatementResultReportAsync(FilterData);

            return Json(new { data = data });
        }


        [HttpPost]
        public async Task<IActionResult> ExportToExcel([FromBody] TransportExpenseStatementReportFilterDataDto filter)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var data = await transportExpenseStatementReportServices.GetAllTransportExpenseStatementResultReportExcelAsync(filter);

            if (data == null || data.Count == 0)
            {
                return Json(new { isSuccess = false, message = "No data found" });
            }

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("TransportExpense");

                // ==========================
                // Header Section
                // ==========================
                ws.Cells["A1:S1"].Merge = true;
                ws.Cells["A1"].Value = "Transport Expenses Statement";
                ws.Cells["A1"].Style.Font.Bold = true;
                ws.Cells["A1"].Style.Font.Size = 16;
                ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                ws.Cells["A2:S2"].Merge = true;

                // Determine subtitle
                string subtitle = "";
                bool isByDate = filter.FromDate.HasValue && filter.ToDate.HasValue && filter.FromDate.Value != DateTime.MinValue && filter.ToDate.Value != DateTime.MinValue;
                bool isByMonth = filter.Month.HasValue && filter.Year.HasValue;

                if (isByDate)
                {
                    subtitle = $"From {filter.FromDate.Value:dd/MM/yyyy} to {filter.ToDate.Value:dd/MM/yyyy}";
                }
                else if (isByMonth)
                {
                    string monthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(filter.Month.Value);
                    subtitle = $"For the Month Of {monthName}, {filter.Year.Value}";
                }

                ws.Cells["A2"].Value = subtitle;
                ws.Cells["A2"].Style.Font.Bold = true;
                ws.Cells["A2"].Style.Font.Size = 12;
                ws.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A2"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                int row = 4;

                // ==========================
                // Helper Function for Formatting
                // ==========================
                string FormatCurrency(decimal? value)
                {
                    if (value == null || value == 0)
                        return "-";
                    return value.Value.ToString("N2");
                }

                // ==========================
                // Table Header
                // ==========================
                string[] headers = {
            "SL.No.", "Transport.No", "Transport Name", "Transport Capacity/Persons", "Driver Name",
            "Helper Name", "CNG /Gas Bill", "R/M Bill", "Fuel/Octane Bill", "Police Donation",
            "Toll/others Bill", "TAX ,Fitness And Rute Permit", "Salary(Driver,Helper)", "Mechanic Salary",
            "Monthly Police Donation", "Monthly Eng.Oil Purchase", "Akesh Technology", "Garage Rent",
            "Total"
        };

                for (int i = 0; i < headers.Length; i++)
                {
                    ws.Cells[row, i + 1].Value = headers[i];
                    ws.Cells[row, i + 1].Style.Font.Bold = true;
                    ws.Cells[row, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[row, i + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells[row, i + 1].Style.WrapText = true;
                    ws.Cells[row, i + 1].Style.TextRotation = 90;
                    ws.Cells[row, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                ws.Row(row).Height = 85;
                row++;

                // ==========================
                // Data Rows
                // ==========================
                int slNo = 1;
                decimal totalCNG = 0, totalRM = 0, totalFuel = 0, totalPolice = 0, totalToll = 0;
                decimal totalTax = 0, totalSalary = 0, totalMechanic = 0, totalMonthlyPolice = 0;
                decimal totalEngOil = 0, totalAkesh = 0, totalGarage = 0, grandTotal = 0;

                foreach (var item in data)
                {
                    ws.Cells[row, 1].Value = slNo++;
                    ws.Cells[row, 2].Value = item.VehicleNo ?? "";
                    ws.Cells[row, 3].Value = item.VehicleType ?? "";
                    ws.Cells[row, 4].Value = item.TransportCapacity.Value.ToString() ?? "";
                    ws.Cells[row, 5].Value = item.FullName ?? "";
                    ws.Cells[row, 6].Value = item.HelperName ?? "";

                    // Use formatted values for display
                    ws.Cells[row, 7].Value = FormatCurrency(item.CNGGasBill);
                    ws.Cells[row, 8].Value = FormatCurrency(item.RMBill);
                    ws.Cells[row, 9].Value = FormatCurrency(item.FuelOctaneBill);
                    ws.Cells[row, 10].Value = FormatCurrency(item.PoliceDonation);
                    ws.Cells[row, 11].Value = FormatCurrency(item.TollOthersBill);
                    ws.Cells[row, 12].Value = FormatCurrency(item.TAXFitnessAndRutePermit);
                    ws.Cells[row, 13].Value = FormatCurrency(item.SalaryDriverHelper);
                    ws.Cells[row, 14].Value = FormatCurrency(item.MechanicSalary);
                    ws.Cells[row, 15].Value = FormatCurrency(item.MonthlyPoliceDonation);
                    ws.Cells[row, 16].Value = FormatCurrency(item.MonthlyEngOilPurchase);
                    ws.Cells[row, 17].Value = FormatCurrency(item.AkeshTechnology);
                    ws.Cells[row, 18].Value = FormatCurrency(item.GarageRent);
                    ws.Cells[row, 19].Value = FormatCurrency(item.TotalExpense);

                    // Right align numeric columns
                    for (int col = 7; col <= 19; col++)
                    {
                        ws.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }

                    // Border for each cell
                    for (int col = 1; col <= headers.Length; col++)
                        ws.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                    // Accumulate totals for calculation (use actual numeric values)
                    totalCNG += item.CNGGasBill;
                    totalRM += item.RMBill;
                    totalFuel += item.FuelOctaneBill;
                    totalPolice += item.PoliceDonation;
                    totalToll += item.TollOthersBill;
                    totalTax += item.TAXFitnessAndRutePermit;
                    totalSalary += item.SalaryDriverHelper;
                    totalMechanic += item.MechanicSalary;
                    totalMonthlyPolice += item.MonthlyPoliceDonation;
                    totalEngOil += item.MonthlyEngOilPurchase;
                    totalAkesh += item.AkeshTechnology;
                    totalGarage += item.GarageRent;
                    grandTotal += item.TotalExpense;

                    row++;
                }

                // ==========================
                // Totals Row with Merge
                // ==========================
                // Merge first 6 columns for "Total:" label
                ws.Cells[row, 1, row, 6].Merge = true;
                ws.Cells[row, 1].Value = "Total:";
                ws.Cells[row, 1].Style.Font.Bold = true;
                ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                ws.Cells[row, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // Set total values in numeric columns
                ws.Cells[row, 7].Value = FormatCurrency(totalCNG);
                ws.Cells[row, 8].Value = FormatCurrency(totalRM);
                ws.Cells[row, 9].Value = FormatCurrency(totalFuel);
                ws.Cells[row, 10].Value = FormatCurrency(totalPolice);
                ws.Cells[row, 11].Value = FormatCurrency(totalToll);
                ws.Cells[row, 12].Value = FormatCurrency(totalTax);
                ws.Cells[row, 13].Value = FormatCurrency(totalSalary);
                ws.Cells[row, 14].Value = FormatCurrency(totalMechanic);
                ws.Cells[row, 15].Value = FormatCurrency(totalMonthlyPolice);
                ws.Cells[row, 16].Value = FormatCurrency(totalEngOil);
                ws.Cells[row, 17].Value = FormatCurrency(totalAkesh);
                ws.Cells[row, 18].Value = FormatCurrency(totalGarage);
                ws.Cells[row, 19].Value = FormatCurrency(grandTotal);

                // Style the totals row
                for (int col = 1; col <= headers.Length; col++)
                {
                    ws.Cells[row, col].Style.Font.Bold = true;
                    ws.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[row, col].Style.Fill.BackgroundColor.SetColor(Color.White);
                    ws.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                    // Right align numeric columns
                    if (col >= 7)
                    {
                        ws.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        ws.Cells[row, 1, row, headers.Length].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                }

                // Double border for totals row bottom
                //ws.Cells[row, 1, row, headers.Length].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
                ws.Cells[row, 7, row, headers.Length].Style.Font.UnderLine = true;


                ws.Cells[1, 1, row, headers.Length].AutoFitColumns();

                var fileBytes = package.GetAsByteArray();
                string fileName = $"Transport_Expense_Statement.xlsx";

                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
    }
}
