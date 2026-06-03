using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.EmployeeContactInfos;

namespace GCTL.UI.Core.ViewModels.EmployeeContactInfos
{
    public class EmployeeContactInfosPageViewModel : BaseViewModel
    {
        public EmployeeContactInfosSetupViewModel Setup = new EmployeeContactInfosSetupViewModel();
        public List<EmployeeContactInfosSetupViewModel> ListTableData = new List<EmployeeContactInfosSetupViewModel>();
    }
}
