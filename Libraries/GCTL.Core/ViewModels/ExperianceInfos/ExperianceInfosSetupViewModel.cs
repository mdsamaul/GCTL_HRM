using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.ExperianceInfos
{
    public class ExperianceInfosSetupViewModel: BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string EmpExpId { get; set; }
        [Required]
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        [Required]
        public string CompanyNameId { get; set; }
        public string CompanyName1 {  get; set; }
        [Required]
        public string BusinessType { get; set; }
        [Required]
        public string Address { get; set; }
        public string DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        [Required]
        public string DesignationId { get; set; }
        public string DesignationName { get; set; }
        [Required]
        public string Responsibilities { get; set; }
        [Required]
        public string JobNatureId { get; set; }
        [Required]
       // [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public string FromDate { get; set; }
        [Required]
       // [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public string ToDate { get; set; }
        [Required]
        public decimal Salary { get; set; }
        [Required]
        public string Remarks { get; set; }
        [Required]
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string Experience { get; set; }
    }
}
