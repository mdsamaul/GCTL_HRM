// GCTL.Core.ViewModels.SalaryInformationReport/SalaryInformationReportYearlyBonusDto.cs

namespace GCTL.Core.ViewModels.SalaryInformationReport
{
    public class SalaryInformationReportYearlyBonusDto
    {
        public int SL { get; set; }
        public string IdNo { get; set; }
        public string PayId { get; set; }
        public string NameOfTheEmployee { get; set; }
        public string Status { get; set; }
        public string Department { get; set; }
        public string DateOfHire { get; set; }
        public string Dot { get; set; }
        public string BankAccountNo { get; set; }
        public decimal? Salary { get; set; }
        public decimal? YearlyBonus { get; set; }
        public string Note { get; set; }
    }
}