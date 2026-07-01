using GCTL.Core.Data;
using GCTL.Core.ViewModels.DailyAttendanceSummaryReport;
using GCTL.Data.Models;
using Microsoft.EntityFrameworkCore;
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

namespace GCTL.Service.DailyAttendanceSummaryReportService
{
    public class DailyAttendanceSummaryReportService : IDailyAttendanceSummaryReportService
    {
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly string _connectionString;
        public DailyAttendanceSummaryReportService(IRepository<CoreAccessCode> accessCodeRepository, IConfiguration configuration)
        {
            this.accessCodeRepository = accessCodeRepository;
            _connectionString = configuration.GetConnectionString("ApplicationDbConnection")!;
        }

        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Daily Attendance Summary" && x.TitleCheck);
        }


        public async Task<DailyAttendanceSummaryResponseDto> GetSummaryAsync(DailyAttendanceSummaryFilterDto filter)
        {
            var result = new DailyAttendanceSummaryResponseDto();

            var deptCodes = (filter.DepartmentCodes != null && filter.DepartmentCodes.Count > 0)
                ? string.Join(",", filter.DepartmentCodes.Select(d => d.Trim()).Where(d => d != ""))
                : null;

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("RPT_GetDailyAttendanceSummaryReport", con)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 120
            };

            cmd.Parameters.AddWithValue("@CompanyCode",
                string.IsNullOrWhiteSpace(filter.CompanyCode) ? DBNull.Value : filter.CompanyCode);

            cmd.Parameters.AddWithValue("@DepartmentCodes",
                string.IsNullOrWhiteSpace(deptCodes) ? DBNull.Value : deptCodes);

            cmd.Parameters.AddWithValue("@FromDate",
                filter.FromDate.HasValue ? filter.FromDate.Value.Date : DBNull.Value);

            cmd.Parameters.AddWithValue("@LoginEmployeeId",                              // NEW
                string.IsNullOrWhiteSpace(filter.LoginEmployeeId) ? DBNull.Value : filter.LoginEmployeeId);

            cmd.Parameters.AddWithValue("@AccessCodeId",                                 // NEW
                string.IsNullOrWhiteSpace(filter.AccessCodeId) ? DBNull.Value : filter.AccessCodeId);

            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            string? dataDate = null;

            while (await reader.ReadAsync())
            {
                var dept = new DailyAttendanceSummaryDto
                {
                    DepartmentName = reader["DepartmentName"]?.ToString() ?? "",
                    NoOfEmps = Convert.ToInt32(reader["NoOfEmps"]),
                    PresentCount = Convert.ToInt32(reader["PresentCount"]),
                    LateCount = Convert.ToInt32(reader["LateCount"]),
                    LeaveCount = Convert.ToInt32(reader["LeaveCount"]),
                    AbsentCount = Convert.ToInt32(reader["AbsentCount"]),
                };

                if (dataDate == null && reader["DataDate"] != DBNull.Value)
                {
                    var date = Convert.ToDateTime(reader["DataDate"]);
                    dataDate = date.ToString("dd/MM/yyyy");
                }

                result.Departments.Add(dept);
            }

            result.TotalNoOfEmps = result.Departments.Sum(x => x.NoOfEmps);
            result.TotalPresent = result.Departments.Sum(x => x.PresentCount);
            result.TotalLate = result.Departments.Sum(x => x.LateCount);
            result.TotalLeave = result.Departments.Sum(x => x.LeaveCount);
            result.TotalAbsent = result.Departments.Sum(x => x.AbsentCount);
            result.DataDate = dataDate;

            return result;
        }

        public byte[] GenerateExcel(DailyAttendanceSummaryResponseDto data)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Attendance Summary");

            var pageWidth = 7; // A থেকে G (6 column + 1 extra for spacing)

            // ─── Row 1: Company Name ───────────────────────────────
            ws.Cells[1, 1, 1, 6].Merge = true;
            ws.Cells[1, 1].Value = data.CompanyName ?? "DataPath Ltd.";
            ws.Cells[1, 1].Style.Font.Size = 16;
            ws.Cells[1, 1].Style.Font.Bold = true;
            ws.Cells[1, 1].Style.Font.Name = "Times New Roman";
            ws.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // ─── Row 2: Report Title ───────────────────────────────
            ws.Cells[2, 1, 2, 6].Merge = true;
            ws.Cells[2, 1].Value = "Daily Attendance Summary Report";
            ws.Cells[2, 1].Style.Font.Size = 13;
            ws.Cells[2, 1].Style.Font.Name = "Times New Roman";
            ws.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // ─── Row 2 underline (border bottom) ──────────────────
            ws.Cells[2, 1, 2, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            // ─── Row 3: Date ──────────────────────────────────────
            ws.Cells[3, 1, 3, 6].Merge = true;
            ws.Cells[3, 1].Value = "Date: " + (data.DataDate ?? "");
            ws.Cells[3, 1].Style.Font.Size = 10;
            ws.Cells[3, 1].Style.Font.Name = "Times New Roman";
            ws.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // ─── Row 4: Empty gap ─────────────────────────────────
            int headerRow = 5;

            // ─── Row 5: Table Header ──────────────────────────────
            string[] headers = { "Department", "No.of Emps", "Present", "Late", "Leave", "Absent" };
            for (int col = 1; col <= 6; col++)
            {
                var cell = ws.Cells[headerRow, col];
                cell.Value = headers[col - 1];
                cell.Style.Font.Bold = true;
                cell.Style.Font.Name = "Times New Roman";
                cell.Style.Font.Size = 10;
                cell.Style.HorizontalAlignment = col == 1
                    ? ExcelHorizontalAlignment.Left
                    : ExcelHorizontalAlignment.Center;
                cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
            }

            // ─── Data Rows ────────────────────────────────────────
            int dataStartRow = headerRow + 1;
            int rowIndex = dataStartRow;

            foreach (var dept in data.Departments)
            {
                SetDataRow(ws, rowIndex, dept.DepartmentName, dept.NoOfEmps,
                    dept.PresentCount, dept.LateCount, dept.LeaveCount, dept.AbsentCount, false);
                rowIndex++;
            }

            // ─── Total Row ────────────────────────────────────────
            SetDataRow(ws, rowIndex, "", data.TotalNoOfEmps,
                data.TotalPresent, data.TotalLate, data.TotalLeave, data.TotalAbsent, true);

            // ─── Footer Row ───────────────────────────────────────
            int footerRow = rowIndex + 2;
            var printDT = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");

            ws.Cells[footerRow, 1, footerRow, 2].Merge = true;
            ws.Cells[footerRow, 1].Value = "Print Datetime:  " + printDT;
            ws.Cells[footerRow, 1].Style.Font.Size = 8;
            ws.Cells[footerRow, 1].Style.Font.Name = "Times New Roman";
            ws.Cells[footerRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            ws.Cells[footerRow, 3, footerRow, 4].Merge = true;
            ws.Cells[footerRow, 3].Value = "GCTL- Human Resource Management";
            ws.Cells[footerRow, 3].Style.Font.Size = 8;
            ws.Cells[footerRow, 3].Style.Font.Name = "Times New Roman";
            ws.Cells[footerRow, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells[footerRow, 5, footerRow, 6].Merge = true;
            ws.Cells[footerRow, 5].Value = "Page 1 of 1";
            ws.Cells[footerRow, 5].Style.Font.Size = 8;
            ws.Cells[footerRow, 5].Style.Font.Name = "Times New Roman";
            ws.Cells[footerRow, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            // ─── Column Width ─────────────────────────────────────
            ws.Column(1).Width = 35;  // Department
            ws.Column(2).Width = 14;  // No.of Emps
            ws.Column(3).Width = 12;  // Present
            ws.Column(4).Width = 10;  // Late
            ws.Column(5).Width = 10;  // Leave
            ws.Column(6).Width = 10;  // Absent

            // ─── Row Heights ──────────────────────────────────────
            ws.Row(1).Height = 22;
            ws.Row(2).Height = 18;
            ws.Row(3).Height = 15;
            ws.Row(headerRow).Height = 18;

            // ─── Print Setup (A4 Portrait) ────────────────────────
            ws.PrinterSettings.PaperSize = ePaperSize.A4;
            ws.PrinterSettings.Orientation = eOrientation.Portrait;
            ws.PrinterSettings.FitToPage = true;
            ws.PrinterSettings.FitToWidth = 1;
            ws.PrinterSettings.FitToHeight = 0;

            return package.GetAsByteArray();
        }

        // ─── Helper ───────────────────────────────────────────────────
        private static void SetDataRow(ExcelWorksheet ws, int row,
            string dept, int total, int present, int late, int leave, int absent, bool isTotalRow)
        {
            object[] values = { dept, total, present, late, leave, absent };

            for (int col = 1; col <= 6; col++)
            {
                var cell = ws.Cells[row, col];
                cell.Value = values[col - 1];
                cell.Style.Font.Name = "Times New Roman";
                cell.Style.Font.Size = 9;
                cell.Style.Font.Bold = isTotalRow;
                cell.Style.HorizontalAlignment = col == 1
                    ? ExcelHorizontalAlignment.Left
                    : ExcelHorizontalAlignment.Center;
                cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }
        }
    }
}

