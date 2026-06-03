namespace GCTL.Core.ViewModels.ManualAttendance
{
    public class ManualAttendanceSetupViewModel : BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string ManualCode { get; set; }
        public string BulkEntryId { get; set; }
        public string AttdEntryType { get; set; }
        public string EmployeeId { get; set; }
        public string AttendanceTypeCode { get; set; }
        public string AttendanceTypeCodeTwo { get; set; }
        //public DateTime DateFrom { get; set; }
        //public DateTime DateTo { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public DateTime EntryTime { get; set; }
        public string ShowEntryTime { get; set; }
        public DateTime ExitTime { get; set; }
        public string Remarks { get; set; }
        public string CompanyCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string EntryVia { get; set; }
        public string MonthName { get; set; }
        public string YearName { get; set; }
        public string DayName { get; set; }

        public string ApprovalStatus { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovalDatetime { get; set; }


        // Newly Added
        public string EmployeeFullName { get; set; }
        public string AttendanceTypeName { get; set; }
        public string CompanyName { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        public bool ISBothInOutEntry { get; set; }
    }


}
