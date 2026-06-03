using GCTL.Data.Models;
using AutoMapper;
using GCTL.Core.ViewModels.BloodGroups;

namespace GCTL.UI.Core.Helpers.Mappers.BloodGroups
{
    public class BloodGroupProfile : Profile
    {
        public BloodGroupProfile()
        {
            CreateMap<HrmDefBloodGroup, BloodGroupSetupViewModel>().ReverseMap();
        }
    }
}
