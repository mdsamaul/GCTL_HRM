using GCTL.Core.Data;
using GCTL.Core.ViewModels.HrmAtdMachineData;
using GCTL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using System.Data;
using System.Data.SqlClient;

namespace GCTL.Service.HrmAtdMachineDatas
{
    public class HrmAtdMachineDataService : AppService<HrmAtdMachineData>, IHrmAtdMachineDataService
    {
        private readonly IRepository<HrmAtdMachineData> _hrmAtdMachineDataRepository;
        private readonly string _connectionString;

        public HrmAtdMachineDataService(
            IRepository<HrmAtdMachineData> hrmAtdMachineDataRepository,
           IConfiguration configuration
            ) : base(hrmAtdMachineDataRepository)
        {
            _hrmAtdMachineDataRepository = hrmAtdMachineDataRepository;
            _connectionString = configuration.GetConnectionString("ApplicationDbConnection");
        }

        public async Task<(List<HrmAtdMachineData> Data, int TotalRecords)> GetPaginatedDataAsync(string searchValue, int page, int pageSize, string sortColumn, string sortDirection)
        {
            var query = _hrmAtdMachineDataRepository.All().AsQueryable();

            // Apply global search
            if (!string.IsNullOrEmpty(searchValue))
            {
                query = query.Where(d =>
                    d.FingerPrintId.Contains(searchValue)
                    //|| d.MachineId.Contains(searchValue)
                    || d.Date.ToString().Contains(searchValue)
                    /*|| d.Time.ToString().Contains(searchValue)*/);
            }

            // Get total record count after filtering
            var totalRecords = await query.CountAsync();

            // Apply sorting
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortDirection))
            {
                query = sortColumn switch
                {
                    "fingerPrintId" => sortDirection == "asc" ? query.OrderBy(d => d.FingerPrintId) : query.OrderByDescending(d => d.FingerPrintId),
                    "machineId" => sortDirection == "asc" ? query.OrderBy(d => d.MachineId) : query.OrderByDescending(d => d.MachineId),
                    "date" => sortDirection == "asc" ? query.OrderBy(d => d.Date) : query.OrderByDescending(d => d.Date),
                    "time" => sortDirection == "asc" ? query.OrderBy(d => d.Time) : query.OrderByDescending(d => d.Time),
                    _ => query.OrderBy(d => d.AutoId), // Default sorting
                };
            }

            // Apply pagination
            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalRecords);
        }

        public async Task<List<EmployeeAttendanceGroupViewModel>> GetFilteredAttendanceAsync(
             string employeeIds,
             DateTime? fromDate,
             DateTime? toDate,
             int? fromMonth,
             int? fromYear,
             int? toMonth,
             int? toYear)
        {
            var result = new List<EmployeeAttendanceGroupViewModel>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_GetFilteredAttendance", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@EmployeeIds", string.IsNullOrWhiteSpace(employeeIds) ? (object)DBNull.Value : employeeIds);
                    command.Parameters.AddWithValue("@FromDate", fromDate.HasValue ? (object)fromDate.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@ToDate", toDate.HasValue ? (object)toDate.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@FromMonth", fromMonth.HasValue && fromMonth > 0 ? (object)fromMonth.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@FromYear", fromYear.HasValue && fromYear > 0 ? (object)fromYear.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@ToMonth", toMonth.HasValue && toMonth > 0 ? (object)toMonth.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@ToYear", toYear.HasValue && toYear > 0 ? (object)toYear.Value : DBNull.Value);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var attendanceList = new List<EmployeeAttendanceRawViewModel>();

                        while (await reader.ReadAsync())
                        {
                            attendanceList.Add(new EmployeeAttendanceRawViewModel
                            {
                                EmployeeId = reader["EmployeeId"] as string,
                                EmployeeName = reader["EmployeeName"] as string ?? "",
                                //Date = reader.GetDateTime("Date"),
                                //Time = reader.GetTimeSpan("Time"),
                                //Time = reader.GetTimeSpan(reader.GetOrdinal("Time")),

                                //FingerPrintId = reader["FingerPrintId"] as string,
                                //MachineId = reader["MachineId"] as string,
                                //Latitude = reader["Latitude"] as decimal?,
                                //Longitude = reader["Longitude"] as decimal?,
                                //Remarks = reader["Remarks"] as string
                            });
                        }

                        // Group by Employee
                        result = attendanceList
                            .GroupBy(x => new { x.EmployeeId, x.EmployeeName })
                            .Select(g => new EmployeeAttendanceGroupViewModel
                            {
                                EmployeeId = g.Key.EmployeeId,
                                EmployeeName = g.Key.EmployeeName,
                                EmployeeList = g.Select(x => new EmployeeAttendanceViewModel
                                {
                                    Date = x.Date,
                                    Time = x.Time,
                                    FingerPrintId = x.FingerPrintId,
                                    MachineId = x.MachineId,
                                    Latitude = x.Latitude,
                                    Longitude = x.Longitude,
                                    Remarks = x.Remarks
                                }).ToList()
                            })
                            .ToList();
                    }
                }
            }

            return result;
        }

        public async Task<byte[]> ExportAttendanceToExcelAsync(
            string employeeIds,
            DateTime? fromDate,
            DateTime? toDate,
            int? fromMonth,
            int? fromYear,
            int? toMonth,
            int? toYear)
        {
            var groupedData = await GetFilteredAttendanceAsync(
                employeeIds, fromDate, toDate, fromMonth, fromYear, toMonth, toYear);

            if (groupedData == null || !groupedData.Any())
            {
                throw new InvalidOperationException("No data found for the given criteria.");
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();

            foreach (var emp in groupedData)
            {
                // Create sheet name (max 31 chars, remove invalid chars)
                var sheetName = SanitizeSheetName($"{emp.EmployeeId}");
                var ws = package.Workbook.Worksheets.Add(sheetName);

                // Employee Info Header Section
                ws.Cells[1, 1].Value = "Employee ID:";
                ws.Cells[1, 2].Value = emp.EmployeeId;
                ws.Cells[1, 1].Style.Font.Bold = true;
                ws.Cells[1, 1].Style.Font.Size = 11;

                ws.Cells[2, 1].Value = "Employee Name:";
                ws.Cells[2, 2].Value = emp.EmployeeName;
                ws.Cells[2, 1].Style.Font.Bold = true;
                ws.Cells[2, 1].Style.Font.Size = 11;

                // Add spacing
                ws.Row(3).Height = 5;

                // Column Headers (Row 4)
                //ws.Cells[4, 1].Value = "Date";
                //ws.Cells[4, 2].Value = "Time";
                //ws.Cells[4, 3].Value = "Machine ID";
                //ws.Cells[4, 4].Value = "FingerPrint ID";
                //ws.Cells[4, 5].Value = "Latitude";
                //ws.Cells[4, 6].Value = "Longitude";
                //ws.Cells[4, 7].Value = "Remarks";

                // Style header row
                //using (var range = ws.Cells[4, 1, 4, 7])
                //{
                //    range.Style.Font.Bold = true;
                //    range.Style.Font.Size = 11;
                //    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                //    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189)); // Blue
                //    range.Style.Font.Color.SetColor(Color.White);
                //    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                //    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                //    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                //    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                //    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                //}

                // Data rows
                //int row = 5;
                //foreach (var item in emp.EmployeeList)
                //{
                //    ws.Cells[row, 1].Value = item.Date.ToString("yyyy-MM-dd");
                //    ws.Cells[row, 2].Value = item.Time.ToString("HH:mm:ss");
                //    ws.Cells[row, 3].Value = item.MachineId;
                //    ws.Cells[row, 4].Value = item.FingerPrintId;
                //    ws.Cells[row, 5].Value = item.Latitude;
                //    ws.Cells[row, 6].Value = item.Longitude;
                //    ws.Cells[row, 7].Value = item.Remarks;

                //    // Add borders to data cells
                //    using (var range = ws.Cells[row, 1, row, 7])
                //    {
                //        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                //        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                //        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                //        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                //    }

                //    row++;
                //}

                // Auto-fit columns
                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                // Set minimum column widths
                for (int col = 1; col <= 7; col++)
                {
                    if (ws.Column(col).Width < 12)
                        ws.Column(col).Width = 12;
                }
            }

            return package.GetAsByteArray();
        }

        private string SanitizeSheetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Sheet1";

            // Remove invalid characters for Excel sheet names
            var invalidChars = new[] { '\\', '/', '*', '?', ':', '[', ']' };
            foreach (var c in invalidChars)
            {
                name = name.Replace(c, '_');
            }

            // Limit to 31 characters
            if (name.Length > 31)
                name = name.Substring(0, 31);

            return name;
        }

        //    public async Task<byte[]> ExportAttendanceToExcelAsync(
        //        string employeeIds,
        //        DateTime? fromDate,
        //        DateTime? toDate,
        //        int? fromMonth,
        //        int? fromYear,
        //        int? toMonth,
        //        int? toYear)
        //    {
        //        var groupedData = await GetFilteredAttendanceAsync(
        //            employeeIds, fromDate, toDate, fromMonth, fromYear, toMonth, toYear);

        //        if (groupedData == null || !groupedData.Any())
        //        {
        //            throw new InvalidOperationException("No data found for the given criteria.");
        //        }

        //        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //        using var package = new ExcelPackage();

        //        foreach (var emp in groupedData)
        //        {
        //            var sheetName = SanitizeSheetName($"{emp.EmployeeId}_{emp.EmployeeName}");
        //            var ws = package.Workbook.Worksheets.Add(sheetName);

        //            // Header Info
        //            ws.Cells[1, 1].Value = "Employee ID:";
        //            ws.Cells[1, 2].Value = emp.EmployeeId;
        //            ws.Cells[2, 1].Value = "Employee Name:";
        //            ws.Cells[2, 2].Value = emp.EmployeeName;

        //            ws.Cells[1, 1, 2, 1].Style.Font.Bold = true;
        //            ws.Cells[1, 1, 2, 1].Style.Font.Size = 11;

        //            ws.Row(3).Height = 5;

        //            // Column Headers
        //            ws.Cells[4, 1].Value = "Date";
        //            ws.Cells[4, 2].Value = "Time";
        //            ws.Cells[4, 3].Value = "Machine ID";
        //            ws.Cells[4, 4].Value = "FingerPrint ID";
        //            ws.Cells[4, 5].Value = "Latitude";
        //            ws.Cells[4, 6].Value = "Longitude";
        //            ws.Cells[4, 7].Value = "Remarks";

        //            using (var range = ws.Cells[4, 1, 4, 7])
        //            {
        //                range.Style.Font.Bold = true;
        //                range.Style.Font.Size = 11;
        //                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));
        //                range.Style.Font.Color.SetColor(Color.White);
        //                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        //                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //            }

        //            int row = 5;
        //            foreach (var item in emp.EmployeeList)
        //            {
        //                ws.Cells[row, 1].Value = item.Date.ToString("yyyy-MM-dd");
        //                ws.Cells[row, 2].Value = item.Time.ToString(@"hh\:mm\:ss");
        //                ws.Cells[row, 3].Value = item.MachineId;
        //                ws.Cells[row, 4].Value = item.FingerPrintId;
        //                ws.Cells[row, 5].Value = item.Latitude;
        //                ws.Cells[row, 6].Value = item.Longitude;
        //                ws.Cells[row, 7].Value = item.Remarks;

        //                using (var range = ws.Cells[row, 1, row, 7])
        //                {
        //                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        //                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //                }
        //                row++;
        //            }

        //            ws.Cells[ws.Dimension.Address].AutoFitColumns();
        //            for (int col = 1; col <= 7; col++)
        //            {
        //                if (ws.Column(col).Width < 12) ws.Column(col).Width = 12;
        //            }
        //        }

        //        return package.GetAsByteArray();
        //    }

        //    private string SanitizeSheetName(string name)
        //    {
        //        if (string.IsNullOrWhiteSpace(name)) return "Sheet1";

        //        var invalid = new[] { '\\', '/', '*', '?', ':', '[', ']' };
        //        foreach (var c in invalid)
        //            name = name.Replace(c, '_');

        //        return name.Length > 31 ? name.Substring(0, 31) : name;
        //    }
        //}



    }
}

