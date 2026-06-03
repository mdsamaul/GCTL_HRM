using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.UnitTypes;

namespace GCTL.UI.Core.ViewModels.UnitTypes
{
    public class UnitTypePageViewModel : BaseViewModel
    {
        public UnitTypeSetupViewModel Setup { get; set; } = new UnitTypeSetupViewModel();
    }
}
