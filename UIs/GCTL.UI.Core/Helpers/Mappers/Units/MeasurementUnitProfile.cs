using GCTL.Data.Models;
using AutoMapper;
using GCTL.Core.ViewModels.Units;

namespace GCTL.UI.Core.Helpers.Mappers.Units
{
    public class MeasurementUnitProfile : Profile
    {
        public MeasurementUnitProfile()
        {
            CreateMap<CoreMeasurementUnit, MeasurementUnitSetupViewModel>().ReverseMap();
        }
    }
}
