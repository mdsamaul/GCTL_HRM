using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.Designations;

namespace GCTL.UI.Core.ViewModels.Designations
{
    public class DesignationPageViewModel : BaseViewModel
    {
        public DesignationSetupViewModel Setup { get; set; } = new DesignationSetupViewModel();
    }
}
