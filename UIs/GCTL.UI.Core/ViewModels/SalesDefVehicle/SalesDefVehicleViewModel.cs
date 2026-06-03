using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.SalesDefVehicle;
using GCTL.Core.ViewModels.SalesDefVehicleType;

namespace GCTL.UI.Core.ViewModels.SalesDefVehicle
{
    public class SalesDefVehicleViewModel : BaseViewModel
    {
        public SalesDefVehicleSetupViewModel Setup { get; set; } = new SalesDefVehicleSetupViewModel();
    }
}
