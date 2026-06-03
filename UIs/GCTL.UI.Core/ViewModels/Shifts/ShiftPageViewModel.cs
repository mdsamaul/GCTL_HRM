using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.Shifts;

namespace GCTL.UI.Core.ViewModels.Shifts
{
    public class ShiftPageViewModel : BaseViewModel
    {
        public ShiftSetupViewModel Setup { get; set; } = new ShiftSetupViewModel();
    }
}
