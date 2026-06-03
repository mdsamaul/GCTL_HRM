using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HRM_EmployeeWeekendDeclaration
{
    public class EmployeeFilterResultDto
    {       
        public List<CodeNameDto> Companies { get; set; }
        public List<CodeNameDto> Branches { get; set; }
        public List<CodeNameDto> Divisions { get; set; }
        public List<CodeNameDto> Departments { get; set; }
        public List<CodeNameDto> Designations { get; set; }
        public List<CodeNameDto> Employees { get; set; }
        public List<CodeNameDto> ActivityStatuses { get; set; }
       
    }

}
