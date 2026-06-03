using GCTL.Data.Models;
using AutoMapper;
using GCTL.Core.ViewModels.DoctorWorkingPlace;

namespace GCTL.UI.Core.Helpers.Mappers.DoctorWorkingPlace
{
    public class DoctorWorkingPlaceProfile : Profile
    {
        public DoctorWorkingPlaceProfile()
        {
            CreateMap<HmsDoctorWorkingPlace, DoctorWorkingPlaceSetupViewModel>().ReverseMap();
        }
    }
}
