using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.EmployeeOfficialInfoReport
{
    public class OfficialInfoDropdownResponse
    {
        public List<DropdownDto> Companies { get; set; } = new();
        public List<DropdownDto> Branches { get; set; } = new();
        public List<DropdownDto> Divisions { get; set; } = new();
        public List<DropdownDto> Departments { get; set; } = new();
        public List<DropdownDto> Designations { get; set; } = new();
        public List<DropdownDto> Employees { get; set; } = new();
        public List<DropdownDto> EmploymentNatures { get; set; } = new();
        public List<DropdownDto> EmployeeTypes { get; set; } = new();
        public List<DropdownDto> ImmediateSupervisors { get; set; } = new();
        public List<DropdownDto> HODs { get; set; } = new();
        public List<DropdownDto> Shifts { get; set; } = new();
        public List<DropdownDto> Expatriates { get; set; } = new();
        public List<DropdownDto> ActivityStatuses { get; set; } = new();
        public List<DropdownDto> NationalIds { get; set; } = new();
        public List<DropdownDto> TinNumbers { get; set; } = new();
        public List<DropdownDto> Passports { get; set; } = new();
        public List<DropdownDto> DrivingLicenses { get; set; } = new();
    }

}
