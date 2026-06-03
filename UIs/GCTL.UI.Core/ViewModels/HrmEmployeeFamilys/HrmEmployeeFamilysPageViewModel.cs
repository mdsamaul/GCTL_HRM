using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HrmEmployeeFamilys;

namespace GCTL.UI.Core.ViewModels.HrmEmployeeFamilys
{
    public class HrmEmployeeFamilysPageViewModel:BaseViewModel
    {
        public HrmEmployeeFamilysSetViewModel Setup { get; set; } = new HrmEmployeeFamilysSetViewModel();
        public List<HrmEmployeeFamilysSetViewModel> TableListData { get; set; } = new List<HrmEmployeeFamilysSetViewModel>();
    }
}
