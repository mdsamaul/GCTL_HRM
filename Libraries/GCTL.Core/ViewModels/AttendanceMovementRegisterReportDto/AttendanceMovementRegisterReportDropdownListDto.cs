using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.AttendanceMovementRegisterReportDto
{
    public class AttendanceMovementRegisterReportDropdownListDto
    {
        public List<IdNamePair> Companies { get; set; }
        public List<IdNamePair> Branches { get; set; }
        public List<IdNamePair> Departments { get; set; }
        public List<IdNamePair> Designations { get; set; }
        public List<IdNamePair> Employees { get; set; }
        public List<IdNamePair> PayHeads { get; set; }
        public List<IdNamePair> YearFrom { get; set; }
        public List<IdNamePair> YearTo { get; set; }
    }

    public class IdNamePair
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}

