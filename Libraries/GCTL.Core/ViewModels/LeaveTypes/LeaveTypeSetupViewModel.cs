
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;


namespace GCTL.Core.ViewModels.LeaveTypes
{
    public class LeaveTypeSetupViewModel:BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string LeaveTypeCode { get; set; }
     
        [Required(ErrorMessage ="Name is required"),Display(Name = "Please Enter Name")]
        public string Name { get; set; }
      
        public string?  ShortName { get; set; }

        public string? RulePolicy { get; set; }

        public decimal? NoOfDay { get; set; }

        public decimal? For { get; set; } 
      
        public string ? Ymwd { get; set; }

        [Required(ErrorMessage = "Wef is required"), Display(Name = "Please enter WEF")]

        //public string  Wef { get; set; }
     
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Wef { get; set; }


    }
}
