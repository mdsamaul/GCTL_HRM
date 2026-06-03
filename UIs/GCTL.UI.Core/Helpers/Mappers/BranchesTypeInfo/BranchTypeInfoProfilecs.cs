using AutoMapper;
using GCTL.Core.ViewModels.BranchesTypeInfo;
using GCTL.Data.Models;

namespace GCTL.UI.Core.Helpers.Mappers.BranchesTypeInfo
{
    public class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            CreateMap<CoreBranch, BranchTypeSetupViewModel>().ReverseMap();
        }
    }
}
