using GCTL.Core.ViewModels.Employees;
using GCTL.Data.Models;
using AutoMapper;

namespace GCTL.UI.Core.Helpers.Mappers.Employees
{
    public class EmployeeProfile : Profile
    {
        //public EmployeeProfile()
        //{
        //    CreateMap<HrmEmployee, EmployeeSetupViewModel>()
        //        .ForMember(d => d.DateOfBirth, src => src.MapFrom(x => (x.DateOfBirthCertificate.HasValue && x.DateOfBirthCertificate.GetValueOrDefault().Year > 1900) ? x.DateOfBirthCertificate.GetValueOrDefault().ToString("dd/MM/yyyy") : ""))
        //        .ForMember(d => d.autoId, src => src.Ignore());

        //    CreateMap<EmployeeSetupViewModel, HrmEmployee>()
        //        .ForMember(d => d.DateOfBirthCertificate, src => src.Ignore()); // DateOfBirthCertificate was DateOfBirth previous
        //}
    }
}