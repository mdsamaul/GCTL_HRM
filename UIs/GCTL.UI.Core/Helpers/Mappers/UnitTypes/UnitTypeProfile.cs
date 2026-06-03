using GCTL.Core.ViewModels.UnitTypes;
using GCTL.Data.Models;
using AutoMapper;

namespace GCTL.UI.Core.Helpers.Mappers.UnitTypes
{
    public class UnitTypeProfile : Profile
    {
        public UnitTypeProfile()
        {
            CreateMap<RmgProdDefUnitType, UnitTypeSetupViewModel>().ReverseMap();
        }
    }
}
