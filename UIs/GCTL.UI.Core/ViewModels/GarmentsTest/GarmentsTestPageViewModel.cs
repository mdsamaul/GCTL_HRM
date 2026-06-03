using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.GarmentsTest;
using GCTL.Core.ViewModels.SizeInformation;

namespace GCTL.UI.Core.ViewModels.GarmentsTest
{
    public class GarmentsTestPageViewModel : BaseViewModel
    {
        public GarmentsTestSetupViewModel Setup { get; set; } = new GarmentsTestSetupViewModel();
        public List<GarmentsTestSetupViewModel> GarmentsTestList { get; set; } = new List<GarmentsTestSetupViewModel>();
    }
}
