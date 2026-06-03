using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.UserAccesses;

namespace GCTL.UI.Core.ViewModels.UserAcesses
{
    public class UserAccessPageViewModel : BaseViewModel
    {
        public UserAccessSetupViewModel Setup { get; set; } = new UserAccessSetupViewModel();
    }
}
