using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HRM_EmployeeWeekendDeclaration
{
    public class HRM_EmployeeWeekendDeclarationDto  : BaseViewModel
    {
        public decimal TC { get; set; }
        public string EWDId { get; set; }
        public List<string> WeekendDates { get; set; }=new List<string>();
        public List<string> WeekendEmployeeIds { get; set; } =new List<string>();
        public string Remark { get; set; }
        public List<string> ExcelRemark { get; set; } = new List<string>();      
        public string? CompanyCode { get; set; }
    }
}


