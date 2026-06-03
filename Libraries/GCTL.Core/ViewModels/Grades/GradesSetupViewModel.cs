using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.Grades
{
    public class GradesSetupViewModel: BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string GradeCode { get; set; }
        [Required(ErrorMessage = "GradeName is required")]
        public string GradeName { get; set; }
        public string GradeShortName { get; set; }
        public decimal? FromGrossSalary { get; set; }
        public decimal? ToGrossSalary { get; set; }
        [Required]
        public string GradeTypeId { get; set; }
        public string GradeTypeName { get; set; }
    }
}
