using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.ExcessTDSForLastIncomeYear
{
    public class ExcessTDSForLastIncomeYearFilterDto
    {
        public List<string> CompanyCodes { get; set; }
        public List<string> BranchCodes { get; set; }
        public List<string> DepartmentCodes { get; set; }
        public List<string> DesignationCodes { get; set; }
        public List<string> EmployeeIDs { get; set; }
        public List<string> EmployeeTypeCodes { get; set; }
        public List<string> EmploymentNatureId { get; set; }
        public List<string> EmploymentNature { get; set; }
        public List<string> EmployeeStatuses { get; set; }
        public List<string> ActivityStatuses { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public List<string> EmployeeTypeIDs { get; set; }
    }
}
