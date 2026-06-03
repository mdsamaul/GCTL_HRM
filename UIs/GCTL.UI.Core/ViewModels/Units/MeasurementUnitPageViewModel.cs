using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.Units;

namespace GCTL.UI.Core.ViewModels.Units
{
    public class MeasurementUnitPageViewModel : BaseViewModel
    {
        public MeasurementUnitSetupViewModel Setup { get; set; } = new MeasurementUnitSetupViewModel();
    }
}
