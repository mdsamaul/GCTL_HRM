using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace GCTL.Core.ViewModels.Doctors
{
    public class DoctorSetupViewModel : BaseViewModel
    {
        public DoctorSetupViewModel()
        {
            Appointments = new List<DoctorAppointment>();
        }

        [Required()]
        [Display(Name = "Doctor ID")]
        public string DoctorCode { get; set; }
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Doctor Type")]
        public string DoctorTypeCode { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Doctor Name")]
        public string DoctorName { get; set; }

        [EmailAddress(ErrorMessage = "Please enter valid email")]
        public string Email { get; set; }

        [Display(Name = "Date of Birth")]
        public string DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string CompanyCode { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name ="Gender")]
        public string SexCode { get; set; }

        [Display(Name = "Religion")]
        public string ReligionCode { get; set; }

        [Display(Name = "Blood Group")]
        public string BloodGroupCode { get; set; }

        [Display(Name = "National Id")]
        public string NationalIdNo { get; set; }

        [Display(Name = "Department")]
        public string DepartmentCode { get; set; }

        [Display(Name = "Qualification")]
        public string QualificationCode { get; set; }

        [Display(Name = "Joining Date")]
        public string JoiningDate { get; set; }

        [Display(Name = "Present Address")]
        public string PresentAddress { get; set; }

        [Display(Name = "Permanent Address")]
        public string PermanentAddress { get; set; }

        [Display(Name = "Specialist")]
        public string SpecialityCode { get; set; }


        [Range(0, double.MaxValue, ErrorMessage = "Invalid Number")]
        public string Salary { get; set; }
        public TimeOnly VisitingTime { get; set; }       

        [Display(Name = "Designation")]
        public string DesignationCode { get; set; }

        [Display(Name = "Shift")]
        public string ShiftCode { get; set; }
        public string PhotoUrl { get; set; }
        public string DigitalSignatureUrl { get; set; }
        public string ActivityStatus { get; set; }


        [Display(Name = "Appointment Days")]
        public string AppointmentDays { get; set; }


        [Display(Name = "Visiting Time From")]
        public string VisitingTimeFrom { get; set; }

        [Display(Name = "Visiting Time To")]
        public string VisitingTimeTo { get; set; }


        [Display(Name = "Visiting Fee")]


        [Range(0, double.MaxValue, ErrorMessage = "Invalid Number")]
        public string VisitingFee { get; set; }

        [Display(Name = "Report Showing Fee")]

        [Range(0, double.MaxValue, ErrorMessage = "Invalid Number")]
        public string ReportShowingFee { get; set; }

        [Display(Name = "2nd Visiting Fee")]

        [Range(0, double.MaxValue, ErrorMessage = "Invalid Number")]
        public string SecondVisitingFee { get; set; }

        [Display(Name = "Duration Within Day(s)")]
        public int? DurationWithin { get; set; }


        public bool IsClearPhoto { get; set; }
        public bool IsClearSignature { get; set; }

        public IFormFile Photo { get; set; }
        public IFormFile Signature { get; set; }
        public string BanglaDoctorName { get; set; }
        public string WorkingPlaceCode { get; set; }
        public string BMDCRegNo { get; set; }
        
        public List<DoctorAppointment> Appointments { get; set; }
    }
}
