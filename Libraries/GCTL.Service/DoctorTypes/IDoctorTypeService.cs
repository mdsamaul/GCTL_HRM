using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;

namespace GCTL.Service.DoctorTypes
{
    public interface IDoctorTypeService
    {
        List<HmsDoctorType> GetDoctorTypes();
        HmsDoctorType GetDoctorType(string code);
        bool DeleteDoctorType(string id);
        HmsDoctorType SaveDoctorType(HmsDoctorType entity);
        bool IsDoctorTypeExistByCode(string code);
        bool IsDoctorTypeExist(string name);
        bool IsDoctorTypeExist(string name, string typeCode);
        IEnumerable<CommonSelectModel> DoctorTypeSelection();
        bool SavePermission(string accessCode);
        bool UpdatePermission(string accessCode);
        bool DeletePermission(string accessCode);
    }
}