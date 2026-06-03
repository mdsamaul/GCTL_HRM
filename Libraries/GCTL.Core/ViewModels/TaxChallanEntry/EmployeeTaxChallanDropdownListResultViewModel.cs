using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.TaxChallanEntry
{
    public class EmployeeTaxChallanDropdownListResultViewModel
    {
        public List<CommonDto> CompanyesList { get; set; }
        public List<CommonDto> EmployeesList { get; set; }
        public List<CommonDto> DesignationsList { get; set; }
        public List<CommonDto> BranchList { get; set; }
        public List<CommonDto> DepartmentsList { get; set; }
        public List<EmployeeTaxChallanResultViewModel>? EmployeeList { get; set; } = new List<EmployeeTaxChallanResultViewModel>(); 
    }

    public class CommonDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
