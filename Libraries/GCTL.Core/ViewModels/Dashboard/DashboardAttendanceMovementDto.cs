namespace GCTL.Core.ViewModels.Dashboard
{
    public class DashboardAttendanceMovementDto
    {
        public int RowNum { get; set; }
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
        public string Movement { get; set; }
        public byte[] Photo { get; set; }
        public string ImgType { get; set; }
        public int TotalCount { get; set; }
        public int StatusOrder { get; set; }  // 1=Present 2=Late 3=OnLeave 4=Absent
        public string Status { get; set; }    // "Present" | "Late" | "On Leave" | "Absent"
        public DateTime DataDate { get; set; }
        public string Remarks { get; set; }
        public object LateByMinutes { get; set; }
    }
}