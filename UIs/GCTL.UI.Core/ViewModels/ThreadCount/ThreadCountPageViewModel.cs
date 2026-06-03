using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.ThreadCount;

namespace GCTL.UI.Core.ViewModels.ThreadCount
{
    public class ThreadCountPageViewModel : BaseViewModel
    {
        public ThreadCountSetupViewModel Setup { get; set; } = new ThreadCountSetupViewModel();
        public List<ThreadCountSetupViewModel> ThreadCountList { get; set; } = new List<ThreadCountSetupViewModel>();
    }
}
