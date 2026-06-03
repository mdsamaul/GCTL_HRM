using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.AdvanceLoanAdjustmentReport;
using GCTL.Core.ViewModels.AttendanceMovementRegisterReportDto;

namespace GCTL.UI.Core.ViewModels.AttendanceMovementRegisterReportViewModel
{
    public class AttendanceMovementRegisterReportViewModel :BaseViewModel
    {
        public AttendanceMovementRegisterReportDto SetupViewModel { get; set; } = new AttendanceMovementRegisterReportDto();
    }
}
