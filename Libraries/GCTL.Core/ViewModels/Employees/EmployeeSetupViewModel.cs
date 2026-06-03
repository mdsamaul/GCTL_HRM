using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace GCTL.Core.ViewModels.Employees
{
    public class EmployeeSetupViewModel : BaseViewModel
    {
        public decimal autoId { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Employee ID")]
        public string EmployeeID { get; set; }
        public string EditEmployeeID { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Father Name")]
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string DateOfBirth { get; set; }
        public string BloodGroupCode { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Gender")]
        public string SexCode { get; set; }
        public string ReligionCode { get; set; }
        public string PresentAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string MobileNo { get; set; }

        [EmailAddress(ErrorMessage = "Please enter valid email")]
        public string Email { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string DepartmentCode { get; set; }
        public string DesignationCode { get; set; }
        public string EmpPhoto { get; set; }
        public string FingerPrintId { get; set; }
        public string DigitalSignature { get; set; }
        public string NationalIdNo { get; set; }
        public string EmployeeStatus { get; set; }
        public bool IsClearImage { get; set; }

        public IFormFile Photo { get; set; }
    }
}
