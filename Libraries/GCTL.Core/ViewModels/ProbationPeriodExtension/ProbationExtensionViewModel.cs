using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.ProbationPeriodExtension
{
    public class ProbationExtensionViewModel
    {
        public string EmployeeID { get; set; }
        public string FullName { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        public DateTime? JoiningDate { get; set; }
        public string showJoiningDate { get; set; }
        public decimal? GrossSalary { get; set; }
        public string ProbationPeriod { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public string showContractEndDate { get; set; }
        public string DurationSinceJoining { get; set; }
    }

}
