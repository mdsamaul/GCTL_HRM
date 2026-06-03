using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HrmDefEmpTypes
{
    public class HrmDefEmpTypeSetupViewModel:BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string EmpTypeCode { get; set; }
        public string EmpTypeName { get; set; }
        public string EmpTypeShortName { get; set; }
        public string CompanyCode { get; set; }
        [Display(Name = "Address (বাংলা)")]
        public string BanglaEmployeeType { get; set; }
    }
}
