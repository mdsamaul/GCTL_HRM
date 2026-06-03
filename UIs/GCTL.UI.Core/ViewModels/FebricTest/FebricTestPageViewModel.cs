using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.FebricTest;

namespace GCTL.UI.Core.ViewModels.FebricTest
{
    public class FebricTestPageViewModel : BaseViewModel
    {
        public FebricTestSetupViewModel Setup { get; set; } = new FebricTestSetupViewModel();
        public List<FebricTestSetupViewModel> FebricTestList { get; set; } = new List<FebricTestSetupViewModel>();
    }
}
