using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HrmEmployeeQualifications;

namespace GCTL.UI.Core.ViewModels.HrmEmployeeQualifications
{
    public class HrmEmployeeQualificationsPageViewModel:BaseViewModel
    {
         public HrmEmployeeQualificationsSetupViewModel Setup {  get; set; }=new HrmEmployeeQualificationsSetupViewModel();
        public List<HrmEmployeeQualificationsSetupViewModel> TableListData { get; set; } = new List<HrmEmployeeQualificationsSetupViewModel>();
    }
}
