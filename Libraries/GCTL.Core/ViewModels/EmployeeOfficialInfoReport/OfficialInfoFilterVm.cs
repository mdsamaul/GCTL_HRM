namespace GCTL.Core.ViewModels.EmployeeOfficialInfoReport
{
    public class OfficialInfoFilterVm
    {
        public List<string> CompanyCodes { get; set; } = new();
        public List<string> BranchCodes { get; set; } = new();
        public List<string> DepartmentCodes { get; set; } = new();
        public List<string> DesignationCodes { get; set; } = new();
        public List<string> EmployeeCodes { get; set; } = new();

        public string EmployeeTypeCode { get; set; }
        public string EmploymentNatureId { get; set; }
        public string NationalId { get; set; }
        public string TinNo { get; set; }
        public string PassportNo { get; set; }
        public string DrivingLicense { get; set; }
        public string IsExpatriate { get; set; }
        public string ImmediateSup { get; set; }
        public string HOD { get; set; }
        public string ShiftCode { get; set; }
        public string EmployeeStatus { get; set; }
    }

}
