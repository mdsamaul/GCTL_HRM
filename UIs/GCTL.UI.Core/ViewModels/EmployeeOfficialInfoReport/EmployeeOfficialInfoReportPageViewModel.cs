using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.EmployeeOfficialInfoReport;

namespace GCTL.UI.Core.ViewModels.EmployeeOfficialInfoReport
{
    public class EmployeeOfficialInfoReportPageViewModel:BaseViewModel
    {
       public EmployeeOfficialInfoReportSetupViewModel Setup {  get; set; }  = new EmployeeOfficialInfoReportSetupViewModel();
    }
} 
