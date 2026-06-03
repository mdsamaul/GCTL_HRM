using AutoMapper;
using GCTL.Core.ViewModels.Relation;
using GCTL.Data.Models;

namespace GCTL.UI.Core.Helpers.Mappers.Relation
{
    public class RelationProfile : Profile
    {
        public RelationProfile()
        {
            CreateMap<HmsRelation, RelationSetupViewModel>().ReverseMap();
        }
    }
}
