using GCTL.Data.Models;
using AutoMapper;
using GCTL.Core.ViewModels.Designations;

namespace GCTL.UI.Core.Helpers.Mappers.Departments
{
    public class DesignationProfile : Profile
    {
        public DesignationProfile()
        {
            CreateMap<HrmDefDesignation, DesignationSetupViewModel>().ReverseMap();
        }
    }
}
