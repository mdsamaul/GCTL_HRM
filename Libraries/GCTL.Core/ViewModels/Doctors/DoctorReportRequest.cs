using GCTL.Core.Reports;

namespace GCTL.Core.ViewModels.Doctors
{
    public class DoctorReportRequest : ApplicationReportRequest
    {
        public string DoctorTypeCode { get; set; }
        public string DepartmentCode { get; set; }
        public string SpecialityCode { get; set; }
        public string QualificationCode { get; set; }
        public bool IsReport { get; set; }
    }
}
