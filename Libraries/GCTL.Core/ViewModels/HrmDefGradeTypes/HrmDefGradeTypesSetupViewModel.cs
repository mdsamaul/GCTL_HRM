using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HrmDefGradeTypes
{
    public class HrmDefGradeTypesSetupViewModel:BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string GradeTypeId { get; set; }
        [Required]
        public string GradeType { get; set; }
        public string ShortName { get; set; }
        public string CompanyCode { get; set; }
        public string BanglaGradeType { get; set; }
    }
}
