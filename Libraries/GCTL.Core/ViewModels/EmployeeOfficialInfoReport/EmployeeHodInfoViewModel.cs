using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.EmployeeOfficialInfoReport
{
    public class EmployeeHodInfoViewModel
    {
        public string EmployeeId { get; set; }
        public string EmpName { get; set; }
        public string HOD { get; set; }  
        public string HODName { get; set; }
        public string ReportingTo { get; set; }
    }
}
