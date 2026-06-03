using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HRLettersReportViewModel
{
    public class HRLetterReportRequestViewModel:BaseViewModel
    {
        public string EmployeeCode { get; set; }
        public string SignatoryEmployeeCode { get; set; }
        public string HrLetterTypeId { get; set; }
        public string Designation { get; set; }
        public string Mobile { get; set; }
        public string Telephone { get; set; }
        public string DateApplied { get; set; }
        public string LetterRefNo { get; set; }
        public string ReportFormat { get; set; } 
        public bool IsPreview { get; set; }
        public string? Email { get; set; }
        public DateTime? AppliedDate { get; set; }

        
        public DateTime? LeaveFrom { get; set; }
        public DateTime? LeaveTo { get; set; }
        public string? DestinationCountry { get; set; }
     
    }
}
