using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.ManualEntryApproval
{
    public class ManualEntryApprovalSetupViewModelDto:BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string ManualCode { get; set; }
        public string BulkEntryId { get; set; }
        public string AttdEntryType { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public string AttendanceTypeCode { get; set; }
        public string AttendanceTypeName { get; set; }
        public DateTime Date { get; set; }
        public string ShowDate { get; set; }
        public DateTime Time { get; set; }
        public string Remarks { get; set; }
        public string CompanyCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string EntryVia { get; set; }
        public string ApprovalStatus { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovalDatetime { get; set; }
        public string? ShowApprovalDatetime { get; set; }
        public string EntryUser { get; set; }
        public string MonthName { get; set; }
        public string YearName { get; set; }
        public string DayName { get; set; }
    }
}
