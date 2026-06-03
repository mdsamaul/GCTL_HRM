using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HrmDefPerformance;


namespace GCTL.UI.Core.ViewModels.HrmDefPerformance
{
    public class HrmDefPerformancePageViewModel : BaseViewModel
    {
        public HrmDefPerformanceSetupViewModel Setup { get; set; } = new HrmDefPerformanceSetupViewModel();
        public List<HrmDefPerformanceSetupViewModel> HrmDefPerformanceList { get; set; } = new List<HrmDefPerformanceSetupViewModel>();
    }
}
