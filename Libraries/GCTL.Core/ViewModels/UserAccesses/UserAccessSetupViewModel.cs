using System.ComponentModel.DataAnnotations;

namespace GCTL.Core.ViewModels.UserAccesses
{
    public class UserAccessSetupViewModel : BaseViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string UserPassword { get; set; }
        public bool Status { get; set; }
        public string AccessCode { get; set; }
        public string EmployeeId { get; set; }
        public string CompanyCode { get; set; }
        public string EmployeeName { get; set; }
        public string Role { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }

        public string JoiningDate { get; set; }
        public string NationalId { get; set; }
        public string Company { get; set; }
        public string Branch { get; set; }
        public string EmployeeType { get; set; }
        public string EmployeeNature { get; set; }
        public string OfficePhone { get; set; }
        public string OfficeEmail { get; set; }
        public string WorkStation { get; set; }

        public string SingleSession { get; set; }
        public bool isRequired => UserId == 0 || !string.IsNullOrWhiteSpace(EmployeeId);

        [Display(Name = "Company")]
        public List<string> AccessPermissionCompanyCode { get; set; }
        [Display(Name = "Branch")]
        public List<string> AccessPermissionBranchCode { get; set; }
        //[Display(Name = "Department")]
        //public string AccessPermissionDepartmentCode { get; set; }


    }
}
