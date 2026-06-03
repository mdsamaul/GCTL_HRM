namespace GCTL.Core.ViewModels.HrmAtdMachineData
{
    public class EmployeeAttendanceViewModel
    {
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public string FingerPrintId { get; set; }
        public string MachineId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Remarks { get; set; }
    }

}
