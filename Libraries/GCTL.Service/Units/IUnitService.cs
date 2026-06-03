using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;

namespace GCTL.Service.Units
{
    public interface IUnitService
    {
        List<HmsLtrvUnit> GetUnits();
        HmsLtrvUnit GetUnit(string code); 
        bool DeleteUnit(string id);    
        HmsLtrvUnit SaveUnit(HmsLtrvUnit entity);
        bool IsUnitExistByCode(string code);
        bool IsUnitExist(string name);
        bool IsUnitExist(string name, string typeCode);
        IEnumerable<CommonSelectModel> UnitSelection();
    }
}