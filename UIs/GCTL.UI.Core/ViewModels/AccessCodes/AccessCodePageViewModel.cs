using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.AccessCodes;

namespace GCTL.UI.Core.ViewModels.AccessCodes
{
    public class AccessCodePageViewModel : BaseViewModel
    {
        public AccessCodeSetupViewModel Setup { get; set; } = new AccessCodeSetupViewModel();
    }
}
