using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HRM_Def_Floor;

namespace GCTL.UI.Core.ViewModels.HRM_Def_Floor
{
    public class HRM_Def_FloorViewModel: BaseViewModel
    {
        public HRM_Def_FloorSetupViewModel Setup { get; set; }  = new HRM_Def_FloorSetupViewModel();
    }
}
