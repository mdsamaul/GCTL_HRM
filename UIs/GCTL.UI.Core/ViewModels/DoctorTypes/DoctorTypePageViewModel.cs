using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.DoctorTypes;

namespace GCTL.UI.Core.ViewModels.DoctorTypes
{
    public class DoctorTypePageViewModel : BaseViewModel
    {
        public DoctorTypeSetupViewModel Setup { get; set; } = new DoctorTypeSetupViewModel();
    }
}
