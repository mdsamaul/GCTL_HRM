using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HRM_EmployeeWeekendDeclaration;

namespace GCTL.UI.Core.ViewModels.HRM_EmployeeWeekendDeclaration
{
    public class HRM_EmployeeWeekendDeclarationViewModel : BaseViewModel
    {
      public HRM_EmployeeWeekendDeclarationSetupViewModel Setup { get; set; } = new HRM_EmployeeWeekendDeclarationSetupViewModel();
        public HRM_EmployeeWeekendDeclarationDto EmployeeWeekendDeclaration { get; set; } = new HRM_EmployeeWeekendDeclarationDto();
    }
}
