using GCTL.Core.Data;
using GCTL.Core.ViewModels.MonthlyTransportExpenseDetailsReport;
using GCTL.Data.Models;
using GCTL.Service.MonthlyTransportExpenseDetailsReportService;
using GCTL.UI.Core.ViewModels.MonthlyTransportExpenseDetailsReport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace GCTL.UI.Core.Controllers
{
    public class MonthlyTransportExpenseDetailsReportController : BaseController
    {
        private readonly IMonthlyTransportExpenseDetailsReportService monthlyTransportExpenseDetailsReportServices;
        private readonly IRepository<HrmPayMonth> monthRepo;

        public MonthlyTransportExpenseDetailsReportController(
            IMonthlyTransportExpenseDetailsReportService monthlyTransportExpenseDetailsReportServices,
             IRepository<HrmPayMonth> monthRepo
            )
        {
            this.monthlyTransportExpenseDetailsReportServices = monthlyTransportExpenseDetailsReportServices;
            this.monthRepo = monthRepo;
        }
        public async Task<IActionResult> Index()
        {
            var hasPermission = await monthlyTransportExpenseDetailsReportServices.PagePermissionAsync(LoginInfo.AccessCode);

            if (!hasPermission)

            {

                return RedirectToAction("Login", "Accounts");

            }
            ViewBag.MonthList = new SelectList(monthRepo.GetAll().Select(x => new { x.MonthId, x.MonthName }), "MonthId", "MonthName");
            MonthlyTransportExpenseDetailsReportViewModel model = new MonthlyTransportExpenseDetailsReportViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> GetFilterDropdownDataReport([FromBody] MonthlyTransportExpenseDetailsReportFilterDataDto FilterData)
        {
            var data = await monthlyTransportExpenseDetailsReportServices.GetAllTransportExpenseStatementDropdownSelectReportAsync(FilterData);

            return Ok(data);
        }
        [HttpPost]
        public async Task<IActionResult> GetFilterResultReport([FromBody] MonthlyTransportExpenseDetailsReportFilterDataDto FilterData)
        {
            var data = await monthlyTransportExpenseDetailsReportServices.GetAllTransportExpenseStatementResultReportAsync(FilterData);

            return Json(new { data = data });
        }


        [HttpPost]
        public async Task<IActionResult> ExportToExcel([FromBody] MonthlyTransportExpenseDetailsReportFilterDataDto filter)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var dataGroups = await monthlyTransportExpenseDetailsReportServices.GetAllTransportExpenseStatementResultReportAsync(filter);

            if (dataGroups == null || dataGroups.Count == 0)
            {
                return Json(new { isSuccess = false, message = "No data found" });
            }

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("TransportExpense");

                // ==========================
                // Title
                // ==========================
                ws.Cells["A1:S1"].Merge = true;
                ws.Cells["A1"].Value = "Monthly Transport Expense Details Report";
                ws.Cells["A1"].Style.Font.Bold = true;
                ws.Cells["A1"].Style.Font.Size = 16;
                ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A1"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                // ==========================
                // Subtitle / Date Range
                // ==========================
                ws.Cells["A2:S2"].Merge = true;

                string subtitle = "";
                bool isByDate = filter.FromDate.HasValue && filter.ToDate.HasValue &&
                                filter.FromDate.Value != DateTime.MinValue && filter.ToDate.Value != DateTime.MinValue;
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
                // Table Headers
                // ==========================
                string[] headers = {
            "SL.No.", "Transport.No", "Transport Type", "Transport Capacity/Persons", "Driver Name",
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

                int slNo = 1;

                decimal grandCNG = 0, grandRM = 0, grandFuel = 0, grandPolice = 0, grandToll = 0;
                decimal grandTax = 0, grandSalary = 0, grandMechanic = 0, grandMonthlyPolice = 0;
                decimal grandEngOil = 0, grandAkesh = 0, grandGarage = 0, grandTotal = 0;

                foreach (var group in dataGroups)
                {
                    // ---------- Date Header ----------
                    ws.Cells[row, 1].Value = $"Date: {group.ReportDate:dd/MM/yyyy}";
                    ws.Cells[row, 1, row, headers.Length].Merge = true;
                    ws.Cells[row, 1].Style.Font.Bold = true;
                    ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells[row, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    row++;

                    decimal subCNG = 0, subRM = 0, subFuel = 0, subPolice = 0, subToll = 0;
                    decimal subTax = 0, subSalary = 0, subMechanic = 0, subMonthlyPolice = 0;
                    decimal subEngOil = 0, subAkesh = 0, subGarage = 0, subTotal = 0;

                    foreach (var item in group.ReportList)
                    {
                        // SL.No - Center
                        ws.Cells[row, 1].Value = slNo++;
                        ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        // Transport.No - Left
                        ws.Cells[row, 2].Value = item.VehicleNo ?? "";
                        ws.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // Transport Type - Left
                        ws.Cells[row, 3].Value = item.VehicleType ?? "";
                        ws.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // Transport Capacity - Center
                        ws.Cells[row, 4].Value = item.TransportCapacity ?? "";
                        ws.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        // Driver Name - Left
                        ws.Cells[row, 5].Value = item.FullName ?? "";
                        ws.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // Helper Name - Left
                        ws.Cells[row, 6].Value = item.HelperName ?? "";
                        ws.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // Numeric Values - Right aligned with formatting
                        ws.Cells[row, 7].Value = FormatValue(item.CNGGasBill);
                        ws.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        ws.Cells[row, 8].Value = FormatValue(item.RMBill);
                        ws.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        ws.Cells[row, 9].Value = FormatValue(item.FuelOctaneBill);
                        ws.Cells[row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        ws.Cells[row, 10].Value = FormatValue(item.PoliceDonation);
                        ws.Cells[row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        ws.Cells[row, 11].Value = FormatValue(item.TollOthersBill);
                        ws.Cells[row, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        ws.Cells[row, 12].Value = FormatValue(item.TaxFitnessAndRoutePermit);
                        ws.Cells[row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        ws.Cells[row, 13].Value = FormatValue(item.SalaryDriverHelper);
                        ws.Cells[row, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        ws.Cells[row, 14].Value = FormatValue(item.MechanicSalary);
                        ws.Cells[row, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        ws.Cells[row, 15].Value = FormatValue(item.MonthlyPoliceDonation);
                        ws.Cells[row, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        ws.Cells[row, 16].Value = FormatValue(item.MonthlyEngineOilPurchase);
                        ws.Cells[row, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        ws.Cells[row, 17].Value = FormatValue(item.AkeshTechnology);
                        ws.Cells[row, 17].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        ws.Cells[row, 18].Value = FormatValue(item.GarageRent);
                        ws.Cells[row, 18].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        ws.Cells[row, 19].Value = FormatValue(item.TotalExpense);
                        ws.Cells[row, 19].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        // Border for each cell
                        for (int col = 1; col <= headers.Length; col++)
                        {
                            ws.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        }

                        // Subtotals (using actual numeric values for calculation)
                        subCNG += item.CNGGasBill;
                        subRM += item.RMBill;
                        subFuel += item.FuelOctaneBill;
                        subPolice += item.PoliceDonation;
                        subToll += item.TollOthersBill;
                        subTax += item.TaxFitnessAndRoutePermit;
                        subSalary += item.SalaryDriverHelper;
                        subMechanic += item.MechanicSalary;
                        subMonthlyPolice += item.MonthlyPoliceDonation;
                        subEngOil += item.MonthlyEngineOilPurchase;
                        subAkesh += item.AkeshTechnology;
                        subGarage += item.GarageRent;
                        subTotal += item.TotalExpense;

                        row++;
                    }

                    // ---------- Subtotal Row ----------
                    ws.Cells[row, 1, row, 6].Merge = true;
                    ws.Cells[row, 1].Value = "Total:";
                    ws.Cells[row, 1].Style.Font.Bold = true;
                    ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    ws.Cells[row, 7].Value = FormatValue(subCNG);
                    ws.Cells[row, 8].Value = FormatValue(subRM);
                    ws.Cells[row, 9].Value = FormatValue(subFuel);
                    ws.Cells[row, 10].Value = FormatValue(subPolice);
                    ws.Cells[row, 11].Value = FormatValue(subToll);
                    ws.Cells[row, 12].Value = FormatValue(subTax);
                    ws.Cells[row, 13].Value = FormatValue(subSalary);
                    ws.Cells[row, 14].Value = FormatValue(subMechanic);
                    ws.Cells[row, 15].Value = FormatValue(subMonthlyPolice);
                    ws.Cells[row, 16].Value = FormatValue(subEngOil);
                    ws.Cells[row, 17].Value = FormatValue(subAkesh);
                    ws.Cells[row, 18].Value = FormatValue(subGarage);
                    ws.Cells[row, 19].Value = FormatValue(subTotal);

                    for (int col = 1; col <= headers.Length; col++)
                    {
                        ws.Cells[row, col].Style.Font.Bold = true;
                        ws.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[row, col].Style.Fill.BackgroundColor.SetColor(Color.White);
                        ws.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        ws.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }

                    // Add to grand totals
                    grandCNG += subCNG;
                    grandRM += subRM;
                    grandFuel += subFuel;
                    grandPolice += subPolice;
                    grandToll += subToll;
                    grandTax += subTax;
                    grandSalary += subSalary;
                    grandMechanic += subMechanic;
                    grandMonthlyPolice += subMonthlyPolice;
                    grandEngOil += subEngOil;
                    grandAkesh += subAkesh;
                    grandGarage += subGarage;
                    grandTotal += subTotal;

                    row += 2;
                }

                // ---------- Grand Total Row ----------
                ws.Cells[row, 1, row, 6].Merge = true;
                ws.Cells[row, 1].Value = "Grand Total:";
                ws.Cells[row, 1].Style.Font.Bold = true;
                ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                ws.Cells[row, 7].Value = FormatValue(grandCNG);
                ws.Cells[row, 8].Value = FormatValue(grandRM);
                ws.Cells[row, 9].Value = FormatValue(grandFuel);
                ws.Cells[row, 10].Value = FormatValue(grandPolice);
                ws.Cells[row, 11].Value = FormatValue(grandToll);
                ws.Cells[row, 12].Value = FormatValue(grandTax);
                ws.Cells[row, 13].Value = FormatValue(grandSalary);
                ws.Cells[row, 14].Value = FormatValue(grandMechanic);
                ws.Cells[row, 15].Value = FormatValue(grandMonthlyPolice);
                ws.Cells[row, 16].Value = FormatValue(grandEngOil);
                ws.Cells[row, 17].Value = FormatValue(grandAkesh);
                ws.Cells[row, 18].Value = FormatValue(grandGarage);
                ws.Cells[row, 19].Value = FormatValue(grandTotal);

                for (int col = 1; col <= headers.Length; col++)
                {
                    ws.Cells[row, col].Style.Font.Bold = true;
                    ws.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[row, col].Style.Fill.BackgroundColor.SetColor(Color.White);
                    ws.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    ws.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                }

                ws.Cells[1, 1, row, headers.Length].AutoFitColumns();

                var fileBytes = package.GetAsByteArray();
                string fileName = $"Monthly_Transport_Expense_Report.xlsx";

                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

        /// <summary>
        /// Format helper
        /// </summary>
        private object FormatValue(decimal? val)
        {
            if (!val.HasValue || val.Value == 0) return "-";
            return string.Format("{0:N2}", val.Value);
        }


    }
}
