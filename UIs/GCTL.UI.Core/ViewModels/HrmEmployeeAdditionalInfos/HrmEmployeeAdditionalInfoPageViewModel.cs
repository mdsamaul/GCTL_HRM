using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HrmEmployeeAdditionalInfos;

namespace GCTL.UI.Core.ViewModels.HrmEmployeeAdditionalInfos
{
    public class HrmEmployeeAdditionalInfoPageViewModel:BaseViewModel
    {
        public HrmEmployeeAdditionalInfoSetupViewModel Setup { get; set; } = new HrmEmployeeAdditionalInfoSetupViewModel();
        public List<HrmEmployeeAdditionalInfoSetupViewModel> TableList { get; set; } = new List<HrmEmployeeAdditionalInfoSetupViewModel>();
    }
}
