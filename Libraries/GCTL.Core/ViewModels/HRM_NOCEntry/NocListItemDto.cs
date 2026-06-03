using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HRM_NOCEntry
{
    // HRM_NOCEntryViewModel.cs (add these)
    public class NocListItemDto:BaseViewModel
    {
        public decimal? AutoId { get; set; }
        public string? NocId { get; set; }
        public string? EmployeeID { get; set; }
        public string? EmployeeName { get; set; }
        // Travel
        public string? PlaceofVisit { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        // Education
        public string? UniversityName { get; set; }
        public string? CourseName { get; set; }
        // Common
        public string? Remarks { get; set; }
    }
}
