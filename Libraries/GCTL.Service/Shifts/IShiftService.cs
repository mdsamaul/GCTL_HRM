using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;

namespace GCTL.Service.Shifts
{
    public interface IShiftService
    {
        List<HmsShift> GetShifts();
        HmsShift GetShift(string code); 
        bool DeleteShift(string id);    
        HmsShift SaveShift(HmsShift entity);
        bool IsShiftExistByCode(string code);
        bool IsShiftExist(string name);
        bool IsShiftExist(string name, string typeCode);
        IEnumerable<CommonSelectModel> ShiftSelection();
        bool SavePermission(string accessCode);
        bool UpdatePermission(string accessCode);
        bool DeletePermission(string accessCode);
    }
}