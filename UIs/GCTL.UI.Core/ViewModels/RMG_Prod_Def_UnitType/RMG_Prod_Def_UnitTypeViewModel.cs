using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.RMG_Prod_Def_UnitType;

namespace GCTL.UI.Core.ViewModels.RMG_Prod_Def_UnitType
{
    public class RMG_Prod_Def_UnitTypeViewModel:BaseViewModel
    {
        public RMG_Prod_Def_UnitTypeSetupViewModel Setup { get; set; }= new RMG_Prod_Def_UnitTypeSetupViewModel();
    }
}
