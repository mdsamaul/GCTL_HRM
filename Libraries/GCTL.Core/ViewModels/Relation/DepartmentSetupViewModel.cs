using System.ComponentModel.DataAnnotations;

namespace GCTL.Core.ViewModels.Relation
{
    public class RelationSetupViewModel : BaseViewModel
    {
        public string RelationCode { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Relation")]
        public string Relation { get; set; }

        
    }
}
