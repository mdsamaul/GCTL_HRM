using GCTL.Core.ViewModels.Companies;
using GCTL.Core.ViewModels.Employees;
using GCTL.Data.Models;
using AutoMapper;
using GCTL.Core.ViewModels.Doctors;

namespace GCTL.UI.Core.Helpers.Mappers.Doctors
{
    public class DoctorProfile : Profile
    {
        public DoctorProfile()
        {
            CreateMap<HmsDoctor, DoctorSetupViewModel>()
                // .ForMember(d => d.EmployeeName, src => src.MapFrom(x => x.FirstName))
                .ForMember(d => d.DateOfBirth, src => src.MapFrom(x => (x.DateOfBirth.GetValueOrDefault().Year > 1905) ? x.DateOfBirth.GetValueOrDefault().ToString("dd/MM/yyyy") : ""))
                .ForMember(d => d.JoiningDate, src => src.MapFrom(x => (x.JoiningDate.GetValueOrDefault().Year > 1905) ? x.DateOfBirth.GetValueOrDefault().ToString("dd/MM/yyyy") : ""));
                //.ForMember(d => d.DoctorId, src => src.Ignore());

            CreateMap<DoctorSetupViewModel, HmsDoctor>()
                .ForMember(d => d.DateOfBirth, src => src.Ignore())
                .ForMember(d => d.JoiningDate, src => src.Ignore());
        }
    }
}
