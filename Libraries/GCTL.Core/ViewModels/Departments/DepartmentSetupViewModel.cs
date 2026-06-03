using System.ComponentModel.DataAnnotations;

namespace GCTL.Core.ViewModels.Departments
{
    public class DepartmentSetupViewModel : BaseViewModel
    {
        public string DepartmentCode { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name ="Department Name")]
        public string DepartmentName { get; set; }

        [Display(Name = "Department Short Name")]
        public string DepartmentShortName { get; set; }
        [Display(Name = "Department ")]
        public string BanglaDepartment { get; set; }
        public string CompanyCode { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
    }
}
