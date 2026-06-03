using GCTL.Data.Models;
using AutoMapper;
using GCTL.Core.ViewModels.AdmissionTypes;

namespace GCTL.UI.Core.Helpers.Mappers.AdmissionTypes
{
    public class AdmissionTypeProfile : Profile
    {
        public AdmissionTypeProfile()
        {
            CreateMap<HmsAdmissionType, AdmissionTypeSetupViewModel>().ReverseMap();
        }
    }
}
