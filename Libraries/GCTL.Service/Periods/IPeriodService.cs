using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;

namespace GCTL.Service.Periods
{
    public interface IPeriodService
    {
        List<HmsLtrvPeriod> GetPeriods();
        HmsLtrvPeriod GetPeriod(string code); 
        bool DeletePeriod(string id);    
        HmsLtrvPeriod SavePeriod(HmsLtrvPeriod entity);
        bool IsPeriodExistByCode(string code);
        bool IsPeriodExist(string name);
        bool IsPeriodExist(string name, string typeCode);
        IEnumerable<CommonSelectModel> PeriodSelection();
    }
}