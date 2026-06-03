using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.EmployeeGeneralInfoReport;


namespace GCTL.UI.Core.ViewModels.EmployeeGeneralInfoReport
{
    public class EmployeeGeneralInfoReportPageViewModel:BaseViewModel
    {
         public EmployeeGeneralInfoReportSetupViewModel Setup { get; set; }=new EmployeeGeneralInfoReportSetupViewModel();
    }
}
