using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HrmEmployees2;
using GCTL.Core.ViewModels.ManualAttendance;

namespace GCTL.UI.Core.ViewModels.ManualAttendance
{
    public class ManualAttendancePageViewModel : BaseViewModel
    {
        public ManualAttendanceSetupViewModel Setup = new ManualAttendanceSetupViewModel();

        public List<ManualAttendanceSetupViewModel> ListTableData = new List<ManualAttendanceSetupViewModel>();
    }
}
