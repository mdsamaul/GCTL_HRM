using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HrmAtdMachineData;
//using GCTL.Core.ViewModels.HrmEmployeeOfficialInfo;

namespace GCTL.UI.Core.ViewModels.HrmAtdMachineData
{
    public class HrmAtdMachineDataPageViewModel : BaseViewModel
    {
        public HrmAtdMachineDataSetupViewModel Setup { get; set; } = new HrmAtdMachineDataSetupViewModel();
    }
}
