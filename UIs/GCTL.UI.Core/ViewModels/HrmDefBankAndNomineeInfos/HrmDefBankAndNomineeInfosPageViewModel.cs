using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HrmDefBankAndNomineeInfos;

namespace GCTL.UI.Core.ViewModels.HrmDefBankAndNomineeInfos
{
    public class HrmDefBankAndNomineeInfosPageViewModel:BaseViewModel
    {
        public HrmDefBankAndNomineeInfosSetupViewModel Setup { get; set; }=new HrmDefBankAndNomineeInfosSetupViewModel();
        public List<HrmDefBankAndNomineeInfosSetupViewModel> TableListData { get; set; } = new List<HrmDefBankAndNomineeInfosSetupViewModel>();
    }
}
