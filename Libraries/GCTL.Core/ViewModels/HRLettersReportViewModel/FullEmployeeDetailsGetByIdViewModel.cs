namespace GCTL.Core.ViewModels.HRLettersReportViewModel
{
    public class FullEmployeeDetailsGetByIdViewModel
    {
        // ── Company ──
        public string CompanyName { get; set; }
        public string Address1 { get; set; }
        public string CompanyCountry { get; set; }
        public string LeavingDate { get; set; }
        public string LetterRefNo { get; set; }

        // ── Personal Info ──
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string FathersOccupation { get; set; }
        public string MothersOccupation { get; set; }
        public string DateOfBirthCertificate { get; set; }
        public string DateOfBirthOrginal { get; set; }
        public string PlaceOfBirth { get; set; }
        public string NationalIDNO { get; set; }
        public string BirthCertificateNo { get; set; }
        public string NoOfSon { get; set; }
        public string NoOfDaughters { get; set; }
        public string Telephone { get; set; }
        public string TINNo { get; set; }
        public string ExtraCurricularActivities { get; set; }
        public string PersonalEmail { get; set; }
        public string CardNo { get; set; }

        // ── Personal Details ──
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public string BloodGroup { get; set; }
        public string Nationality { get; set; }
        public string Religion { get; set; }

        // ── Additional Info ──
        public string PassportNo { get; set; }
        public string PassportExpiryDate { get; set; }
        public string DrivingLicenseNo { get; set; }
        public string DrivingLicenseExpiryDate { get; set; }
        public string WorkPermitNo { get; set; }
        public string WorkPermitType { get; set; }
        public string WpEffectiveDate { get; set; }
        public string WpExpireDate { get; set; }

        // ── Official Info ──
        public string BranchName { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public string EmployeeType { get; set; }
        public string EmploymentNature { get; set; }
        public string GradeNo { get; set; }
        public decimal GrossSalary { get; set; }
        public string ShiftName { get; set; }
        public string ModeOfPayment { get; set; }
        public string ImmediateSupervisor { get; set; }
        public string HeadOfDepartment { get; set; }
        public string OfficialPhone { get; set; }
        public string OfficialEmail { get; set; }
        public string AppointmentLetterDate { get; set; }
        public string JoiningDate { get; set; }
        public string ProbationPeriod { get; set; }
        public string ProbationEndDate { get; set; }
        public string ConfirmationDate { get; set; }
        public string ContractEndDate { get; set; }
        public string ServiceLength { get; set; }

        // ── Contact Info ──
        //public string PresentAddress { get; set; }
        //public string ParmanentAddress { get; set; }

        // ── Emergency Contact 1 ──
        public string EmContactName1 { get; set; }
        public string EmContactRelation1 { get; set; }
        public string EmContactMobile1 { get; set; }
        public string EmContactPhone1 { get; set; }
        public string EmContactEmail { get; set; }

        // ── Emergency Contact 2 ──
        public string EmContactName2 { get; set; }
        public string EmContactRelation2 { get; set; }
        public string EmContactMobile2 { get; set; }
        public string EmContactPhone2 { get; set; }
        public string EmContactEmai2 { get; set; }

        // ── Photo & Signature ──
        public byte[] Photo { get; set; }
        public byte[] DigitalSignature { get; set; }

        // ── Helper: Photo as Base64 ──
        public string PhotoBase64 =>
            Photo != null && Photo.Length > 0
                ? $"data:image/jpeg;base64,{Convert.ToBase64String(Photo)}"
                : null;

        public string SignatureBase64 =>
            DigitalSignature != null && DigitalSignature.Length > 0
                ? $"data:image/jpeg;base64,{Convert.ToBase64String(DigitalSignature)}"
                : null;

        // ── Highest Education ──
        public string HighestExamTitle { get; set; }       // e.g. "B.Sc. Engineering"
        public string HighestInstitute { get; set; }       // e.g. "BUET"
        public string HighestBoard { get; set; }           // e.g. "Dhaka Board"
        public string HighestGroup { get; set; }           // e.g. "Science"
        public string HighestResult { get; set; }          // e.g. "First Class"
        public string HighestCgpa { get; set; }            // e.g. "3.75"
        public string HighestScaleOf { get; set; }         // e.g. "4.00"
        public string HighestYearOfPassing { get; set; }   // e.g. "2019"
        public string HighestDuration { get; set; }        // e.g. "4"
        public string HighestDurationType { get; set; }    // e.g. "Year"
        public string HighestAchievement { get; set; }     // e.g. "Dean's List"

        // ── Internship Offer Letter fields (013) ──
        public string? InternUniversityName { get; set; }
        public string? InternAddress { get; set; }
        public string? InternStartDate { get; set; }
        public string? InternDurationMonths { get; set; }
        public string? CompanyAddress { get; set; }

        // ── Present Address ──
        public string PresentAddress { get; set; }
        public string PresentPostOffice { get; set; }
        public string PresentThana { get; set; }
        public string PresentPostCode { get; set; }
        public string PresentDistrict { get; set; }
        public string PresentMobile { get; set; }
        public string PresentPhone { get; set; }
        public string PresentFax { get; set; }
        public string PresentEmail { get; set; }
        public string PresentAddressBangla { get; set; }

        // ── Permanent Address ──
        public string ParmanentAddress { get; set; }
        public string ParmanentPostOffice { get; set; }
        public string ParmanentThana { get; set; }
        public string ParmanentPostCode { get; set; }
        public string ParmanentDistrict { get; set; }
        public string ParmanentPhone { get; set; }
        public string ParmanentAddressBangla { get; set; }
    }
}