using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.AttendanceMovementRegisterReportCount
{
    public class DepartmentAndDateGroupedData
    {
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public int TotalEmployees { get; set; }
        public DateTime Date { get; set; }
        public List<AttendanceMovementRegisterReportCountDto> Employees { get; set; } = new List<AttendanceMovementRegisterReportCountDto>();

    }
}

