using GCTL.Data.Models;
using AutoMapper;
using GCTL.Core.ViewModels.Units;

namespace GCTL.UI.Core.Helpers.Mappers.Units
{
    public class UnitProfile : Profile
    {
        public UnitProfile()
        {
            CreateMap<HmsLtrvUnit, UnitSetupViewModel>().ReverseMap();
        }
    }
}
