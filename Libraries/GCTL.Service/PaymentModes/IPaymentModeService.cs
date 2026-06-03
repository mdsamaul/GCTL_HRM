using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;

namespace GCTL.Service.PaymentModes
{
    public interface IPaymentModeService
    {
        List<SalesDefPaymentMode> GetPaymentModes();
        SalesDefPaymentMode GetPaymentMode(string code); 
        bool DeletePaymentMode(string id);    
        SalesDefPaymentMode SavePaymentMode(SalesDefPaymentMode entity);
        bool IsPaymentModeExistByCode(string code);
        bool IsPaymentModeExist(string name);
        bool IsPaymentModeExist(string name, string typeCode);
        IEnumerable<CommonSelectModel> PaymentModeSelection();
        IEnumerable<CommonSelectModel> PaymentModeSelectionOnlyCash();
    }
}