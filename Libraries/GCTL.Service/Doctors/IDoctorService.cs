using GCTL.Core.Messaging;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.Doctors;
using GCTL.Data.Models;
using System.Collections;

namespace GCTL.Service.Doctors
{
    public interface IDoctorService
    {
        List<DoctorViewModel> GetDoctors(string doctorTypeCode, string departmentCode, string specialityCode, string qualificationCode);
        HmsDoctor GetDoctor(string id);
        DoctorViewModel GetDoctorByCode(string doctorCode);
        ApplicationResponse DeleteDoctor(string id);
        HmsDoctor SaveDoctor(HmsDoctor entity);
        bool IsDoctorExistByCode(string id);
        bool IsDoctorExist(string name);
        bool IsDoctorExist(string name, string doctorCode);
        IEnumerable GetDoctorSelections();
        IEnumerable<CommonSelectModel> LabTestDoctorSelection();
        IEnumerable GetDoctorSelections(string departmentCode);
        public List<DoctorReportModel> GetDoctorReports(DoctorReportRequest request);

        bool SavePermission(string accessCode);
        bool UpdatePermission(string accessCode);
        bool DeletePermission(string accessCode);
    }
}