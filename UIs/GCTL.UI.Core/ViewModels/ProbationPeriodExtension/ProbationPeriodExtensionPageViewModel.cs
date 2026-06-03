using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.ProbationPeriodExtension;

namespace GCTL.UI.Core.ViewModels.ProbationPeriodExtension
{
    public class ProbationPeriodExtensionPageViewModel : BaseViewModel
    {
        public ProbationPeriodExtensionSetupViewModel Setup { get; set; } = new ProbationPeriodExtensionSetupViewModel();
      
        public List<ProbationPeriodExtensionGetAll> ProbationPeriodExtensionList2 { get; set; } = new List<ProbationPeriodExtensionGetAll>();
    }
}

