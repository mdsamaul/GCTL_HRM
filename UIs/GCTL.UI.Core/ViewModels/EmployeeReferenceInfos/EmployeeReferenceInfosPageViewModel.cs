using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.EmployeeReferenceInfos;

namespace GCTL.UI.Core.ViewModels.EmployeeReferenceInfos
{
    public class EmployeeReferenceInfosPageViewModel: BaseViewModel
    {
        public EmployeeReferenceInfosSetupViewModel Setup { get; set; } = new EmployeeReferenceInfosSetupViewModel();
        public List<EmployeeReferenceInfosSetupViewModel> TableListData { get; set; } = new List<EmployeeReferenceInfosSetupViewModel>();
    }
}
