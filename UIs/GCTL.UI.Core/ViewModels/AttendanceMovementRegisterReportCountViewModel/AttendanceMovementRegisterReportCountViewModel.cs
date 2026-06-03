using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.AttendanceMovementRegisterReportCount;
using GCTL.Core.ViewModels.AttendanceMovementRegisterReportDto;

namespace GCTL.UI.Core.ViewModels.AttendanceMovementRegisterReportContViewModel
{
    public class AttendanceMovementRegisterReportCountViewModel : BaseViewModel
    {
        public AttendanceMovementRegisterReportCountDto SetupViewModel { get; set; } = new AttendanceMovementRegisterReportCountDto();
    }
}
