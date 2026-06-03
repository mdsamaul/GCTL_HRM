using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.AttendanceMovementRegisterReportDto
{
  
    public class AttendanceMovementRegisterReportFilterData
    {
        public List<string> CompanyCodes { get; set; }
        public List<string> BranchCodes { get; set; }
        public List<string> DepartmentCodes { get; set; }
        public List<string> DesignationCodes { get; set; }
        public List<string> EmployeeIDs { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<int> MonthIDs { get; set; }
        public List<int> YearIDs { get; set; }
        public string? AccessCode { get; set; }
        public string? EmployeeId { get; set; }
    }
}
