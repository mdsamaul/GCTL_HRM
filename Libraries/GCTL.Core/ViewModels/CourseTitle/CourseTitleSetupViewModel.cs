using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.CourseTitle
{
    public class CourseTitleSetupViewModel: BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string CourseCode { get; set; }
        [Required(ErrorMessage = "CourseName is required")]
        public string CourseName { get; set; }
        public string ShortName { get; set; }
    }
}
