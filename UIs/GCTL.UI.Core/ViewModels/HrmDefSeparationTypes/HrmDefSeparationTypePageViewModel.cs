using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HrmDefSeparationTypes;

namespace GCTL.UI.Core.ViewModels.HrmDefSeparationTypes
{
    public class HrmDefSeparationTypePageViewModel : BaseViewModel
    {
        public HrmDefSeparationTypeSetupViewModel Setup { get; set; } = new HrmDefSeparationTypeSetupViewModel();
        public List<HrmDefSeparationTypeSetupViewModel> HrmDefSeparationTypeList { get; set; } = new List<HrmDefSeparationTypeSetupViewModel>();
    }
}
