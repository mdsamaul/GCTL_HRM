using GCTL.Core.ViewModels.EmployeeLoanInformationReport;
using GCTL.Service.EmployeeLoanInformationReport;
using GCTL.UI.Core.ViewModels.EmployeeLoanInformationReport;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace GCTL.UI.Core.Controllers
{
    public class EmployeeLoanInformationReportController : BaseController
    {
        private readonly IEmployeeLoanInformationReportServices employeeLoanInformationReportServices;

        public EmployeeLoanInformationReportController(
                IEmployeeLoanInformationReportServices employeeLoanInformationReportServices
            )
        {
            this.employeeLoanInformationReportServices = employeeLoanInformationReportServices;
        }
        public async Task<IActionResult> Index()
        {
            var hasPermission = await employeeLoanInformationReportServices.PagePermissionAsync(LoginInfo.AccessCode);

            if (!hasPermission)

            {

                return RedirectToAction("Login", "Accounts");

            }
            EmployeeLoanInformationReportSetupVM model = new EmployeeLoanInformationReportSetupVM()
            {
                PageUrl = Url.Action(nameof(Index))
            };
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> GetLoanDetails([FromQuery] LoanFilterVM filter)
        {
            var data = await employeeLoanInformationReportServices.GetLoanDetailsAsync(filter);
            return Json(data);
        }

        [HttpPost]
        public IActionResult GenerateExcel([FromBody] List<EmployeeLoanInformationReportVM> installmentData)
        {
            try
            {
                if (installmentData == null || !installmentData.Any())
                {
                    return BadRequest("No data available for Excel report");
                }

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage())
                {
                    for (int loanIndex = 0; loanIndex < installmentData.Count; loanIndex++)
                    {
                        var loan = installmentData[loanIndex];
                        var worksheet = package.Workbook.Worksheets.Add($"Loan_{loanIndex + 1}");

                        GenerateLoanSheet(worksheet, loan);
                    }

                    var fileBytes = package.GetAsByteArray();
                    return File(fileBytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Loan_Report.xlsx");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generating Excel: {ex.Message}");
            }
        }


        //FormateAmountEN = function(amount)
        //{
        //    if (amount >= 10000000)
        //    {
        //        return (amount / 10000000).toFixed(1).replace(/\.0$/, '') + 'Cr';
        //    }
        //    else if (amount >= 100000)
        //    {
        //        return (amount / 100000).toFixed(1).replace(/\.0$/, '') + 'Lakh';
        //    }
        //    else if (amount >= 1000)
        //    {
        //        return (amount / 1000).toFixed(1).replace(/\.0$/, '') + 'K';
        //    }
        //    else
        //    {
        //        return amount + ' TK';
        //    }
        //}

        private string CalculteAmount(decimal amount)
        {
            if (amount >= 10000000)
            {
                return (amount / 10000000M).ToString("0.#") + "Cr";
            }
            else if (amount >= 100000)
            {
                return (amount / 100000M).ToString("0.#") + "Lakh";
            }
            else if (amount >= 1000)
            {
                return (amount / 1000M).ToString("0.#") + "K";
            }
            else
            {
                return amount.ToString("0.#") + " TK";
            }
        }


        private void GenerateLoanSheet(ExcelWorksheet worksheet, EmployeeLoanInformationReportVM loan)
        {
            int currentRow = 1;

            // Company Name Header
            worksheet.Cells[currentRow, 1, currentRow, 5].Merge = true;
            worksheet.Cells[currentRow, 1].Value = loan.CompanyName ?? "Company Name";
            worksheet.Cells[currentRow, 1].Style.Font.Size = 18;
            worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[currentRow, 1, currentRow, 5].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            currentRow += 2;

            // Employee Loan Information Header
            worksheet.Cells[currentRow, 1, currentRow, 5].Merge = true;
            worksheet.Cells[currentRow, 1].Value = "Employee Loan Information";
            worksheet.Cells[currentRow, 1].Style.Font.Size = 14;
            worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[currentRow, 1, currentRow, 5].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            currentRow += 2;

            // Employee Information Section
            var employeeInfo = new[]
            {
            new { Label = "Employee ID", Value = loan.EmployeeID?.ToString() ?? "" },
            new { Label = "Name", Value = loan.FullName ?? "" },
            new { Label = "Department", Value = loan.DepartmentName ?? "" },
            new { Label = "Designation", Value = loan.DesignationName ?? "" },
            new { Label = "Reason of Loan taken", Value = loan.Reason ?? "" },
            new { Label = "Loan Number", Value = loan.TotalLoans.ToString() },
            new { Label = "Loan Amount", Value = $"BDT {loan.LoanAmount:F2}" },
            new { Label = "Loan Disbursed Date", Value = loan.StartDate.ToShortDateString() },
            new { Label = "Loan Repayment Method", Value = loan.LoanRepaymentMethod ?? "" },
            //new { Label = "Installment Details", Value = loan.InstallmentDetails ?? "" },
             new { Label = "Installment Details", Value = "Installments of " + CalculteAmount(loan.MonthlyDeduction) + " tk per month to be deposited to designated account." },
            new { Label = "Loan Paidout Date", Value = loan.EndDate.ToShortDateString() },
            new { Label = "Remarks", Value = loan.Remarks ?? "" }
        };

            foreach (var info in employeeInfo)
            {
                // Label in column 2
                worksheet.Cells[currentRow, 1, currentRow, 2].Merge = true;
                worksheet.Cells[currentRow, 1].Value = info.Label;
                worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[currentRow, 1].Style.Font.Bold = true;

                // Value with colon, merged from column 3 to 5
                worksheet.Cells[currentRow, 3, currentRow, 5].Merge = true;
                worksheet.Cells[currentRow, 3].Value = ": " + info.Value;
                worksheet.Cells[currentRow, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                // Optional: Add border around full row (column 2 to 5)
                worksheet.Cells[currentRow, 1, currentRow, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                currentRow++;
            }

            currentRow += 1;
            // Employee Loan Information Header
            worksheet.Cells[currentRow, 1, currentRow, 5].Merge = true;
            worksheet.Cells[currentRow, 1].Value = "Loan Installment Details";
            worksheet.Cells[currentRow, 1].Style.Font.Size = 14;
            worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[currentRow, 1, currentRow, 5].Style.Border.BorderAround(ExcelBorderStyle.Medium);
            currentRow += 2;
            // Installments Table Header
            var headers = new[] { "Installment No", "Transaction Date", "Installment Details (Cash/Cheque)", "Deposit Amount", "Outstanding Balance" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[currentRow, i + 1].Value = headers[i];
                worksheet.Cells[currentRow, i + 1].Style.Font.Bold = true;
                worksheet.Cells[currentRow, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[currentRow, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                worksheet.Cells[currentRow, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[currentRow, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
            currentRow++;

            // Installments Data
            decimal totalDeposit = 0;
            if (loan.Installments != null && loan.Installments.Any())
            {
                foreach (var installment in loan.Installments)
                {
                    worksheet.Cells[currentRow, 1].Value = installment.InstallmentNo;
                    worksheet.Cells[currentRow, 2].Value = installment.InstallmentDate ?? "";
                    worksheet.Cells[currentRow, 3].Value = installment.PaymentMode ?? "";

                    if (installment.Deposit > 0)
                    {
                        worksheet.Cells[currentRow, 4].Value = $"BDT {installment.Deposit:F2}";
                        totalDeposit += installment.Deposit;
                    }
                    else
                    {
                        worksheet.Cells[currentRow, 4].Value = "";
                    }

                    worksheet.Cells[currentRow, 5].Value = $"BDT {installment.OutstandingBalance:F2}";

                    // Styling for data rows
                    for (int col = 1; col <= 5; col++)
                    {
                        worksheet.Cells[currentRow, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        if (col == 1) worksheet.Cells[currentRow, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        if (col >= 4) worksheet.Cells[currentRow, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }
                    currentRow++;
                }
            }

            // Total Section
            currentRow += 1;
            worksheet.Cells[currentRow, 4].Value = $"BDT {totalDeposit:F2}";
            worksheet.Cells[currentRow, 4].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            // Add top and bottom borders for total
            worksheet.Cells[currentRow, 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[currentRow, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

            // Column widths
            worksheet.Column(1).Width = 15;  // Installment No
            worksheet.Column(2).Width = 20;  // Transaction Date
            worksheet.Column(3).Width = 25;  // Installment Details
            worksheet.Column(4).Width = 18;  // Deposit Amount
            worksheet.Column(5).Width = 20;  // Outstanding Balance

            // Auto fit if needed
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }

    }
}


