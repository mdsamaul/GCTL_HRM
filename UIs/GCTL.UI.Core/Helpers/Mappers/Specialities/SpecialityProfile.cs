using GCTL.Data.Models;
using AutoMapper;
using GCTL.Core.ViewModels.Specialities;

namespace GCTL.UI.Core.Helpers.Mappers.Specialities
{
    public class SpecialityProfile : Profile
    {
        public SpecialityProfile()
        {
            CreateMap<HmsSpeciality, SpecialitySetupViewModel>().ReverseMap();
        }
    }
}
