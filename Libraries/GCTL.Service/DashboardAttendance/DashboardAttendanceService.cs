using Dapper;
using GCTL.Core.ViewModels.Dashboard;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace GCTL.Service.DashboardAttendance
{
    public class DashboardAttendanceService : IDashboardAttendanceService
    {
        private readonly string _conn;

        public DashboardAttendanceService(IConfiguration config)
        {
            _conn = config.GetConnectionString("ApplicationDbConnection");
        }

        public async Task<(DashboardAttendanceSummaryDto Summary,
                           IEnumerable<DashboardAttendanceMovementDto> Items,
                           int TotalCount)>
            GetAttendanceMovementAsync(
                string companyCode, string branchCode, string departmentCode,
                DateTime forDate, int page, int pageSize, string search = null)
        {
            var list = new List<DashboardAttendanceMovementDto>();
            var summary = new DashboardAttendanceSummaryDto();
            int total = 0;

            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand("usp_GetAttendanceMovement", conn)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 120
            };

            cmd.Parameters.AddWithValue("@CompanyCode", (object?)companyCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BranchCode", (object?)branchCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DepartmentCode", (object?)departmentCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FromDate", forDate.Date);
            cmd.Parameters.AddWithValue("@Page", page);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);
            cmd.Parameters.AddWithValue("@Search", (object?)search ?? DBNull.Value);

            await conn.OpenAsync();
            using var rdr = await cmd.ExecuteReaderAsync();

            // ── Result Set 1: Summary ─────────────────────────────
            if (rdr.HasRows && await rdr.ReadAsync())
            {
                summary = new DashboardAttendanceSummaryDto
                {
                    TotalEmployees = SafeInt(rdr, "TotalEmployees"),
                    PresentCount = SafeInt(rdr, "PresentCount"),
                    AbsentCount = SafeInt(rdr, "AbsentCount"),
                    LateCount = SafeInt(rdr, "LateCount"),
                    OnLeaveCount = SafeInt(rdr, "OnLeaveCount"),
                    PresentPct = SafeDec(rdr, "PresentPct"),
                    AbsentPct = SafeDec(rdr, "AbsentPct"),
                    LatePct = SafeDec(rdr, "LatePct"),
                    OnLeavePct = SafeDec(rdr, "OnLeavePct"),
                    DataDate = SafeDate(rdr, "DataDate")
                };
            }

            // ── Result Set 2: Employee rows ───────────────────────
            if (await rdr.NextResultAsync())
            {
                var schema = rdr.GetColumnSchema();
                var colNames = schema.Select(c => c.ColumnName).ToHashSet(StringComparer.OrdinalIgnoreCase);

                while (await rdr.ReadAsync())
                {
                    var dto = new DashboardAttendanceMovementDto
                    {
                        RowNum = SafeInt(rdr, "RowNum"),
                        EmployeeId = SafeStr(rdr, "EmployeeId"),
                        Name = SafeStr(rdr, "Name"),
                        Designation = SafeStr(rdr, "Designation"),
                        CheckIn = SafeStr(rdr, "CheckIn"),
                        CheckOut = SafeStr(rdr, "CheckOut"),
                        Movement = SafeStr(rdr, "Movement"),
                        // ── নতুন columns ──────────────────────────
                        Remarks = colNames.Contains("Remarks") ? SafeStr(rdr, "Remarks") : null,
                        LateByMinutes = colNames.Contains("LateByMinutes") ? SafeInt(rdr, "LateByMinutes") : 0,
                        // ──────────────────────────────────────────
                        ImgType = SafeStr(rdr, "ImgType"),
                        Status = colNames.Contains("Status") ? SafeStr(rdr, "Status") : null,
                        StatusOrder = colNames.Contains("StatusOrder") ? SafeInt(rdr, "StatusOrder") : 4,
                        Photo = colNames.Contains("Photo") && rdr["Photo"] != DBNull.Value
                                        ? (byte[])rdr["Photo"] : null,
                        DataDate = SafeDate(rdr, "DataDate")
                    };

                    if (total == 0 && colNames.Contains("TotalCount"))
                        total = SafeInt(rdr, "TotalCount");

                    list.Add(dto);
                }
            }

            return (summary, list, total);
        }

        // ── Safe helpers ──────────────────────────────────────────
        private static int SafeInt(SqlDataReader r, string col)
        {
            try { return r[col] != DBNull.Value ? Convert.ToInt32(r[col]) : 0; }
            catch { return 0; }
        }

        private static decimal SafeDec(SqlDataReader r, string col)
        {
            try { return r[col] != DBNull.Value ? Convert.ToDecimal(r[col]) : 0m; }
            catch { return 0m; }
        }

        private static string SafeStr(SqlDataReader r, string col)
        {
            try { return r[col] as string; }
            catch { return null; }
        }

        private static DateTime SafeDate(SqlDataReader r, string col)
        {
            try { return r[col] != DBNull.Value ? Convert.ToDateTime(r[col]) : DateTime.Today; }
            catch { return DateTime.Today; }
        }


        public async Task<LeaveDashboardResponseDto> GetLeaveDashboardAsync(
            string companyCode,
            string branchCode,
            string departmentCode,
            int year,
            int page,
            int pageSize,
            string search,
            string employeeId = null)   // ← নতুন parameter
        {
            using var con = new SqlConnection(_conn);
            await con.OpenAsync();

            var param = new DynamicParameters();
            param.Add("@CompanyCode", string.IsNullOrEmpty(companyCode) ? null : companyCode, DbType.String);
            param.Add("@BranchCode", string.IsNullOrEmpty(branchCode) ? null : branchCode, DbType.String);
            param.Add("@DepartmentCode", string.IsNullOrEmpty(departmentCode) ? null : departmentCode, DbType.String);
            param.Add("@Year", year, DbType.Int32);
            param.Add("@Page", page, DbType.Int32);
            param.Add("@PageSize", pageSize, DbType.Int32);
            param.Add("@Search", string.IsNullOrEmpty(search) ? null : search, DbType.String);
            param.Add("@EmployeeId", string.IsNullOrEmpty(employeeId) ? null : employeeId, DbType.String);  // ← নতুন

            using var multi = await con.QueryMultipleAsync(
                "usp_GetLeaveDashboard",
                param,
                commandType: CommandType.StoredProcedure
            );

            // RS1 — Summary
            var summary = await multi.ReadFirstOrDefaultAsync<LeaveSummaryCardDto>()
                          ?? new LeaveSummaryCardDto();

            // RS2 — Leave types
            var leaveTypes = (await multi.ReadAsync<LeaveTypeDto>()).ToList();

            // RS3 — Paged flat rows
            var employees = (await multi.ReadAsync<EmployeeLeaveRowDto>()).ToList();

            int totalCount = employees.FirstOrDefault()?.TotalCount ?? 0;

            return new LeaveDashboardResponseDto
            {
                Summary = summary,
                LeaveTypes = leaveTypes,
                Employees = employees,
                TotalCount = totalCount
            };
        }
    }
}