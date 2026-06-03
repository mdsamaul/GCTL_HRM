using GCTL.Data.Models;
using AutoMapper;
using GCTL.Core.ViewModels.Shifts;

namespace GCTL.UI.Core.Helpers.Mappers.Shifts
{
    public class ShiftProfile : Profile
    {
        public ShiftProfile()
        {
            CreateMap<HmsShift, ShiftSetupViewModel>()
                 .ForMember(d => d.InTime, src => src.MapFrom(x => (x.InTime.HasValue && x.InTime.GetValueOrDefault().Year > 1900) ? x.InTime.GetValueOrDefault().ToString("hh:mm tt") : ""))
                 .ForMember(d => d.OutTime, src => src.MapFrom(x => (x.OutTime.HasValue && x.OutTime.GetValueOrDefault().Year > 1900) ? x.OutTime.GetValueOrDefault().ToString("hh:mm tt") : ""));

            CreateMap<ShiftSetupViewModel, HmsShift>()
                 .ForMember(d => d.InTime, src => src.MapFrom(x => !string.IsNullOrWhiteSpace(x.InTime) ? DateTime.Parse(x.InTime) : DateTime.MinValue))
                 .ForMember(d => d.OutTime, src => src.MapFrom(x => !string.IsNullOrWhiteSpace(x.OutTime) ? DateTime.Parse(x.OutTime) : DateTime.MinValue));

            CreateMap<HmsShift, ShiftViewModel>()
                .ForMember(d => d.InTime, src => src.MapFrom(x => (x.InTime.HasValue && x.InTime.GetValueOrDefault().Year > 1900) ? x.InTime.GetValueOrDefault().ToString("hh:mm tt") : ""))
                .ForMember(d => d.OutTime, src => src.MapFrom(x => (x.OutTime.HasValue && x.OutTime.GetValueOrDefault().Year > 1900) ? x.OutTime.GetValueOrDefault().ToString("hh:mm tt") : ""));
        }
    }
}
