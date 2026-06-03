using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HrmEmployeeOfficialInfo
{
    public class HrmEmployeeOfficialInfoSetupViewModel : BaseViewModel
    {
        public decimal AutoId { get; set; }
        [Required]
        public string EmployeeId { get; set; }
        [Required]
        public string OfficialInfoCompanyCode { get; set; }
        public string OfficialInfoBranchCode { get; set; }
        public string DivisionCode { get; set; }
        [Required]
        public string DepartmentCode { get; set; }
        [Required]
        public string DesignationCode { get; set; }
        public string EmpTypeCode { get; set; }
        public string GradeCode { get; set; }
        public string EmploymentNatureId { get; set; }
        public decimal GrossSalary { get; set; }
        public string CurrencyCode { get; set; }
        public string PaymentPeriodId { get; set; }
        public string DisbursementMethodId { get; set; }
        public string ShiftCode { get; set; }
        public string EmployeeStatus { get; set; }
        public string ReportingTo { get; set; }
        public string Hod { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string AppointmentLetterNo { get; set; }
      //  [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? AppointmentLetterDate { get; set; }
        //public string JoiningDate { get; set; }
       // [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? JoiningDate { get; set; }
        public decimal JoiningSalary { get; set; }
        public string ProbationPeriodType { get; set; }
        public string ProbationPeriod { get; set; }
       // [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ConfirmeDate { get; set; }
        public string CompanyCodeSession { get; set; }
        public string StepNoId { get; set; }
        public string TecnicalSkillTypeId { get; set; }
        public string SalaryScaleId { get; set; }
       // [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ContractEndDate { get; set; }
        public string SectionCode { get; set; }
        public string LineCode { get; set; }
        public string AttendenceId { get; set; }
        public string IsExpatriate { get; set; }
        public decimal? ExpatriateBasicSalary { get; set; }
        public decimal? ExpatriateHouseRent { get; set; }
        public decimal? ExpatriateConveyance { get; set; }
        public decimal? ExpatriateMedical { get; set; }
        public decimal? Lfa { get; set; }
        public decimal? MobileAllowance { get; set; }
        public string ConfirmationRefNo { get; set; }
        public DateTime? ProbationEffectDate { get; set; }
        public decimal? ModeOfPaymentInBankPercentage { get; set; }
        public string IsLunchBilEligible { get; set; }
        public string IsOverTimeEligible { get; set; }
        public string IsExtraDutyEligible { get; set; }
        public string IsGovtHolidayEligible { get; set; }
        public string IsAttendanceBonusEligible { get; set; }
        public string PayId { get; set; }


        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public string DivisionName { get; set; }
        public string EmployeeType { get; set; }
        public string EmployeeNature { get; set; }
        public string ShiftName { get; set; }
        public string CompanyCode { get; set; }
        
        
        public string SuporVisorName1 { get; set; }
        public string HodName1 { get; set; }


    }
}
