namespace GCTL.Core.ViewModels.ExcessTDSForLastIncomeYear
{
    public class ExcessTDSForLastIncomeYearFilterListDto
    {
        public List<ExcessTDSForLastIncomeYearFilterResultDto> Companies { get; set; }
        public List<ExcessTDSForLastIncomeYearFilterResultDto> Branches { get; set; }
        public List<ExcessTDSForLastIncomeYearFilterResultDto> Divisions { get; set; }
        public List<ExcessTDSForLastIncomeYearFilterResultDto> Departments { get; set; }
        public List<ExcessTDSForLastIncomeYearFilterResultDto> Designations { get; set; }
        public List<ExcessTDSForLastIncomeYearFilterResultDto> Employees { get; set; }
        public List<ExcessTDSForLastIncomeYearFilterResultDto> ActivityStatuses { get; set; }
        public List<ExcessTDSForLastIncomeYearFilterResultDto> EmployeeTypes { get; set; }
        public List<ExcessTDSForLastIncomeYearFilterResultDto> EmploymentNature { get; set; }
        public DateTime Date { get; set; }
    }
}
