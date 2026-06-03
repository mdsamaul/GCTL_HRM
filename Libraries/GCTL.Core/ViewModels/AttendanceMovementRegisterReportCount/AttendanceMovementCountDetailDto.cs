using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.AttendanceMovementRegisterReportCount
{
    public class AttendanceMovementCountDetailDto
    {
        public string CompanyName { get; set; }
        public string CompanyCode { get; set; }
        public string Address1 { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationCode { get; set; }
        public string DesignationName { get; set; }

        public string EmployeeID { get; set; }
        public string FullName { get; set; }

        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }

        public string MachineId { get; set; }
        public string FingerPrintID { get; set; }

        public string? Longitude { get; set; }
        public string? Latitude { get; set; }

        public List<AttendanceMovementCountRowDto> Movements { get; set; } = new();
    }
    public class AttendanceMovementCountRowDto
    {
        public DateTime Time { get; set; }
        public string MachineId { get; set; }
        public string LocationUrl { get; set; }
    }

    public class EmployeeMovementRequestDto
    {
        public string EmployeeId { get; set; }
        public DateTime Date { get; set; }
    }
}
