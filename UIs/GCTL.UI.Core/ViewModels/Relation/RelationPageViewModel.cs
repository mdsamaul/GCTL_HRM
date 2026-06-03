using GCTL.Core.ViewModels.Relation;
using GCTL.Core.ViewModels;

namespace GCTL.UI.Core.ViewModels.Relation
{
    public class RelationPageViewModel : BaseViewModel
    {
        public RelationSetupViewModel Setup { get; set; } = new RelationSetupViewModel();
    }
    
}
