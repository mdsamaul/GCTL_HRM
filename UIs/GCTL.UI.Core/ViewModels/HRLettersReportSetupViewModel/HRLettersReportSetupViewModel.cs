using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HRLettersReportViewModel;

namespace GCTL.UI.Core.ViewModels.HRLettersReportSetupViewModel
{
    public class HRLettersReportSetupViewModel:BaseViewModel
    {
        public HRLettersReportViewModel Setup { get; set; } = new HRLettersReportViewModel();
    }
}
