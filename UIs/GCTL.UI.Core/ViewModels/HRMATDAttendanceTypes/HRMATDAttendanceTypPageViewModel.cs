using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HRMATDAttendanceTypes;

namespace GCTL.UI.Core.ViewModels.HRMATDAttendanceTypes
{
    public class HRMATDAttendanceTypPageViewModel : BaseViewModel
    {
        public HRMATDAttendanceTypeSetupViewModel Setup { get; set; } = new HRMATDAttendanceTypeSetupViewModel();
        public List<HRMATDAttendanceTypeSetupViewModel> TableDataList { get; set; } = new List<HRMATDAttendanceTypeSetupViewModel>();
    }
}
