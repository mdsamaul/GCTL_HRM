using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.Departments;

namespace GCTL.UI.Core.ViewModels.Departments
{
    public class DepartmentPageViewModel : BaseViewModel
    {
        public DepartmentSetupViewModel Setup { get; set; } = new DepartmentSetupViewModel();

    }
}
