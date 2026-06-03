using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;

namespace GCTL.Service.Currencies
{
    public class CurrencyService : AppService<CaDefCurrency>, ICurrencyService
    {
        private readonly IRepository<CaDefCurrency> currencyRepository;

        public CurrencyService(IRepository<CaDefCurrency> currencyRepository)
            : base(currencyRepository)
        {
            this.currencyRepository = currencyRepository;
        }

        public List<CaDefCurrency> GetCurrencies()
        {
            return GetAll();
        }

        public CaDefCurrency GetCurrency(string id)
        {
            return currencyRepository.GetById(id);
        }

        public CaDefCurrency SaveCurrency(CaDefCurrency entity)
        {
            if (IsCurrencyExistByCode(entity.CurrencyId))
                Update(entity);
            else
                Add(entity);

            return entity;
        }

        public bool DeleteCurrency(string id)
        {
            var entity = GetCurrency(id);
            if (entity != null)
            {
                currencyRepository.Delete(entity);
                return true;
            }
            return false;
        }

        public bool IsCurrencyExistByCode(string code)
        {
            return currencyRepository.All().Any(x => x.CurrencyId == code);
        }

        public bool IsCurrencyExist(string name)
        {
            return currencyRepository.All().Any(x => x.CurrencyName == name);
        }

        public bool IsCurrencyExist(string name, string typeCode)
        {
            return currencyRepository.All().Any(x => x.CurrencyName == name && x.CurrencyId != typeCode);
        }

        public IEnumerable<CommonSelectModel> CurrencySelection()
        {
            return currencyRepository.All()
                .Select(x => new CommonSelectModel
                {
                    Code = x.CurrencyId,
                    Name = x.ShortName
                });
        }
    }
}
