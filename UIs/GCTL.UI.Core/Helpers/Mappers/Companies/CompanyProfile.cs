using GCTL.Core.ViewModels.Companies;
using GCTL.Data.Models;
using AutoMapper;

namespace GCTL.UI.Core.Helpers.Mappers.Companies
{
    public class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            CreateMap<CoreCompany, CompanySetupViewModel>().ReverseMap();
        }
    }
}
