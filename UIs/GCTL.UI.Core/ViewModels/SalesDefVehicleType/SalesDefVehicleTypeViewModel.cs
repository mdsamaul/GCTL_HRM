using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.SalesDefVehicleType;

namespace GCTL.UI.Core.ViewModels.SalesDefVehicleType
{
    public class SalesDefVehicleTypeViewModel:BaseViewModel
    {
        public SalesDefVehicleTypeSetupViewModel Setup { get; set; } = new SalesDefVehicleTypeSetupViewModel();
    }
}
