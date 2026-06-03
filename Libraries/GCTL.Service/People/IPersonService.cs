using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;

namespace GCTL.Service.People
{
    public interface IPersonService
    {
        List<HmsLtrvPerson> GetPersons();
        HmsLtrvPerson GetPerson(string code); 
        bool DeletePerson(string id);    
        HmsLtrvPerson SavePerson(HmsLtrvPerson entity);
        bool IsPersonExistByCode(string code);
        bool IsPersonExist(string name);
        bool IsPersonExist(string name, string typeCode);
        IEnumerable<CommonSelectModel> PersonSelection();
    }
}