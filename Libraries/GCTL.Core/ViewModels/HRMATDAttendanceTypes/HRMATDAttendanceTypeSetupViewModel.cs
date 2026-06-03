using System.ComponentModel.DataAnnotations;

namespace GCTL.Core.ViewModels.HRMATDAttendanceTypes
{
    public class HRMATDAttendanceTypeSetupViewModel : BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string AttendanceTypeCode { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string AttendanceTypeName { get; set; }
        public string ShortName { get; set; }
    }
}
