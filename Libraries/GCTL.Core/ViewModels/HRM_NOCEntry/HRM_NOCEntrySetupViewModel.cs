using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HRM_NOCEntry
{
    public class HRM_NOCEntrySetupViewModel:BaseViewModel
    {
        public decimal? AutoId { get; set; }

        public string? NOCID { get; set; }

        public string? NOCTypeId { get; set; }

        public string? EmployeeID { get; set; }

        public string? PlaceofVisit { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string? UniversityName { get; set; }

        public string? CourseName { get; set; }

        public string? Remarks { get; set; }
        public string? CompanyCode { get; set; }
        public string UserEmployeeID { get; set; }
    }
}
