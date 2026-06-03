using System.ComponentModel.DataAnnotations;

namespace GCTL.Core.ViewModels.Dosages
{
    public class DosageSetupViewModel : BaseViewModel
    {
        public string MedicineDosageId { get; set; }
        public string MedicineDosageCode { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Dosage")]
        public string MedicineDosageName { get; set; }
    }
}
