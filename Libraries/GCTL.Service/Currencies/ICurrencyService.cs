using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;

namespace GCTL.Service.Currencies
{
    public interface ICurrencyService
    {
        List<CaDefCurrency> GetCurrencies();
        CaDefCurrency GetCurrency(string code); 
        bool DeleteCurrency(string id);    
        CaDefCurrency SaveCurrency(CaDefCurrency entity);
        bool IsCurrencyExistByCode(string code);
        bool IsCurrencyExist(string name);
        bool IsCurrencyExist(string name, string typeCode);
        IEnumerable<CommonSelectModel> CurrencySelection();
    }
}