using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HrmEmployeeEducations
{
    public class HrmEmployeeEducationsSetupViewModel:BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string EmpEduCode { get; set; }
        public string EmployeeName { get; set; }
        [Required]
        public string EmployeeId { get; set; }
        public string DegreeName { get; set; }
        [Required]
        public string DegreeCode { get; set; }
        public string ExamTitleName { get; set; }
        [Required]
        public string ExamTitleCode { get; set; }
        public string InstituteName { get; set; }
        [Required]
        public string InstitueCode { get; set; }
      
        public string BoardName { get; set; }
        [Required]
        public string BoardCode { get; set; }
        public string GroupName { get; set; }
        [Required]
        public string GroupCode { get; set; }
       
        public string ResultDivision { get; set; }
        [Required]
        public string CgpaMarks { get; set; }
        [Required]
        public string ScaleOf { get; set; }
        [Required]
        public string YearofPasssing { get; set; }
        [Required]
        public string Dueration { get; set; }
        [Required]
        public string DuratioinType { get; set; }
        public string Achievment { get; set; }
        public string Remarks { get; set; }
         public string ComapanyName { get; set; }
        [Required]
        public string CompanyCode { get; set; }
        public string BranchName { get; set; }    
        public string BranchCode { get; set; }
        public string DesignationName { get; set; }   
        public string DepartmentName { get; set; } 
    }
}


