namespace GCTL.Core.ViewModels.ManualAttendance
{
    public class DeleteRequestModel
    {
        public List<string> Ids { get; set; }
        public List<string> SelectedEmployeeIds { get; set; }
        public string AttendanceTypeCode { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public bool IsBothInOutEntry { get; set; }
    }
}
