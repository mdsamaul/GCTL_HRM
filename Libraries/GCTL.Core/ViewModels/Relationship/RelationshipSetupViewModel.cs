using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.Relationship
{
    public class RelationshipSetupViewModel : BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string RelationshipCode { get; set; }
        [Required(ErrorMessage = "Relationship is required")]
        public string Relationship { get; set; }
        public string ShortName { get; set; }
    }
}
