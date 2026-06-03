using GCTL.Core.DataTables;
using GCTL.Data.Models;

namespace GCTL.Service.UnitTypes
{
    public interface IUnitTypeService
    {
        List<RmgProdDefUnitType> GetUnitTypes();
        RmgProdDefUnitType GetUnitType(string code); 
        bool DeleteUnitType(string id);    
        RmgProdDefUnitType SaveUnitType(RmgProdDefUnitType entity);
        bool IsUnitTypeExistByCode(string code);
        bool IsUnitTypeExist(string name);
        bool IsUnitTypeExist(string name, string typeCode);
        PagedList<RmgProdDefUnitType> GetPagedUnitTypes(DataTablesOptions options);
    }
}