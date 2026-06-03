using GCTL.Data.Models;
using AutoMapper;
using GCTL.Core.ViewModels.PaymentModes;

namespace GCTL.UI.Core.Helpers.Mappers.PaymentModes
{
    public class PaymentModeProfile : Profile
    {
        public PaymentModeProfile()
        {
            CreateMap<SalesDefPaymentMode, PaymentModeSetupViewModel>().ReverseMap();
        }
    }
}
