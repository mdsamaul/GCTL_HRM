using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;

namespace GCTL.Service.Specialities
{
    public interface ISpecialityService
    {
        List<HmsSpeciality> GetSpecialities();
        HmsSpeciality GetSpeciality(string code); 
        bool DeleteSpeciality(string id);    
        HmsSpeciality SaveSpeciality(HmsSpeciality entity);
        bool IsSpecialityExistByCode(string code);
        bool IsSpecialityExist(string name);
        bool IsSpecialityExist(string name, string typeCode);
        IEnumerable<CommonSelectModel> SpecialitySelection();
        bool SavePermission(string accessCode);
        bool UpdatePermission(string accessCode);
        bool DeletePermission(string accessCode);
    }
}