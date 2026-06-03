using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.PFAssignEntry
{
    public class PFAssignEntryFilterResultDto :BaseViewModel
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? EmpID { get; set; }
        public string? Designation { get; set; }
        public string? Department { get; set; }
        public string? Branch { get; set; }
        public string? Company { get; set; }
        public string? EmployeeType { get; set; }
        public string? EmploymentNature { get; set; }
        public string? JoiningDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? DaysInYear { get; set; }
        public double? ServiceDuration { get; set; }
        public DateTime? SeparationDate { get; set; }
        public string? ServiceDuration2 { get; set; }
        public string ShowDate { get; set; }
        public string dayName { get; set; }
        public string Remarks { get; set; }
        public string PFApprove { get; set; }
    }
}
