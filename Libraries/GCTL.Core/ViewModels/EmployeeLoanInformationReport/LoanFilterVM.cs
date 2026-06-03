using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.EmployeeLoanInformationReport
{
    public class LoanFilterVM
    {
        public string CompanyID { get; set; }
        public string EmployeeID { get; set; }
        public string LoanID { get; set; }
        public string LoanTypeID { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }

}
