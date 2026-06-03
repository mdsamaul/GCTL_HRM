using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HRM_EmployeeWeekendDeclaration
{
    public class HRM_EmployeeWeekendDeclarationSetupViewModel : BaseViewModel
    {
        public decimal TC { get; set; }
        public string EWDId { get; set; }
        public string EmployeeID { get; set; }
        public DateTime Date { get; set; }
        public string Remark { get; set; }
        public string LUser { get; set; }
        public DateTime? LDate { get; set; }
        public string LIP { get; set; }
        public string LMAC { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string CompanyCode { get; set; }
    }
}
