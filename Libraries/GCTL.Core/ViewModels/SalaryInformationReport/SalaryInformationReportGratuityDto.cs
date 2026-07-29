// GCTL.Core.ViewModels.SalaryInformationReport/SalaryInformationReportGratuityDto.cs

namespace GCTL.Core.ViewModels.SalaryInformationReport
{
    public class SalaryInformationReportGratuityDto
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
        public int? Tenure { get; set; }
        public string Gratuity { get; set; }
        public string Note { get; set; }
    }
}