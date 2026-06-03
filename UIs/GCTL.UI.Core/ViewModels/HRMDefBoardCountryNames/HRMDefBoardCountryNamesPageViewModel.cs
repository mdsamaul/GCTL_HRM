using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HRMDefBoardCountryNames;
using GCTL.Core.ViewModels.HrmDefDegrees;

namespace GCTL.UI.Core.ViewModels.HRMDefBoardCountryNames
{
    public class HRMDefBoardCountryNamesPageViewModel:BaseViewModel
    {
       

        public HRMDefBoardCountryNamesSetupViewModel Setup { get; set; } = new HRMDefBoardCountryNamesSetupViewModel();
        public List<HRMDefBoardCountryNamesSetupViewModel> TableListData { get; set; } = new List<HRMDefBoardCountryNamesSetupViewModel>();
    }
}
