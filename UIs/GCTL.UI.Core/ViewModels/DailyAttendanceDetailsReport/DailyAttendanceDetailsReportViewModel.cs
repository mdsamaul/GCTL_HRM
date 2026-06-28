using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.DailyAttendanceDetailsReport;

namespace GCTL.UI.Core.ViewModels.DailyAttendanceDetailsReport
{
    public class DailyAttendanceDetailsReportViewModel : BaseViewModel
    {
        public DailyAttendanceDetailsFilterDto Setup { get; set; } = new DailyAttendanceDetailsFilterDto();
    }
}