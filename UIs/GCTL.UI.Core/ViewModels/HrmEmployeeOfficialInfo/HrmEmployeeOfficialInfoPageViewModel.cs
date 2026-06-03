using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HrmEmployeeOfficialInfo;

namespace GCTL.UI.Core.ViewModels.HrmEmployeeOfficialInfo
{
    public class HrmEmployeeOfficialInfoPageViewModel : BaseViewModel
    {
        public HrmEmployeeOfficialInfoSetupViewModel Setup { get; set; } = new HrmEmployeeOfficialInfoSetupViewModel();
        public List<HrmEmployeeOfficialInfoSetupViewModel> SetupList { get; set; } = new List<HrmEmployeeOfficialInfoSetupViewModel>();
    }
}
