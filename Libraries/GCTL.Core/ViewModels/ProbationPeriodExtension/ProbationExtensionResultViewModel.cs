using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.ProbationPeriodExtension
{
    public class ProbationExtensionResultViewModel
    {
        public List<ProbationExtensionViewModel> FullList { get; set; }
        public List<CompanyInfo1> CompanyList { get; set; }
        public List<EmployeeInfo> EmployeeList { get; set; }
    }

    public class CompanyInfo1
    {
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
    }

    public class EmployeeInfo
    {
        public string EmployeeID { get; set; }
        public string FullName { get; set; }
    }

}
