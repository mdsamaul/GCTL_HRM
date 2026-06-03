using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;

namespace GCTL.Service.DeliveryPeriods
{
    public interface IDeliveryPeriodService
    {
        List<HmsDeliveryPeriod> GetDeliveryPeriods();
        HmsDeliveryPeriod GetDeliveryPeriod(string code); 
        bool DeleteDeliveryPeriod(string id);    
        HmsDeliveryPeriod SaveDeliveryPeriod(HmsDeliveryPeriod entity);
        bool IsDeliveryPeriodExistByCode(string code);
        bool IsDeliveryPeriodExist(string name);
        bool IsDeliveryPeriodExist(string name, string typeCode);
        IEnumerable<CommonSelectModel> DeliveryPeriodSelection();
    }
}