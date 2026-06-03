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
        public string Status { get; set; }
        public string AccessCode { get; set; }
        public string EmployeeId { get; set; }
        public string CompanyCode { get; set; }
        public string EmployeeName { get; set; }
        public string Role { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
    }
}
