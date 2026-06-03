using GCTL.Data.Models;
using AutoMapper;
using GCTL.Core.ViewModels.Qualifications;

namespace GCTL.UI.Core.Helpers.Mappers.Qualifications
{
    public class QualificationProfile : Profile
    {
        public QualificationProfile()
        {
            CreateMap<HmsQualification, QualificationSetupViewModel>().ReverseMap();
        }
    }
}
