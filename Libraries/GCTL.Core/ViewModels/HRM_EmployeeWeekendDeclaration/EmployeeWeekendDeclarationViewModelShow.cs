using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HRM_EmployeeWeekendDeclaration
{
    public class EmployeeWeekendDeclarationViewModelShow
    {
        public decimal TC { get; set; }
        public string ID { get; set; }
        public string EmpID { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string WeekendDate { get; set; }
        public string Day { get; set; }
        public string Remarks { get; set; }
        public string EntryUser { get; set; }
    }
}
