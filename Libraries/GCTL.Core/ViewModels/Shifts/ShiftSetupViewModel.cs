using System.ComponentModel.DataAnnotations;

namespace GCTL.Core.ViewModels.Shifts
{
    public class ShiftSetupViewModel : BaseViewModel
    {
        public string ShiftId { get; set; }
        public string ShiftCode { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Shift Name")]
        public string ShiftName { get; set; }

        [Display(Name = "Shift Short Name")]
        public string ShiftShortName { get; set; }

        [Display(Name = "In Time")]
        public string InTime { get; set; }

        [Display(Name = "Out Time")]
        public string OutTime { get; set; }
    }
}
