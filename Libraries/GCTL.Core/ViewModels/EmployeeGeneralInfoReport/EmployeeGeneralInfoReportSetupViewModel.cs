using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.EmployeeGeneralInfoReport
{
    public class EmployeeGeneralInfoReportSetupViewModel:BaseViewModel
    {
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string ComapnyBranchName { get; set; }
        public string BranchCode { get; set; }
        public string DepartmentName { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string Nationality { get; set; }
        public string NationalityCode { get; set; }
        public string Gender { get; set; }
        public string GenderCode { get; set; }
        public string BloodGroup { get; set; }

        public string BloodGroupCode { get; set; }
        public string Religion { get; set; }
        public string ReligionCode { get; set; }
        public string MaritalStatus { get; set; }
        public string MaritalStatusCode { get; set; }
        public string Designation { get; set; }
        public string DesignationCode { get; set; }
        public string ExportPdfXL { get; set; }
    }
}
