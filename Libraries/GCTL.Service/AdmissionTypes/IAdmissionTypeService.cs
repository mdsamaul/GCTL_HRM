using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;

namespace GCTL.Service.AdmissionTypes
{
    public interface IAdmissionTypeService
    {
        List<HmsAdmissionType> GetAdmissionTypes();
        HmsAdmissionType GetAdmissionType(string code); 
        bool DeleteAdmissionType(string id);    
        HmsAdmissionType SaveAdmissionType(HmsAdmissionType entity);
        bool IsAdmissionTypeExistByCode(string code);
        bool IsAdmissionTypeExist(string name);
        bool IsAdmissionTypeExist(string name, string typeCode);
        IEnumerable<CommonSelectModel> AdmissionTypeSelection();

        bool SavePermission(string accessCode);
        bool UpdatePermission(string accessCode);
        bool DeletePermission(string accessCode);
    }
}