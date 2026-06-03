namespace GCTL.Core.ViewModels.ManualAttendanceBulk
{
    public class ManualAttendanceBulkSetupViewModel : BaseViewModel
    {
        public string shift { get; set; }

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


        // Newly Added
        public string EmployeeFullName { get; set; }
        public string AttendanceTypeName { get; set; }
        public string CompanyName { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public bool ISBothInOutEntry { get; set; }
        public string ListTypeCode { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string DesignationCode { get; set; }
        public string ActivityStatusCode { get; set; }
        public DateTime? InTime { get; set; }
        public string ShowInTime { get; set; }
        public DateTime? OutTime { get; set; }
        public string ShowOutTime { get; set; }
    }
}
