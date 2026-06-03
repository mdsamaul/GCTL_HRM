using GCTL.Data.Models;
using AutoMapper;
using GCTL.Core.ViewModels.Religions;

namespace GCTL.UI.Core.Helpers.Mappers.Religions
{
    public class ReligionProfile : Profile
    {
        public ReligionProfile()
        {
            CreateMap<HrmDefReligion, ReligionSetupViewModel>().ReverseMap();
        }
    }
}
