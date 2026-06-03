using GCTL.Data.Models;
using AutoMapper;
using GCTL.Core.ViewModels.Currencies;

namespace GCTL.UI.Core.Helpers.Mappers.Currencies
{
    public class CurrencyProfile : Profile
    {
        public CurrencyProfile()
        {
            CreateMap<CaDefCurrency, CurrencySetupViewModel>().ReverseMap();
        }
    }
}
