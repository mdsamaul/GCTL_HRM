using System.ComponentModel.DataAnnotations;

namespace GCTL.Core.ViewModels.Designations
{
    public class DesignationSetupViewModel : BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string DesignationCode { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Designation Name")]
        public string DesignationName { get; set; }

        [Display(Name = "Designaiton Short Name")]
        public string DesignationShortName { get; set; }

        [Display(Name = "Designaiton Short Name")]
        public string BanglaDesignation { get; set; } = string.Empty;
       
        public string BanglaShortName { get; set; } = string.Empty;
        public string StepNoId { get; set; }
        public string CompanyCode { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string MobileAllowanceId { get; set; } = string.Empty;
    }
}
