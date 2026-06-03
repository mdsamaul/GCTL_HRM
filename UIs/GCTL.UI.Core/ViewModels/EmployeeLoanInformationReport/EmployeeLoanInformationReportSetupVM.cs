using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.EmployeeLoanInformationReport;

namespace GCTL.UI.Core.ViewModels.EmployeeLoanInformationReport
{
    public class EmployeeLoanInformationReportSetupVM:BaseViewModel
    {
        public EmployeeLoanInformationReportVM setupEmpLoan {  get; set; }  = new EmployeeLoanInformationReportVM();
    }
}
