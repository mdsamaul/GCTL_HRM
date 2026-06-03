using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HrmDefPerformance2;


namespace GCTL.UI.Core.ViewModels.HrmDefPerformance2
{
    public class HrmDefPerformance2PageViewModel : BaseViewModel
    {
        public HrmDefPerformance2SetupViewModel Setup { get; set; } = new HrmDefPerformance2SetupViewModel();
        public List<HrmDefPerformance2SetupViewModel> HrmDefPerformanceList2 { get; set; } = new List<HrmDefPerformance2SetupViewModel>();
    }
}
