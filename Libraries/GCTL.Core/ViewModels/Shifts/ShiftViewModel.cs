using System.ComponentModel.DataAnnotations;

namespace GCTL.Core.ViewModels.Shifts
{
    public class ShiftViewModel 
    {
        public string ShiftId { get; set; }
        public string ShiftCode { get; set; }
        public string ShiftName { get; set; }
        public string ShiftShortName { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
    }
}
