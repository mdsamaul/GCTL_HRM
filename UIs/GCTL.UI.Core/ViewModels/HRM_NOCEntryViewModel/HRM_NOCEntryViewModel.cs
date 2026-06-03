using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HRLettersReportViewModel;
using GCTL.Core.ViewModels.HRM_NOCEntry;

namespace GCTL.UI.Core.ViewModels.HRM_NOCEntryViewModel
{
    public class HRM_NOCEntryViewModel: BaseViewModel
    {
        public HRM_NOCEntrySetupViewModel Setup { get; set; } = new HRM_NOCEntrySetupViewModel();
    }
}
