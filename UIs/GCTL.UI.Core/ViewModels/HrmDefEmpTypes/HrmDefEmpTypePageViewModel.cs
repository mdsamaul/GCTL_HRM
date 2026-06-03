using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HRMCompanyWeekEnds;
using GCTL.Core.ViewModels.HrmDefEmpTypes;

namespace GCTL.UI.Core.ViewModels.HrmDefEmpTypes
{
    public class HrmDefEmpTypePageViewModel:BaseViewModel
    {
        public HrmDefEmpTypeSetupViewModel Setup { get; set; } = new HrmDefEmpTypeSetupViewModel();
        public List<HrmDefEmpTypeSetupViewModel> tableDataList { get; set; } = new List<HrmDefEmpTypeSetupViewModel>();
    }
}
