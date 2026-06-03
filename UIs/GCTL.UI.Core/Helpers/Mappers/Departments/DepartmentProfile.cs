using GCTL.Data.Models;
using AutoMapper;
using GCTL.Core.ViewModels.Departments;

namespace GCTL.UI.Core.Helpers.Mappers.Departments
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<HrmDefDepartment, DepartmentSetupViewModel>().ReverseMap();
        }
    }
}
