using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.ManualAttendanceBulk;

namespace GCTL.UI.Core.ViewModels.ManualAttendanceBulk
{
    public class ManualAttendanceBulkPageViewModel : BaseViewModel
    {
        public ManualAttendanceBulkSetupViewModel Setup = new ManualAttendanceBulkSetupViewModel();

        public List<ManualAttendanceBulkSetupViewModel> ListTableData = new List<ManualAttendanceBulkSetupViewModel>();
    }
}
