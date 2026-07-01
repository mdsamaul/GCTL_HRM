namespace GCTL.Core.ViewModels.FullEmployeeDetailsGetById
{
    /// <summary>
    /// Merged ViewModel — Doc1 + Doc2 combined.
    /// Used by FullEmployeeDetailsGetByid SP.
    /// </summary>
    public class FullEmployeeDetailsGetByIdViewModel : BaseViewModel
    {
        // ══════════════════════════════════════════
        // ── Company ──
        // ══════════════════════════════════════════
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string Address1 { get; set; }
        public string CompanyCountry { get; set; }   // Doc2
        public string CompanyAddress { get; set; }   // Doc2: Intern Offer (013)
        public string LetterRefNo { get; set; }   // Doc2

        // ══════════════════════════════════════════
        // ── Personal Info ──
        // ══════════════════════════════════════════
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
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

        // ══════════════════════════════════════════
        // ── Personal Details (Code + Name) ──
        // ══════════════════════════════════════════
        public string GenderCode { get; set; }
        public string Gender { get; set; }

        public string MaritalStatusCode { get; set; }
        public string MaritalStatus { get; set; }

        public string BloodGroupCode { get; set; }
        public string BloodGroup { get; set; }

        public string NationalityCode { get; set; }
        public string Nationality { get; set; }

        public string ReligionCode { get; set; }
        public string Religion { get; set; }

        // ══════════════════════════════════════════
        // ── Additional Info ──
        // ══════════════════════════════════════════
        public string PassportNo { get; set; }
        public string PassportExpiryDate { get; set; }
        public string DrivingLicenseNo { get; set; }
        public string DrivingLicenseExpiryDate { get; set; }
        public string WorkPermitNo { get; set; }
        public string WorkPermitType { get; set; }
        public string WpEffectiveDate { get; set; }
        public string WpExpireDate { get; set; }

        // ══════════════════════════════════════════
        // ── Official Info (Code + Name) ──
        // ══════════════════════════════════════════
        public string BranchCode { get; set; }
        public string BranchName { get; set; }

        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }

        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }

        public string DesignationCode { get; set; }
        public string DesignationName { get; set; }

        public string EmployeeTypeCode { get; set; }
        public string EmployeeType { get; set; }

        public string EmploymentNatureId { get; set; }
        public string EmploymentNature { get; set; }

        public string GradeCode { get; set; }
        public string GradeNo { get; set; }

        public decimal GrossSalary { get; set; }
        public decimal JoiningSalary { get; set; }

        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }

        public string PaymentPeriodId { get; set; }
        public string PaymentPeriodName { get; set; }

        public string DisbursementMethodId { get; set; }
        public string DisbursementMethodName { get; set; }

        public string ShiftCode { get; set; }
        public string ShiftName { get; set; }

        public string ModeOfPayment { get; set; }
        public decimal ModeOfPaymentInBankPercentage { get; set; }

        public string? ClientIdRaw { get; set; }
        public List<string>? ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;

        // ══════════════════════════════════════════
        // ── Supervisor & HOD (Code + Name) ──
        // ══════════════════════════════════════════
        public string ImmediateSupervisorID { get; set; }
        public string ImmediateSupervisor { get; set; }
        public string SuporVisorName1 { get; set; }

        public string HeadOfDepartmentID { get; set; }
        public string HeadOfDepartment { get; set; }
        public string HodName1 { get; set; }

        // ══════════════════════════════════════════
        // ── Section / Line ──
        // ══════════════════════════════════════════
        public string SectionCode { get; set; }
        public string SectionName { get; set; }
        public string LineCode { get; set; }
        public string LineName { get; set; }

        // ══════════════════════════════════════════
        // ── Scale / Step / Skill ──
        // ══════════════════════════════════════════
        public string StepNoId { get; set; }
        public string StepNoName { get; set; }
        public string TecnicalSkillTypeId { get; set; }
        public string TecnicalSkillTypeName { get; set; }
        public string SalaryScaleId { get; set; }
        public string SalaryScaleName { get; set; }

        // ══════════════════════════════════════════
        // ── Employee Status ──
        // ══════════════════════════════════════════
        public string EmployeeStatusId { get; set; }
        public string EmployeeStatus { get; set; }

        // ══════════════════════════════════════════
        // ── Official Contact & Dates ──
        // ══════════════════════════════════════════
        public string OfficialPhone { get; set; }
        public string OfficialEmail { get; set; }
        public string AppointmentLetterNo { get; set; }
        public string AppointmentLetterDate { get; set; }
        public string JoiningDate { get; set; }
        public string ProbationPeriodType { get; set; }
        public string ProbationPeriod { get; set; }
        public string ProbationEffectDate { get; set; }
        public string ProbationEndDate { get; set; }
        public string ConfirmationRefNo { get; set; }
        public string ConfirmationDate { get; set; }
        public string ContractEndDate { get; set; }
        public string ServiceLength { get; set; }
        public string LeavingDate { get; set; }

        // ══════════════════════════════════════════
        // ── Expatriate Info ──
        // ══════════════════════════════════════════
        public string IsExpatriate { get; set; }
        public decimal ExpatriateBasicSalary { get; set; }
        public decimal ExpatriateHouseRent { get; set; }
        public decimal ExpatriateConveyance { get; set; }
        public decimal ExpatriateMedical { get; set; }
        public decimal Lfa { get; set; }
        public decimal MobileAllowance { get; set; }

        // ══════════════════════════════════════════
        // ── Eligibility Flags ──
        // ══════════════════════════════════════════
        public string IsLunchBilEligible { get; set; }
        public string IsOverTimeEligible { get; set; }
        public string IsExtraDutyEligible { get; set; }
        public string IsGovtHolidayEligible { get; set; }
        public string IsAttendanceBonusEligible { get; set; }

        // ══════════════════════════════════════════
        // ── Pay / Attendance Reference ──
        // ══════════════════════════════════════════
        public string PayId { get; set; }
        public string AttendenceId { get; set; }

        // ══════════════════════════════════════════
        // ── Company Session / Misc ──
        // ══════════════════════════════════════════
        public string CompanyCodeSession { get; set; }
        public string Address { get; set; }
        public string FullName { get; set; }

        // ══════════════════════════════════════════
        // ── Present Address (Full Detail) ──  Doc2
        // ══════════════════════════════════════════
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

        // ══════════════════════════════════════════
        // ── Permanent Address (Full Detail) ──  Doc2
        // ══════════════════════════════════════════
        public string ParmanentAddress { get; set; }
        public string ParmanentPostOffice { get; set; }
        public string ParmanentThana { get; set; }
        public string ParmanentPostCode { get; set; }
        public string ParmanentDistrict { get; set; }
        public string ParmanentPhone { get; set; }
        public string ParmanentAddressBangla { get; set; }

        // ══════════════════════════════════════════
        // ── Emergency Contact 1 (Code + Name) ──
        // ══════════════════════════════════════════
        public string EmContactName1 { get; set; }
        public string EmContactRelation1Code { get; set; }
        public string EmContactRelation1 { get; set; }
        public string EmContactMobile1 { get; set; }
        public string EmContactPhone1 { get; set; }
        public string EmContactEmail { get; set; }

        // ══════════════════════════════════════════
        // ── Emergency Contact 2 (Code + Name) ──
        // ══════════════════════════════════════════
        public string EmContactName2 { get; set; }
        public string EmContactRelation2Code { get; set; }
        public string EmContactRelation2 { get; set; }
        public string EmContactMobile2 { get; set; }
        public string EmContactPhone2 { get; set; }
        public string EmContactEmai2 { get; set; }

        // ══════════════════════════════════════════
        // ── Highest Education ──  Doc2
        // ══════════════════════════════════════════
        public string HighestExamTitle { get; set; }
        public string HighestInstitute { get; set; }
        public string HighestBoard { get; set; }
        public string HighestGroup { get; set; }
        public string HighestResult { get; set; }
        public string HighestCgpa { get; set; }
        public string HighestScaleOf { get; set; }
        public string HighestYearOfPassing { get; set; }
        public string HighestDuration { get; set; }
        public string HighestDurationType { get; set; }
        public string HighestAchievement { get; set; }

        // ══════════════════════════════════════════
        // ── Internship Offer Letter Fields (013) ── Doc2
        // ══════════════════════════════════════════
        public string? InternUniversityName { get; set; }
        public string? InternAddress { get; set; }
        public string? InternStartDate { get; set; }
        public string? InternDurationMonths { get; set; }

        // ══════════════════════════════════════════
        // ── Photo & Signature ──
        // ══════════════════════════════════════════
        public byte[] Photo { get; set; }
        public byte[] DigitalSignature { get; set; }

        // ══════════════════════════════════════════
        // ── Computed Helpers ──
        // ══════════════════════════════════════════
        public string PhotoBase64 =>
            Photo is { Length: > 0 }
                ? $"data:image/jpeg;base64,{Convert.ToBase64String(Photo)}"
                : null;

        public string SignatureBase64 =>
            DigitalSignature is { Length: > 0 }
                ? $"data:image/jpeg;base64,{Convert.ToBase64String(DigitalSignature)}"
                : null;
    }
}