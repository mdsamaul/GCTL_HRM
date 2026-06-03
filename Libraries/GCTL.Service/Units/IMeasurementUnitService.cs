using GCTL.Core.Enums;
using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;

namespace GCTL.Service.Units
{
    public interface IMeasurementUnitService
    {
        List<CoreMeasurementUnit> GetUnits();
        List<CoreMeasurementUnit> GetUnits(UnitType unitType);
        CoreMeasurementUnit GetUnit(string code); 
        bool DeleteUnit(string id);    
        CoreMeasurementUnit SaveUnit(CoreMeasurementUnit entity);
        bool IsUnitExistByCode(string code);
        bool IsUnitExist(string name);
        bool IsUnitExist(string name, string typeCode);
        IEnumerable<CommonSelectModel> UnitSelection();
    }
}