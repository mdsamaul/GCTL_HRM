using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.JobTitles;

namespace GCTL.UI.Core.ViewModels.JobTitles
{
    public class JobTitlePageViewModel : BaseViewModel
    {
        public JobTitleSetupViewModel Setup { get; set; } = new JobTitleSetupViewModel();
        public List<JobTitleSetupViewModel> JobTitleList { get; set; } = new List<JobTitleSetupViewModel>();
    }
}
