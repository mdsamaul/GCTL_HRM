using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.Specialities;

namespace GCTL.UI.Core.ViewModels.Specialities
{
    public class SpecialityPageViewModel : BaseViewModel
    {
        public SpecialitySetupViewModel Setup { get; set; } = new SpecialitySetupViewModel();
    }
}
