using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.MonthlyTransportExpenseDetailsReport
{
    public class MonthlyTransportExpenseDetailsReportSetupDto:BaseViewModel
    {        

        public string MonthlyTransportId { get; set; }
        public DateTime ReportDate { get; set; }
        public List<MonthlyTransportExpenseDetailsReportListDto> ReportList { get; set; }

        public string ShowReportDate => ReportDate.ToString("dd/MM/yyyy");
    }
}
