using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.BranchesTypeInfo;


namespace GCTL.UI.Core.ViewModels.BranchesTypeInfo
{
    public class BranchTypePageViewModel : BaseViewModel
    {
        public BranchTypeSetupViewModel Setup { get; set; } = new BranchTypeSetupViewModel();
    }
}
