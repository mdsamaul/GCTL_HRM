using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.EmployeeReferenceInfos
{
    public class EmployeeReferenceInfosSetupViewModel : BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string EmpReferenceId { get; set; }
        [Required(ErrorMessage = "EmployeeId is required")]
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        [Required(ErrorMessage = "ReferenceName is required")]
        public string ReferenceName { get; set; }
        public string OrganizationName { get; set; }
        public string Designation { get; set; }
        public string RefAddress { get; set; }
        public string RelationId { get; set; }
        public string RelationName { get; set; }
        [Display(Name = "Phone ")]
        [RegularExpression(@"^(?:(?:\+|00)88|01)?\d{11}$", ErrorMessage = "Invalid phone.")]
        [MaxLength(14)]
        public string MobileNumber { get; set; }
        [Display(Name = "Phone ")]
        [RegularExpression(@"^(?:(?:\+|00)88|01)?\d{11}$", ErrorMessage = "Invalid phone.")]
        [MaxLength(14)]
        public string PhoneNumber { get; set; }
        public string Fax { get; set; }
        [EmailAddress(ErrorMessage = "Please enter valid email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "CompanyCode is required")]
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string NationalityCode { get; set; }
        public string Nationality { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        public string CoreBranchName { get; set; }
    }
}
