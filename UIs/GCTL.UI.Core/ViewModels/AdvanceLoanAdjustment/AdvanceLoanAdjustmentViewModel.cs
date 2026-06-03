using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.AdvanceLoanAdjustment;

namespace GCTL.UI.Core.ViewModels.AdvanceLoanAdjustment
{
    public class AdvanceLoanAdjustmentViewModel : BaseViewModel
    {
        public AdvanceLoanAdjustmentSetupViewModel Setup { get; set; } = new AdvanceLoanAdjustmentSetupViewModel();
    }
}
