using System.ComponentModel.DataAnnotations;

namespace GCTL.Core.ViewModels.Doctors
{
    public class DoctorFilterViewModel
    {
        [Display(Name = "Doctor Type")]
        public string DoctorTypeCode { get; set; }

        [Display(Name = "Department")]
        public string DepartmentCode { get; set; }

        [Display(Name = "Speciality")]
        public string SpecialityCode { get; set; }


        [Display(Name = "Qualification")]
        public string QualificationCode { get; set; }
    }
}
