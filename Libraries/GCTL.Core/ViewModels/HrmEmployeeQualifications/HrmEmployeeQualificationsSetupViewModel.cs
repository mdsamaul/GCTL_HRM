using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HrmEmployeeQualifications
{
    public class HrmEmployeeQualificationsSetupViewModel : BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string EmpQualificationId { get; set; }
        [Required]
        public string EmployeeId { get; set; }
        [Required]
        public string CourseCode { get; set; }
        public string CourseTitleCode { get; set; }
        public string InstitueCode { get; set; }
        public string Instituteaddress { get; set; }
        public string ResultDivision { get; set; }
        public string YearofPasssing { get; set; }
        public string Dueration { get; set; }
        public string DuratioinType { get; set; }
        public string Achievment { get; set; }
        public string Remarks { get; set; }
        public string CompanyCode { get; set; }

        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }

        public string InstituteName { get; set; }
        public string CourseTittleName { get; set; }
    }
}
