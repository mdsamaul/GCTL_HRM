using GCTL.Data.Models;
using AutoMapper;
using GCTL.Core.ViewModels.AccessCodes;

namespace GCTL.UI.Core.Helpers.Mappers.AccessCodes
{
    public class AccessCodeProfile : Profile
    {
        public AccessCodeProfile()
        {
            CreateMap<CoreAccessCode, AccessCodeSetupViewModel>().ReverseMap();

            CreateMap<AccessCodeModel, CoreAccessCode>().ReverseMap();
            CreateMap<CoreMenuTab2, AccessCodeModel>();

        }
    }
}
