using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HRMDefExamTitles;

namespace GCTL.UI.Core.ViewModels.HRMDefExamTitles
{
    public class HRMDefExamTitlesPageViewModel:BaseViewModel
    {
        public HRMDefExamTitlesSetupViewModel Setup { get; set; } = new HRMDefExamTitlesSetupViewModel();
        public List<HRMDefExamTitlesSetupViewModel> TableListData { get; set; } = new List<HRMDefExamTitlesSetupViewModel>();
    }
}
