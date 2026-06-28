using ClosedXML.Excel;
using Dapper;
using GCTL.Core.Data;
using GCTL.Core.ViewModels.DailyAttendanceDetailsReport;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace GCTL.Service.DailyAttendanceDetailsReport
{
    public interface IDailyAttendanceDetailsReportService
    {
        Task<bool> PagePermissionAsync(string accessCode);
        Task<DailyAttendanceDetailsResultDto> GetReportDataAsync(DailyAttendanceDetailsFilterDto filter);
        Task<byte[]> ExportExcelAsync(DailyAttendanceDetailsFilterDto filter, string? logoPhysicalPath = null);
    }
}