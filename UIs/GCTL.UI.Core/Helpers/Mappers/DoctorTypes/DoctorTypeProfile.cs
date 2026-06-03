using GCTL.Data.Models;
using AutoMapper;
using GCTL.Core.ViewModels.DoctorTypes;

namespace GCTL.UI.Core.Helpers.Mappers.DoctorTypes
{
    public class DoctorTypeProfile : Profile
    {
        public DoctorTypeProfile()
        {
            CreateMap<HmsDoctorType, DoctorTypeSetupViewModel>().ReverseMap();
        }
    }
}
