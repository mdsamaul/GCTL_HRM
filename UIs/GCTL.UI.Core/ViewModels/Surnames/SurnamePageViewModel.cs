using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.Surnames;

namespace GCTL.UI.Core.ViewModels.Surnames
{
    public class SurnamePageViewModel : BaseViewModel
    {
        public SurnameSetupViewModel Setup { get; set; } = new SurnameSetupViewModel();
    }
}
