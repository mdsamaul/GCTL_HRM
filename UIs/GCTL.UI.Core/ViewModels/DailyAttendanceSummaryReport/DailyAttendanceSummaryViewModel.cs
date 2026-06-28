using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.DailyAttendanceSummaryReport;
using GCTL.Core.ViewModels.RosterScheduleEntry;

namespace GCTL.UI.Core.ViewModels.DailyAttendanceSummaryReport
{
    public class DailyAttendanceSummaryViewModel:BaseViewModel
    {
        public DailyAttendanceSummaryFilterDto Setup { get; set; } = new DailyAttendanceSummaryFilterDto();
    }
}
