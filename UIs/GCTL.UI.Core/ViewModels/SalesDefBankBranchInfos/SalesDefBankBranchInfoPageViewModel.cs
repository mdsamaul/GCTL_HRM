using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.SalesDefBankBranchInfos;

namespace GCTL.UI.Core.ViewModels.SalesDefBankBranchInfos
{
    public class SalesDefBankBranchInfoPageViewModel:BaseViewModel
    {
        public SalesDefBankBranchInfoSetupViewModel Setup = new SalesDefBankBranchInfoSetupViewModel();
        public List<SalesDefBankBranchInfoSetupViewModel> TableList=new List<SalesDefBankBranchInfoSetupViewModel>();
    }
}
