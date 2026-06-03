using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.Units;

namespace GCTL.UI.Core.ViewModels.Units
{
    public class UnitPageViewModel : BaseViewModel
    {
        public UnitSetupViewModel Setup { get; set; } = new UnitSetupViewModel();
    }
}
