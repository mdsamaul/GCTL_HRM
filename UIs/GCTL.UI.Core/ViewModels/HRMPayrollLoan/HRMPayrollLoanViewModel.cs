using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HRMPayrollLoan;

namespace GCTL.UI.Core.ViewModels.HRMPayrollLoan
{
    public class HRMPayrollLoanViewModel : BaseViewModel
    {
        public HRMPayrollLoanSetupViewModel Setup { get; set; } = new HRMPayrollLoanSetupViewModel();
    }
}
