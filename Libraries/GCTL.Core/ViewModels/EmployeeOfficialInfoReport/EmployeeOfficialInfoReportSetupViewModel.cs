
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.EmployeeOfficialInfoReport
{
    public class EmployeeOfficialInfoReportSetupViewModel:BaseViewModel
    {
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string ComapnyBranchName { get; set; }
        public string BranchCode { get; set; }
        public string DepartmentName { get; set; }
        public string EmployeeType { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string NationalID { get; set; }
        public string DrivingLicense { get; set; }
        public string ImmediateSupervisor { get; set; }
        public string ImmediateSupervisorCode { get; set; }
        public string Shift { get; set; }
        public decimal? GrossSalaryRangeFrom { get; set; }
        public decimal? GrossSalaryRangeTo { get; set; }
        public string AppoinmentDateFrom { get; set; }
        public string AppoinmentDateTo { get; set; }
        public string JoiningDateFrom { get; set; }
        public string JoiningDateTo { get; set; }
        public string TerminationDateFrom { get; set; }
        public string TerminationDateTo { get; set; }
        public string ProbationDateFrom { get; set; }
        public string ProbationDateTo { get; set; }
        public string ConfirmationDateFrom { get; set; }
        public string ConfirmationDateTo { get; set; }
        public string Designation { get; set; }
        public string DesignationCode { get; set; }
        public string EmploymentNature { get; set; }
        public string IsExpatriate { get; set; }
        public string TIN { get; set; }
        public string PassportNo { get; set; }
        public string HeadOfDepartment { get; set; }
        public string HeadOfDepartmentCode { get; set; }
        public string EmployeeStatusCode { get; set; }
        public string EmployeeStatus { get; set; }
        public string ExportPdfXL { get; set; }

        //public static implicit operator EmployeeOfficialInfoReportSetupViewModel(ProfessionalQualificationReportSetupViewModel v)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
