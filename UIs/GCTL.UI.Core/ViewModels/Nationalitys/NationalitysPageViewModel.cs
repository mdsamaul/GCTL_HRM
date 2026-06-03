using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.Nationalitys;

namespace GCTL.UI.Core.ViewModels.Nationalitys
{
    public class NationalitysPageViewModel : BaseViewModel
    {
        public NationalitysSetupViewModel Setup { get; set; } = new NationalitysSetupViewModel();
        public List<NationalitysSetupViewModel> NationalitysList { get; set; } = new List<NationalitysSetupViewModel>();
    }
}
