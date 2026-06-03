using GCTL.Core.ViewModels.UserAccesses;
using GCTL.Core.ViewModels.Users;
using GCTL.Data.Models;
using AutoMapper;

namespace GCTL.UI.Core.Helpers.Mappers.UserAccess
{
    public class UserAccessProfile : Profile
    {
        public UserAccessProfile()
        {
            CreateMap<CoreUserInfo, UserAccessSetupViewModel>().ReverseMap();
            CreateMap<UserViewModel, UserAccessSetupViewModel>().ReverseMap();
        }
    }
}
