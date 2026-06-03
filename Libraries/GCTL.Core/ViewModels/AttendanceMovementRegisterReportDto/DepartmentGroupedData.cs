using GCTL.Core.ViewModels.AdvanceLoanAdjustmentReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.AttendanceMovementRegisterReportDto
{
    public class DepartmentGroupedData
    {

        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public int TotalEmployees { get; set; }
        public List<AttendanceMovementRegisterReportDto> Employees { get; set; } = new List<AttendanceMovementRegisterReportDto>();
    }
}
