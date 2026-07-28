using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HrmEmployeeSalaryInfoEntry;

namespace GCTL.UI.Core.ViewModels.HrmEmpSalaryInfoEntry
{
    public class HrmEmpSalaryInfoEntryViewModel : BaseViewModel
    {
        public EmployeeListItemViewModel Setup { get; set; } = new EmployeeListItemViewModel();
        public string AccessCode { get; set; }
    }
}
