using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.SeasonInformation;
using GCTL.Core.ViewModels.SizeInformation;

namespace GCTL.UI.Core.ViewModels.SeasonInformation
{
    public class SeasonInformationPageViewModel : BaseViewModel
    {
        public SeasonInformationSetupViewModel Setup { get; set; } = new SeasonInformationSetupViewModel();
        public List<SeasonInformationSetupViewModel> SeasonList { get; set; } = new List<SeasonInformationSetupViewModel>();
    }
}
