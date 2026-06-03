using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.DoctorWorkingPlace;

namespace GCTL.UI.Core.ViewModels.DoctorWorkingPlace
{
    public class DoctorWokingPlacePageViewModel : BaseViewModel
    {
        public DoctorWorkingPlaceSetupViewModel Setup { get; set; } = new DoctorWorkingPlaceSetupViewModel();
    }
}
