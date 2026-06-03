using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HRM_Size;

namespace GCTL.UI.Core.ViewModels.HRM_Size
{
    public class HRM_SizeViewModel :BaseViewModel
    {
        public HRM_SizeSetupViewModel Setup { get; set; } = new HRM_SizeSetupViewModel();
    }
}
