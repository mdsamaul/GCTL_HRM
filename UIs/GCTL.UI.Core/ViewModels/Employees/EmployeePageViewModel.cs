using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.Employees;

namespace GCTL.UI.Core.ViewModels.Employees
{
    public class EmployeePageViewModel : BaseViewModel
    {
        public EmployeeSetupViewModel Setup { get; set; } = new EmployeeSetupViewModel();
    }
}
