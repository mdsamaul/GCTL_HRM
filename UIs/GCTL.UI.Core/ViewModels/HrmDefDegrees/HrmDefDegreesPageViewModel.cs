using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HrmDefDegrees;

namespace GCTL.UI.Core.ViewModels.HrmDefDegrees
{
    public class HrmDefDegreesPageViewModel:BaseViewModel
    {
        public HrmDefDegreesSetupViewModel Setup { get; set; } = new HrmDefDegreesSetupViewModel();
        public List<HrmDefDegreesSetupViewModel> TableList { get; set; } = new List<HrmDefDegreesSetupViewModel>();
    }
}
