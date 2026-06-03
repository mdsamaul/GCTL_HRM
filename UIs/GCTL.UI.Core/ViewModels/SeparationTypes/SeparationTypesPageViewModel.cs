using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.SeparationTypes;

namespace GCTL.UI.Core.ViewModels.SeparationTypes
{
    public class SeparationTypesPageViewModel : BaseViewModel
    {
        public SeparationTypesSetupViewModel Setup { get; set; } = new SeparationTypesSetupViewModel();
        public List<SeparationTypesSetupViewModel> SeparationList { get; set; } = new List<SeparationTypesSetupViewModel>();
    }
}
