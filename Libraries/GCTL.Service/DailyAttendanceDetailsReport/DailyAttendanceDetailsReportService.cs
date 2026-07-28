using ClosedXML.Excel;
using Dapper;
using GCTL.Core.Data;
using GCTL.Core.ViewModels.DailyAttendanceDetailsReport;
using GCTL.Data.Models;
using Humanizer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using System.Data;


namespace GCTL.Service.DailyAttendanceDetailsReport
{
    public class DailyAttendanceDetailsReportService : IDailyAttendanceDetailsReportService
    {
        private readonly IRepository<CoreAccessCode> _accessCodeRepo;
        private readonly string _connectionString;

        public DailyAttendanceDetailsReportService(
            IRepository<CoreAccessCode> accessCodeRepo,
            IConfiguration configuration)
        {
            _accessCodeRepo = accessCodeRepo;
            _connectionString = configuration.GetConnectionString("ApplicationDbConnection")!;
        }

        // ── Permission ───────────────────────────────────────────────
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await _accessCodeRepo.All()
                .AnyAsync(x => x.AccessCodeId == accessCode
                            && x.Title == "Daily Attendance Details"
                            && x.TitleCheck);
        }

        // ── Main Data ────────────────────────────────────────────────
        public async Task<DailyAttendanceDetailsResultDto> GetReportDataAsync(DailyAttendanceDetailsFilterDto filter)
        {
            var result = new DailyAttendanceDetailsResultDto();

            var branchCsv = filter.BranchCodes != null ? string.Join(",", filter.BranchCodes) : null;
            var deptCsv = filter.DepartmentCodes != null ? string.Join(",", filter.DepartmentCodes) : null;
            var empCsv = filter.EmployeeIds != null ? string.Join(",", filter.EmployeeIds) : null;

            DateTime? fromDate = null;
            if (!string.IsNullOrWhiteSpace(filter.FromDate) &&
                DateTime.TryParse(filter.FromDate, out var parsedDate))
                fromDate = parsedDate.Date;

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var param = new DynamicParameters();
            param.Add("@CompanyCode", filter.CompanyCode, DbType.String);
            param.Add("@BranchCodes", branchCsv, DbType.String);
            param.Add("@DepartmentCodes", deptCsv, DbType.String);
            param.Add("@EmployeeIds", empCsv, DbType.String);
            param.Add("@FromDate", fromDate, DbType.Date);
            param.Add("@ReportType", filter.ReportType, DbType.String);
            param.Add("@LoginEmployeeId", filter.LoginEmployeeId, DbType.String);
            param.Add("@AccessCodeId", filter.AccessCodeId, DbType.String);

            using var multi = await conn.QueryMultipleAsync(
                "RPT_GetDailyAttendanceDetailsReport",
                param,
                commandType: CommandType.StoredProcedure);

            switch (filter.ReportType)
            {
                case "Present":
                    result.PresentRows = (await multi.ReadAsync<DailyAttendancePresentRowDto>()).ToList();
                    break;
                case "Absent":
                    result.AbsentRows = (await multi.ReadAsync<DailyAttendanceAbsentRowDto>()).ToList();
                    break;
                case "Late":
                    result.LateRows = (await multi.ReadAsync<DailyAttendanceLateRowDto>()).ToList();
                    break;
                case "InOut":
                    result.InOutRows = (await multi.ReadAsync<DailyAttendanceInOutRowDto>()).ToList();
                    break;
                case "MissingCheckOut":
                    result.MissingCheckOutRows = (await multi.ReadAsync<DailyAttendanceInOutRowDto>()).ToList();
                    break;
                case "EarlyLeave":
                    result.EarlyLeaveRows = (await multi.ReadAsync<DailyAttendanceInOutRowDto>()).ToList();
                    break;
            }

            var header = await multi.ReadFirstOrDefaultAsync<DailyAttendanceDetailsHeaderDto>();
            if (header != null) result.Header = header;

            return result;
        }

        // ── Excel Export ─────────────────────────────────────────────
        public async Task<byte[]> ExportExcelAsync(DailyAttendanceDetailsFilterDto filter, string? logoPhysicalPath = null)
        {
            var data = await GetReportDataAsync(filter);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var reportName = filter.ReportType switch
            {
                "Present" => "Daily Present Report",
                "Absent" => "Daily Absent Report",
                "Late" => "Daily Late Report",
                "InOut" => "Daily In-Out Report",
                "MissingCheckOut" => "Daily Missing Check-Out Report",
                "EarlyLeave" => "Daily Early Office Leave Report",
                _ => "Daily Attendance Details"
            };

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add(reportName);

            int totalCols = GetTotalColumns(filter.ReportType);

            ws.Cells[1, 1, 1, totalCols].Merge = true;
            ws.Cells[1, 1].Value = data.Header?.CompanyName ?? string.Empty;
            ws.Cells[1, 1].Style.Font.Bold = true;
            ws.Cells[1, 1].Style.Font.Size = 14;
            ws.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells[2, 1, 2, totalCols].Merge = true;
            ws.Cells[2, 1].Value = reportName;
            ws.Cells[2, 1].Style.Font.Bold = true;
            ws.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells[3, 1, 3, totalCols].Merge = true;
            if (DateTime.TryParse(data.Header?.DataDate, out DateTime dt))
            {
                ws.Cells[3, 1].Value = $"Date: {dt:dd/MM/yyyy}";
            }
            else
            {
                ws.Cells[3, 1].Value = $"Date: {data.Header?.DataDate}";
            }
            ws.Cells[3, 1].Style.Font.Bold = true;
            ws.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            AddLogo(ws, logoPhysicalPath);

            int startRow = 5;
            string currDept = string.Empty;
            int sn = 1;

            switch (filter.ReportType)
            {
                case "Present":
                    BuildPresentSheet(ws, data.PresentRows ?? new(), startRow, ref currDept, ref sn);
                    break;
                case "Absent":
                    BuildAbsentSheet(ws, data.AbsentRows ?? new(), startRow, ref currDept, ref sn);
                    break;
                case "Late":
                    BuildLateSheet(ws, data.LateRows ?? new(), startRow, ref currDept, ref sn);
                    break;
                case "InOut":
                    BuildInOutSheet(ws, data.InOutRows ?? new(), startRow, ref currDept, ref sn);
                    break;
                case "MissingCheckOut":
                    BuildMissingCheckOutSheet(ws, data.MissingCheckOutRows ?? new(), startRow, ref currDept, ref sn);
                    break;
                case "EarlyLeave":
                    BuildEarlyLeaveSheet(ws, data.EarlyLeaveRows ?? new(), startRow, ref currDept, ref sn);
                    break;
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            return await package.GetAsByteArrayAsync();
        }

        // ── Logo helper ──────────────────────────────────────────────
        private static void AddLogo(ExcelWorksheet ws, string? logoPhysicalPath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(logoPhysicalPath) || !File.Exists(logoPhysicalPath))
                    return;

                var fileInfo = new FileInfo(logoPhysicalPath);
                var pic = ws.Drawings.AddPicture("CompanyLogo", fileInfo);
                pic.SetPosition(0, 2, 0, 2);
                pic.SetSize(120, 60);
            }
            catch
            {
                // Logo is decorative — never fail the export because of it.
            }
        }

        // ── Column count by report type (must match PDF columns) ─────
        private static int GetTotalColumns(string reportType) => reportType switch
        {
            "Present" => 9,
            "Absent" => 5,
            "Late" => 9,
            "InOut" => 13,
            "MissingCheckOut" => 10,
            "EarlyLeave" => 12,
            _ => 9
        };

        // ── Shared: department header + column headers ──
        private static void AddDeptHeader(ExcelWorksheet ws, string deptName, ref int row, string[] cols)
        {
            ws.Cells[row, 1, row, cols.Length].Merge = true;
            ws.Cells[row, 1].Value = "Department: " + deptName;
            ws.Cells[row, 1].Style.Font.Bold = true;
            ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Cells[row, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            row++;

            for (int i = 0; i < cols.Length; i++)
            {
                var cell = ws.Cells[row, i + 1];
                cell.Value = cols[i];
                cell.Style.Font.Bold = true;
                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                cell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            }
            row++;
        }

        // ── Shared: writes one body cell ──
        private static void SetCell(ExcelWorksheet ws, int row, int col, object? value, bool leftAlign = false)
        {
            var cell = ws.Cells[row, col];
            cell.Value = value;
            cell.Style.HorizontalAlignment = leftAlign ? ExcelHorizontalAlignment.Left : ExcelHorizontalAlignment.Center;
            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        }

        // ── Present ──
        private static void BuildPresentSheet(ExcelWorksheet ws,
            List<DailyAttendancePresentRowDto> rows, int startRow,
            ref string currentDept, ref int sn)
        {
            string[] cols = { "SN", "Emp. ID", "Name", "Designation", "Shift", "In Time", "Late", "Status", "Remarks" };
            int row = startRow;
            foreach (var r in rows)
            {
                if (r.DepartmentName != currentDept)
                {
                    currentDept = r.DepartmentName;
                    sn = 1;
                    AddDeptHeader(ws, currentDept, ref row, cols);
                }
                SetCell(ws, row, 1, sn++);
                SetCell(ws, row, 2, r.EmployeeId);
                SetCell(ws, row, 3, r.EmployeeName, leftAlign: true);
                SetCell(ws, row, 4, r.Designation, leftAlign: true);
                SetCell(ws, row, 5, r.ShiftName);
                SetCell(ws, row, 6, r.InTime);
                SetCell(ws, row, 7, r.LateDisplay);
                SetCell(ws, row, 8, r.Status);
                SetCell(ws, row, 9, r.Remarks);
                row++;
            }
        }

        // ── Absent ──
        private static void BuildAbsentSheet(ExcelWorksheet ws,
            List<DailyAttendanceAbsentRowDto> rows, int startRow,
            ref string currentDept, ref int sn)
        {
            string[] cols = { "SN", "Emp. ID", "Name", "Designation", "Status" };
            int row = startRow;
            foreach (var r in rows)
            {
                if (r.DepartmentName != currentDept)
                {
                    currentDept = r.DepartmentName;
                    sn = 1;
                    AddDeptHeader(ws, currentDept, ref row, cols);
                }
                SetCell(ws, row, 1, sn++);
                SetCell(ws, row, 2, r.EmployeeId);
                SetCell(ws, row, 3, r.EmployeeName, leftAlign: true);
                SetCell(ws, row, 4, r.Designation, leftAlign: true);
                SetCell(ws, row, 5, r.Status);
                row++;
            }
        }

        // ── Late ──
        private static void BuildLateSheet(ExcelWorksheet ws,
            List<DailyAttendanceLateRowDto> rows, int startRow,
            ref string currentDept, ref int sn)
        {
            string[] cols = { "SN", "Emp. ID", "Name", "Designation", "Shift", "In Time", "Late", "Status", "Remarks" };
            int row = startRow;
            foreach (var r in rows)
            {
                if (r.DepartmentName != currentDept)
                {
                    currentDept = r.DepartmentName;
                    sn = 1;
                    AddDeptHeader(ws, currentDept, ref row, cols);
                }
                SetCell(ws, row, 1, sn++);
                SetCell(ws, row, 2, r.EmployeeId);
                SetCell(ws, row, 3, r.EmployeeName, leftAlign: true);
                SetCell(ws, row, 4, r.Designation, leftAlign: true);
                SetCell(ws, row, 5, r.ShiftName);
                SetCell(ws, row, 6, r.InTime);
                SetCell(ws, row, 7, r.LateDisplay);
                SetCell(ws, row, 8, r.Status);
                SetCell(ws, row, 9, r.Remarks);
                row++;
            }
        }

        // ── In-Out (has OT(H)) ──
        private static void BuildInOutSheet(ExcelWorksheet ws,
            List<DailyAttendanceInOutRowDto> rows, int startRow,
            ref string currentDept, ref int sn)
        {
            string[] cols = { "SN", "Emp. ID", "Name", "Designation", "Shift", "In Time", "Late", "Out Time", "Early Out", "W.Hour(s)", "OT(H)", "Status", "Remarks" };
            int row = startRow;
            foreach (var r in rows)
            {
                if (r.DepartmentName != currentDept)
                {
                    currentDept = r.DepartmentName;
                    sn = 1;
                    AddDeptHeader(ws, currentDept, ref row, cols);
                }
                SetCell(ws, row, 1, sn++);
                SetCell(ws, row, 2, r.EmployeeId);
                SetCell(ws, row, 3, r.EmployeeName, leftAlign: true);
                SetCell(ws, row, 4, r.Designation, leftAlign: true);
                SetCell(ws, row, 5, r.ShiftName);
                SetCell(ws, row, 6, r.InTime);
                SetCell(ws, row, 7, r.LateDisplay);
                SetCell(ws, row, 8, r.OutTime);
                SetCell(ws, row, 9, r.EarlyOut);
                SetCell(ws, row, 10, r.WorkHours);
                SetCell(ws, row, 11, r.OTHours);
                SetCell(ws, row, 12, r.Status);
                SetCell(ws, row, 13, r.Remarks);
                row++;
            }
        }

        // ── Missing Check-Out — SN, EmpID, Name, Designation, Shift, InTime, Late, OutTime, Status, Remarks ──
        private static void BuildMissingCheckOutSheet(ExcelWorksheet ws,
            List<DailyAttendanceInOutRowDto> rows, int startRow,
            ref string currentDept, ref int sn)
        {
            string[] cols = { "SN", "Emp. ID", "Name", "Designation", "Shift", "In Time", "Late", "Out Time", "Status", "Remarks" };
            int row = startRow;
            foreach (var r in rows)
            {
                if (r.DepartmentName != currentDept)
                {
                    currentDept = r.DepartmentName;
                    sn = 1;
                    AddDeptHeader(ws, currentDept, ref row, cols);
                }
                SetCell(ws, row, 1, sn++);
                SetCell(ws, row, 2, r.EmployeeId);
                SetCell(ws, row, 3, r.EmployeeName, leftAlign: true);
                SetCell(ws, row, 4, r.Designation, leftAlign: true);
                SetCell(ws, row, 5, r.ShiftName);
                SetCell(ws, row, 6, r.InTime);
                SetCell(ws, row, 7, r.LateDisplay);
                SetCell(ws, row, 8, r.OutTime);
                SetCell(ws, row, 9, r.Status);
                SetCell(ws, row, 10, r.Remarks);
                row++;
            }
        }

        // ── Early Leave — SN, EmpID, Name, Designation, Shift, InTime, Late, OutTime, EarlyOut, W.Hour(s), Status, Remarks ──
        private static void BuildEarlyLeaveSheet(ExcelWorksheet ws,
            List<DailyAttendanceInOutRowDto> rows, int startRow,
            ref string currentDept, ref int sn)
        {
            string[] cols = { "SN", "Emp. ID", "Name", "Designation", "Shift", "In Time", "Late", "Out Time", "Early Out", "W.Hour(s)", "Status", "Remarks" };
            int row = startRow;
            foreach (var r in rows)
            {
                if (r.DepartmentName != currentDept)
                {
                    currentDept = r.DepartmentName;
                    sn = 1;
                    AddDeptHeader(ws, currentDept, ref row, cols);
                }
                SetCell(ws, row, 1, sn++);
                SetCell(ws, row, 2, r.EmployeeId);
                SetCell(ws, row, 3, r.EmployeeName, leftAlign: true);
                SetCell(ws, row, 4, r.Designation, leftAlign: true);
                SetCell(ws, row, 5, r.ShiftName);
                SetCell(ws, row, 6, r.InTime);
                SetCell(ws, row, 7, r.LateDisplay);
                SetCell(ws, row, 8, r.OutTime);
                SetCell(ws, row, 9, r.EarlyOut);
                SetCell(ws, row, 10, r.WorkHours);
                SetCell(ws, row, 11, r.Status);
                SetCell(ws, row, 12, r.Remarks);
                row++;
            }
        }
    }
}