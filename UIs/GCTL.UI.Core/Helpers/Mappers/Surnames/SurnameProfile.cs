using GCTL.Data.Models;
using AutoMapper;
using GCTL.Core.ViewModels.Surnames;

namespace GCTL.UI.Core.Helpers.Mappers.Surnames
{
    public class SurnameProfile : Profile
    {
        public SurnameProfile()
        {
            CreateMap<HrmDefSurName, SurnameSetupViewModel>().ReverseMap();
        }
    }
}
