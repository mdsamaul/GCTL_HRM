using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.ProbationPeriodExtension
{
    public class ProbationPeriodExtensionGetAll
    {
        public decimal AutoId { get; set; }
        public string Ppeid { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string ExtendedPeriod { get; set; }
        public string Wef { get; set; }
        public string RefLetterNo { get; set; }
        public string RefLetterDate { get; set; }
        public decimal? ExtensionSalary { get; set; }
        public string Remarks { get; set; }
    }
}
