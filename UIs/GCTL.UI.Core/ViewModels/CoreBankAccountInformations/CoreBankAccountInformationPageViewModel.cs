using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.CoreBankAccountInformations;

namespace GCTL.UI.Core.ViewModels.CoreBankAccountInformations
{
    public class CoreBankAccountInformationPageViewModel:BaseViewModel
    {
        public CoreBankAccountInformationSetupViewModel Setup = new CoreBankAccountInformationSetupViewModel();
        public List<CoreBankAccountInformationSetupViewModel> TableList= new List<CoreBankAccountInformationSetupViewModel>();
    }
}
