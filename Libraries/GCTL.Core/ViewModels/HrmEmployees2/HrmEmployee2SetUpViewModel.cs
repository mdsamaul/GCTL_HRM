using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.ViewModels.HrmEmployeePhotos;
using Microsoft.AspNetCore.Http;

namespace GCTL.Core.ViewModels.HrmEmployees2
{
    public class HrmEmployee2SetUpViewModel : BaseViewModel
    {
        public decimal AutoId { get; set; }

        // [Required, ReadOnly(true)]
        public string EmployeeId { get; set; }

        [Required(ErrorMessage = "Please Enter First Name")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfBirthCertificate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime? DateOfBirthOrginal { get; set; }
        public string BirthCertificateNo { get; set; }
        public string PlaceOfBirth { get; set; }
        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Gender")]
        public string SexCode { get; set; }
        public string SexName { get; set; }

        [Display(Name = "Religion")]
        public string ReligionCode { get; set; }
        public string Religion { get; set; }

        [Display(Name = "Blood Group")]
        public string BloodGroupCode { get; set; }
        public string BloodGroup { get; set; }

        [Display(Name = "Marital Status")]
        public string MaritalStatusCode { get; set; }
        public string MaritalStatus { get; set; }
        public string Nationality { get; set; }
        public string NationalityCode { get; set; }
        public string NationalIdno { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? MarriageDate { get; set; }
        public string NoOfSon { get; set; }
        public string NoOfDaughters { get; set; }
        public string CardNo { get; set; }
       
        [Required(ErrorMessage ="Please Enter Company")]
        public string CompanyCode { get; set; }
       

        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string PersonalEmail { get; set; }
        public string Telephone { get; set; }
        public string TinNo { get; set; }
        [DataType(DataType.MultilineText)]
        public string ExtraCurriActivities { get; set; }
        public string Remarks { get; set; }
        public string FirstNameBangla { get; set; }
        public string LastNameBangla { get; set; }
        [Required(ErrorMessage ="Please Enter Branch"),Display(Name ="Branch")]
        public string BranchCode { get; set; }
        public  string Branch { get; set; }
        public  string Company { get; set; }
        public string FatherOccupation { get; set; }
        public string MotherOccupation { get; set; }
        public string Spouse { get; set; }
        public bool IsClearPhoto { get; set; }
        public bool IsClearSignature { get; set; }
        public IFormFile Photo { get; set; }
        public IFormFile Signature { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;
        public string? DigitalSignatureUrl { get; set; } = string.Empty;
        public bool IsEditMode { get; set; }
        public IList<HrmEmployeePhotoSetupViewModel> ? HrmEmployeePhotoSetupVM { get; set; } = new List<HrmEmployeePhotoSetupViewModel>();

    }
}
