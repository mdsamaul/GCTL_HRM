using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.BloodGroups;

namespace GCTL.UI.Core.ViewModels.BloodGroups
{
    public class BloodGroupPageViewModel : BaseViewModel
    {
        public BloodGroupSetupViewModel Setup { get; set; } = new BloodGroupSetupViewModel();
    }
}
