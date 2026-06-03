using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.AdvanceLoanAdjustmentReport
{
    public class DepartmentGroupedData:BaseViewModel
    {
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public int TotalEmployees { get; set; }
        public List<AdvanceLoanAdjustmentReportSetupViewModel> Employees { get; set; } = new List<AdvanceLoanAdjustmentReportSetupViewModel>();
    }
}
