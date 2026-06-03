using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HrmEmployeeEducations;

namespace GCTL.UI.Core.ViewModels.HrmEmployeeEducations
{
    public class HrmEmployeeEducationsPageViewModel:BaseViewModel
    {
        public HrmEmployeeEducationsSetupViewModel Setup { get; set; } =new HrmEmployeeEducationsSetupViewModel();
        public List<HrmEmployeeEducationsSetupViewModel> TableListData { get; set; } = new List<HrmEmployeeEducationsSetupViewModel>();
    }
}
