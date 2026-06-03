using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HrmDefExamGroupInfos;

namespace GCTL.UI.Core.ViewModels.HrmDefExamGroupInfos
{
    public class HrmDefExamGroupInfosPageViewModel:BaseViewModel
    {
        public HrmDefExamGroupInfosSetupViewModel Setup { get; set; } = new HrmDefExamGroupInfosSetupViewModel();
        public List<HrmDefExamGroupInfosSetupViewModel> TableList { get; set; } = new List<HrmDefExamGroupInfosSetupViewModel>();
    }
}
