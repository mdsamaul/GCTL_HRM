using Dapper;
using GCTL.Core.ViewModels.SalaryInformationReport;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace GCTL.Service.SalaryInformationReport
{
    public class SalaryInformationReportService:ISalaryInformationReportService
    {
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _env;

        // IWebHostEnvironment eta DI container theke automatic ashbe.
        // Controller theke direct pass korar dorkar nai — Startup/Program.cs e
        // service ke DI te register korle ASP.NET Core nijei eta inject kore dibe.
        public SalaryInformationReportService(IConfiguration configuration, IWebHostEnvironment env)
        {
            _connectionString = configuration.GetConnectionString("ApplicationDbConnection");
            _env = env;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task<List<SalaryInformationReportDto>> GetPayrollMasterFileAsync(SalaryInformationReportFilterDto filter)
        {
            using IDbConnection db = new SqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@CompanyCode", NullIfEmpty(filter.CompanyCode));
            parameters.Add("@BranchCode", NullIfEmpty(filter.BranchCode));
            parameters.Add("@DepartmentCode", NullIfEmpty(filter.DepartmentCode));
            parameters.Add("@EmployeeID", NullIfEmpty(filter.EmployeeID));
            parameters.Add("@ModeOfPayment", NullIfEmpty(filter.ModeOfPayment));
            parameters.Add("@EmploymentNature", NullIfEmpty(filter.EmploymentNature));
            parameters.Add("@GenerateType", NullIfEmpty(filter.GenerateType));
            parameters.Add("@DateFrom", filter.DateFrom);
            parameters.Add("@DateTo", filter.DateTo);
            parameters.Add("@MonthName", NullIfEmpty(filter.MonthName));
            parameters.Add("@YearName", filter.YearName);
            parameters.Add("@AsOnDate", filter.AsOnDate);

            // SP er output column alias e space/period ache (e.g. "SL.", "ID NO."),
            // tai dynamic row hishebe poira IDictionary<string,object> theke
            // exact bracketed name diye map kora hoise.
            var rows = await db.QueryAsync(
                "dbo.usp_GetPayrollMasterFile_General",
                parameters,
                commandType: CommandType.StoredProcedure,
                commandTimeout: 120);

            return rows.Select(r => MapDynamicToDto((IDictionary<string, object>)r)).ToList();
        }

        public async Task<byte[]> ExportToExcelAsync(SalaryInformationReportFilterDto filter)
        {
            try
            {
                var data = await GetPayrollMasterFileAsync(filter);

                using var package = new ExcelPackage();
                var ws = package.Workbook.Worksheets.Add("PayrollMasterFile_General");

                const int logoColSpan = 3;
                const int firstDataCol = 1;
                var headers = new[]
                {
            "SN", "ID NO.", "Pay ID", "DP User ID", "DBBL Employees Name",
            "UCBL Employees Name", "Status", "DEPARTMENT", "DESIGNATION",
            "DOH", "DOT", "Duration", "DBBL", "UCBL", "Salary",
            "Yearly Bonus Eligibility", "Gratuity Eligibility",
            "Eid Bonus Eligibility", "PF Eligiblity", "Gender", "Cell Phone",
            "Special Notes", "End of Probation"
        };
                int totalCols = headers.Length;
                int headerRow = 5;

                // ===== Logo (left side, no background fill) =====
                var logoPath = Path.Combine(_env.WebRootPath ?? "wwwroot", "images", "DPL.jpeg");

                ws.Cells[1, 1, 3, logoColSpan].Merge = true;
                ws.Cells[1, 1, 3, logoColSpan].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[1, 1, 3, logoColSpan].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                if (File.Exists(logoPath))
                {
                    var picture = ws.Drawings.AddPicture("CompanyLogo", new FileInfo(logoPath));
                    picture.SetSize(110, 40);
                    picture.SetPosition(0, 10, 0, 15);
                }

                string periodText = BuildPeriodText(filter);

                ws.Cells[1, logoColSpan + 1, 1, totalCols].Merge = true;
                ws.Cells[2, logoColSpan + 1, 2, totalCols].Merge = true;
                ws.Cells[3, logoColSpan + 1, 3, totalCols].Merge = true;

                var companyCell = ws.Cells[1, logoColSpan + 1];
                companyCell.Value = "DataPath Ltd.";
                companyCell.Style.Font.Name = "Times New Roman";
                companyCell.Style.Font.Size = 16;
                companyCell.Style.Font.Bold = true;
                companyCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                companyCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                var reportCell = ws.Cells[2, logoColSpan + 1];
                reportCell.Value = "Payroll Master File - General";
                reportCell.Style.Font.Name = "Times New Roman";
                reportCell.Style.Font.Size = 13;
                reportCell.Style.Font.Bold = true;
                reportCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                reportCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                var periodCell = ws.Cells[3, logoColSpan + 1];
                periodCell.Value = periodText;
                periodCell.Style.Font.Name = "Times New Roman";
                periodCell.Style.Font.Size = 10;
                periodCell.Style.Font.Italic = true;
                periodCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                periodCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // Column headers - NO background fill, just bold + border + center
                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = ws.Cells[headerRow, firstDataCol + i];
                    cell.Value = headers[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.Font.Name = "Times New Roman";
                    cell.Style.Font.Size = 10;
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    cell.Style.WrapText = true;
                }

                var centerAlignColumns = new HashSet<string>
        {
            "SN", "ID NO.", "Pay ID", "Status", "DOH", "DOT", "Duration",
            "DBBL", "UCBL", "Yearly Bonus Eligibility", "Gratuity Eligibility",
            "Eid Bonus Eligibility", "PF Eligiblity", "Gender", "Cell Phone",
            "Special Notes", "End of Probation"
        };
                var colCenterFlags = new bool[headers.Length];
                for (int i = 0; i < headers.Length; i++)
                    colCenterFlags[i] = centerAlignColumns.Contains(headers[i]);

                // Track max text length per column (start with header length) for auto width
                var maxLen = new int[totalCols];
                for (int i = 0; i < headers.Length; i++)
                    maxLen[i] = headers[i]?.Length ?? 0;

                void TrackLen(int colIdx0, object val)
                {
                    if (val == null) return;
                    var s = val.ToString();
                    if (s.Length > maxLen[colIdx0]) maxLen[colIdx0] = s.Length;
                }

                int r = headerRow + 1;
                foreach (var row in data)
                {
                    int c = firstDataCol;

                    ws.Cells[r, c].Value = row.SL; TrackLen(c - firstDataCol, row.SL); c++;
                    ws.Cells[r, c].Value = row.IdNo; TrackLen(c - firstDataCol, row.IdNo); c++;
                    ws.Cells[r, c].Value = row.PayId; TrackLen(c - firstDataCol, row.PayId); c++;
                    ws.Cells[r, c].Value = row.DpUserId; TrackLen(c - firstDataCol, row.DpUserId); c++;
                    ws.Cells[r, c].Value = row.DbblEmployeesName; TrackLen(c - firstDataCol, row.DbblEmployeesName); c++;
                    ws.Cells[r, c].Value = row.UcblEmployeesName; TrackLen(c - firstDataCol, row.UcblEmployeesName); c++;
                    ws.Cells[r, c].Value = row.Status; TrackLen(c - firstDataCol, row.Status); c++;
                    ws.Cells[r, c].Value = row.Department; TrackLen(c - firstDataCol, row.Department); c++;
                    ws.Cells[r, c].Value = row.Designation; TrackLen(c - firstDataCol, row.Designation); c++;
                    ws.Cells[r, c].Value = row.Doh; TrackLen(c - firstDataCol, row.Doh); c++;
                    ws.Cells[r, c].Value = row.Dot; TrackLen(c - firstDataCol, row.Dot); c++;

                    ws.Cells[r, c].Value = row.Duration;
                    ws.Cells[r, c].Style.Numberformat.Format = "0.00";
                    TrackLen(c - firstDataCol, row.Duration?.ToString("0.00"));
                    c++;

                    ws.Cells[r, c].Value = row.Dbbl; TrackLen(c - firstDataCol, row.Dbbl); c++;
                    ws.Cells[r, c].Value = row.Ucbl; TrackLen(c - firstDataCol, row.Ucbl); c++;

                    ws.Cells[r, c].Value = row.Salary;
                    ws.Cells[r, c].Style.Numberformat.Format = "#,##0.00";
                    TrackLen(c - firstDataCol, row.Salary?.ToString("#,##0.00"));
                    c++;

                    ws.Cells[r, c].Value = row.YearlyBonusEligibility; TrackLen(c - firstDataCol, row.YearlyBonusEligibility); c++;
                    ws.Cells[r, c].Value = row.GratuityEligibility; TrackLen(c - firstDataCol, row.GratuityEligibility); c++;

                    ws.Cells[r, c].Value = row.EidBonusEligibility;
                    ws.Cells[r, c].Style.Numberformat.Format = "0.00";
                    TrackLen(c - firstDataCol, row.EidBonusEligibility?.ToString("0.00"));
                    c++;

                    ws.Cells[r, c].Value = row.PfEligiblity;
                    ws.Cells[r, c].Style.Numberformat.Format = "0.00";
                    TrackLen(c - firstDataCol, row.PfEligiblity?.ToString("0.00"));
                    c++;

                    ws.Cells[r, c].Value = row.Gender; TrackLen(c - firstDataCol, row.Gender); c++;
                    ws.Cells[r, c].Value = row.CellPhone; TrackLen(c - firstDataCol, row.CellPhone); c++;
                    ws.Cells[r, c].Value = row.SpecialNotes; TrackLen(c - firstDataCol, row.SpecialNotes); c++;
                    ws.Cells[r, c].Value = row.EndOfProbation; TrackLen(c - firstDataCol, row.EndOfProbation); c++;
                    ws.Cells[r, c].Value = row.ModeOfPayment; TrackLen(c - firstDataCol, row.ModeOfPayment); c++;
                    ws.Cells[r, c].Value = row.EmploymentNature; TrackLen(c - firstDataCol, row.EmploymentNature); c++;

                    for (int cc = firstDataCol; cc < firstDataCol + totalCols; cc++)
                    {
                        var dataCell = ws.Cells[r, cc];
                        dataCell.Style.Font.Name = "Times New Roman";
                        dataCell.Style.Font.Size = 9;
                        dataCell.Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        int headerIdx = cc - firstDataCol;
                        if (headerIdx >= 0 && headerIdx < colCenterFlags.Length && colCenterFlags[headerIdx])
                        {
                            dataCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            dataCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        }
                    }
                    r++;
                }

                int totalRow = r;
                ws.Cells[totalRow, firstDataCol].Value = "Total";
                ws.Cells[totalRow, firstDataCol].Style.Font.Bold = true;
                ws.Cells[totalRow, firstDataCol, totalRow, firstDataCol + 13].Merge = true;
                ws.Cells[totalRow, firstDataCol].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                int salaryColIndex = Array.IndexOf(headers, "Salary") + firstDataCol;
                if (data.Count > 0)
                {
                    var salaryColLetter = ExcelCellAddress.GetColumnLetter(salaryColIndex);
                    ws.Cells[totalRow, salaryColIndex].Formula =
                        $"SUM({salaryColLetter}{headerRow + 1}:{salaryColLetter}{r - 1})";
                }
                ws.Cells[totalRow, salaryColIndex].Style.Numberformat.Format = "#,##0.00";
                ws.Cells[totalRow, salaryColIndex].Style.Font.Bold = true;
                ws.Cells[totalRow, firstDataCol].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                r += 2;
                ws.Cells[r, firstDataCol].Value = "Grand Total";
                ws.Cells[r, firstDataCol].Style.Font.Bold = true;

                // ===== Auto width: each column = widest text in it (header or data) =====
                ws.Column(1).Width = 6; // SN stays compact
                for (int i = 1; i < totalCols; i++)
                {
                    double width = Math.Max(maxLen[i] + 2, 8); // padding + minimum width
                    ws.Column(firstDataCol + i).Width = width;
                }

                // ===== Page Setup – যত column আছে সব এক পেজে fit হবে =====
                ws.PrinterSettings.Orientation = eOrientation.Landscape;
                ws.PrinterSettings.PaperSize = ePaperSize.A3;
                ws.PrinterSettings.FitToPage = true;
                ws.PrinterSettings.FitToWidth = 1;      // সব column ১ পেজে
                ws.PrinterSettings.FitToHeight = 0;     // height unlimited
                ws.PrinterSettings.HorizontalCentered = true;
                ws.PrinterSettings.VerticalCentered = false;

                // margins কমিয়ে আরও জায়গা পাওয়া যায়
                ws.PrinterSettings.LeftMargin = 0.25m;
                ws.PrinterSettings.RightMargin = 0.25m;
                ws.PrinterSettings.TopMargin = 0.4m;
                ws.PrinterSettings.BottomMargin = 0.4m;

                ws.View.FreezePanes(headerRow + 1, firstDataCol + 2);

                return await package.GetAsByteArrayAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static string NullIfEmpty(string s) => string.IsNullOrWhiteSpace(s) ? null : s;

        private static string BuildPeriodText(SalaryInformationReportFilterDto filter)
        {
            if (string.Equals(filter.GenerateType, "ByMonth", StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrWhiteSpace(filter.MonthName) && filter.YearName.HasValue)
            {
                return $"For the month of {filter.MonthName}, {filter.YearName}";
            }
            if (string.Equals(filter.GenerateType, "ByDate", StringComparison.OrdinalIgnoreCase)
                && filter.DateFrom.HasValue && filter.DateTo.HasValue)
            {
                return $"For the period {filter.DateFrom:dd/MM/yyyy} to {filter.DateTo:dd/MM/yyyy}";
            }
            return $"Generated on {DateTime.Now:dd/MM/yyyy hh:mm tt}";
        }

        // Dynamic row (bracketed SP column names) -> DTO mapping
        private static SalaryInformationReportDto MapDynamicToDto(IDictionary<string, object> r)
        {
            T Get<T>(string key)
            {
                if (!r.TryGetValue(key, out var val) || val == null || val == DBNull.Value)
                    return default;
                return (T)Convert.ChangeType(val, typeof(T));
            }

            string GetStr(string key)
            {
                if (!r.TryGetValue(key, out var val) || val == null || val == DBNull.Value)
                    return null;
                return val.ToString();
            }

            decimal? GetDec(string key)
            {
                if (!r.TryGetValue(key, out var val) || val == null || val == DBNull.Value)
                    return null;
                return Convert.ToDecimal(val);
            }

            return new SalaryInformationReportDto
            {
                SL = Get<int>("SL."),
                IdNo = GetStr("ID NO."),
                PayId = GetStr("Pay ID"),
                DpUserId = GetStr("DP User ID"),
                DbblEmployeesName = GetStr("DBBL Employees Name"),
                UcblEmployeesName = GetStr("UCBL Employees Name"),
                Status = GetStr("Status"),
                Department = GetStr("DEPARTMENT"),
                Designation = GetStr("DESIGNATION"),
                Doh = GetStr("DOH"),
                Dot = GetStr("DOT"),
                Duration = GetDec("Duration"),
                Dbbl = GetStr("DBBL"),
                Ucbl = GetStr("UCBL"),
                Salary = GetDec("Salary"),
                YearlyBonusEligibility = GetStr("Yearly Bonus Eligibility"),
                GratuityEligibility = GetStr("Gratuity Eligibility"),
                EidBonusEligibility = GetDec("Eid Bonus Eligibility"),
                PfEligiblity = GetDec("PF Eligiblity"),
                Gender = GetStr("Gender"),
                CellPhone = GetStr("Cell Phone"),
                SpecialNotes = GetStr("Special Notes"),
                EndOfProbation = GetStr("End of Probation")
            };
        }
    }
}