using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.AdvanceLoanAdjustmentReport
{
     public class HrmAdvancePayReportFilter :BaseViewModel
    {     
        public List<string> CompanyCodes { get; set; }
        public List<string> BranchCodes { get; set; }
        public List<string> DepartmentCodes { get; set; }
        public List<string> DesignationCodes { get; set; }
        public List<string> EmployeeIDs { get; set; }
        public List<string> PayHeadIDs { get; set; }
        public List<string> MonthIDs { get; set; }
        public List<string> YearIDs { get; set; }
    }
}
