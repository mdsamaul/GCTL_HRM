using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HrmDefOccupations;

namespace GCTL.UI.Core.ViewModels.HrmDefOccupations
{
    public class HrmDefOccupationsPageViewModel : BaseViewModel
    {
        public HrmDefOccupationsSetupViewModel Setup { get; set; }= new HrmDefOccupationsSetupViewModel();
        public List<HrmDefOccupationsSetupViewModel> TableListData { get; set; } = new List<HrmDefOccupationsSetupViewModel>();
    }
}
