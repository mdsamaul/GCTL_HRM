using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;

namespace GCTL.Service.Surnames
{
    public interface ISurnameService
    {
        List<HrmDefSurName> GetSurnames();
        HrmDefSurName GetSurname(string code);
        HrmDefSurName GetSurname(decimal id);
        bool DeleteSurname(string id);    
        HrmDefSurName SaveSurname(HrmDefSurName entity);
        bool IsSurnameExistByCode(string code);
        bool IsSurnameExist(string name);
        bool IsSurnameExist(string name, string typeCode);
        bool IsSurnameExist(string name, decimal autoId);
        IEnumerable<CommonSelectModel> SurnameSelection();
        bool SavePermission(string accessCode);
        bool UpdatePermission(string accessCode);
        bool DeletePermission(string accessCode);
    }
}