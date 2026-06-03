using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HolidayTypes;
using GCTL.Core.ViewModels.HrmAttWorkingDayDeclarations;

namespace GCTL.UI.Core.ViewModels.HrmAttWorkingDayDeclarations
{
    public class HrmAttWorkingDayDeclarationPageViewModel:BaseViewModel
    {
        public HrmAttWorkingDayDeclarationSetupViewModel Setup { get; set; } = new HrmAttWorkingDayDeclarationSetupViewModel();
        public List<HrmAttWorkingDayDeclarationSetupViewModel> TableListdata { get; set; } = new List<HrmAttWorkingDayDeclarationSetupViewModel>();
    }
}
