using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.EmployeeOfficialInfoReport
{
    public class OfficialInfoReportFilterVm
    {
        public List<string> CompanyCodes { get; set; } = new();
        public List<string> BranchCodes { get; set; } = new();
        public List<string> DepartmentCodes { get; set; } = new();
        public List<string> DesignationCodes { get; set; } = new();
        public List<string> EmployeeCodes { get; set; } = new();

        public string EmployeeTypeCode { get; set; }
        public string EmploymentNatureId { get; set; }
        public string NationalId { get; set; }
        public string TinNo { get; set; }
        public string PassportNo { get; set; }
        public string DrivingLicense { get; set; }
        public string IsExpatriate { get; set; }
        public string ImmediateSup { get; set; }
        public string HOD { get; set; }
        public string ShiftCode { get; set; }
        public string EmployeeStatus { get; set; }

        // Salary Range
        public decimal? SalaryFrom { get; set; }
        public decimal? SalaryTo { get; set; }

        // Appointment Dates
        public DateTime? AppointmentDateFrom { get; set; }
        public DateTime? AppointmentDateTo { get; set; }

        // Joining Dates
        public DateTime? JoiningDateFrom { get; set; }
        public DateTime? JoiningDateTo { get; set; }

        // Termination Dates
        public DateTime? TerminationDateFrom { get; set; }
        public DateTime? TerminationDateTo { get; set; }

        // Probation Dates
        public DateTime? ProbationDateFrom { get; set; }
        public DateTime? ProbationDateTo { get; set; }

        // Confirmation Dates
        public DateTime? ConfirmationDateFrom { get; set; }
        public DateTime? ConfirmationDateTo { get; set; }

    }
}
