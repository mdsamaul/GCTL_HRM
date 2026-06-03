using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HRLettersReportViewModel
{
    public class SaveOrUpdateLetterRequestDto:BaseViewModel
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string LetterTypeId { get; set; } = string.Empty;
        public string EntryUserEmployeeId { get; set; } = string.Empty;
        public string CompanyCode { get; set; } = string.Empty;
        public string? NocId { get; set; }
        public DateTime? AppliedDate { get; set; }
    }
}
