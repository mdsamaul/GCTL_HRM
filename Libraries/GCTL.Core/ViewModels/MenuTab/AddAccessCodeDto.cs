using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.MenuTab
{
    public class AddAccessCodeDto
    {
        [Required(ErrorMessage = "Access Code Id is required.")]
        public string AccessCodeId { get; set; }
        [Required(ErrorMessage = "Access Code Name is required.")]
        public string AccessCodeName { get; set; }
    }
}
