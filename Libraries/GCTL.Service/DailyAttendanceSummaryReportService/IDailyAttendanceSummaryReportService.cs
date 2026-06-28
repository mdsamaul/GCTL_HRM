using GCTL.Core.ViewModels.DailyAttendanceSummaryReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.DailyAttendanceSummaryReportService
{
    public interface IDailyAttendanceSummaryReportService
    {
        Task<bool> PagePermissionAsync(string accessCode);
        Task<DailyAttendanceSummaryResponseDto> GetSummaryAsync(DailyAttendanceSummaryFilterDto filter);
        byte[] GenerateExcel(DailyAttendanceSummaryResponseDto data);
    }
}
