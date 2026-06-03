using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.Doctors;

namespace GCTL.UI.Core.ViewModels.Doctors
{
    public class DoctorPageViewModel : BaseViewModel
    {
        public DoctorSetupViewModel Setup { get; set; } = new DoctorSetupViewModel();
        public DoctorFilterViewModel Filter { get; set; } = new DoctorFilterViewModel();
    }
}
