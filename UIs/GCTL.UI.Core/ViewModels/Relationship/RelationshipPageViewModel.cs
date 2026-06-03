using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.Relationship;

namespace GCTL.UI.Core.ViewModels.Relationship
{
    public class RelationshipPageViewModel : BaseViewModel
    {
        public RelationshipSetupViewModel Setup { get; set; } = new RelationshipSetupViewModel();
        public List<RelationshipSetupViewModel> RelationshipList { get; set; } = new List<RelationshipSetupViewModel>();
    }
}
