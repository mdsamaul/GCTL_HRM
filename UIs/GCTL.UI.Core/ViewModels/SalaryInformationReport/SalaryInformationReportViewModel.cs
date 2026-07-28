using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.SalaryInformationReport;

namespace GCTL.UI.Core.ViewModels.SalaryInformationReport
{
    public class SalaryInformationReportViewModel:BaseViewModel
    {
       
        public SalaryInformationReportFilterDto Filter { get; set; } = new SalaryInformationReportFilterDto();

        public List<SalaryInformationReportDto> Result { get; set; } = new List<SalaryInformationReportDto>();
        
    }
}
