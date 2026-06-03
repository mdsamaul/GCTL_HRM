using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.MonthlyTransportExpenseDetailsReport
{
    public class MonthlyTransportExpenseDetailsReportFilterDataDto
    {
        // Multiple selection (Lists)
        public List<string> TransportTypeIds { get; set; }
        public List<string>? TransportIds { get; set; }
        public List<string>? DriverEmployeeIds { get; set; }

        // Date filters
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        // Month, Year
        public int? Month { get; set; }
        public int? Year { get; set; }
    }
}
