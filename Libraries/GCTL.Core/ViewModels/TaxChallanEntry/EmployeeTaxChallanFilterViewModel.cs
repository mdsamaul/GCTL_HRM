using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.TaxChallanEntry
{
    public class EmployeeTaxChallanFilterViewModel
    {
        public List<string> CompanyCodes { get; set; } = new List<string>();
        public List<string> EmployeeIds { get; set; } = new List<string>();
        public List<string> DesignationCodes { get; set; } = new List<string>();
        public List<string> DepartmentCodes { get; set; } = new List<string>();
        public List<string> BranchCodes { get; set; } = new List<string>();
        public string ActiveStatus { get; set; }
        
    }
}
