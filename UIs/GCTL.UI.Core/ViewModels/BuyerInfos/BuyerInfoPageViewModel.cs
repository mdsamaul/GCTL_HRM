using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.BuyerInfos;
using GCTL.Core.ViewModels.BuyerDLAddress;
using GCTL.Core.ViewModels.BuyerBrands;
using GCTL.Core.ViewModels.Accounts;

namespace GCTL.UI.Core.ViewModels.BuyerInfos
{
    public class BuyerInfoPageViewModel : BaseViewModel
    {
        public BuyerInfoSetupViewModel Setup { get; set; } = new BuyerInfoSetupViewModel();
        public RMGProdDLAddressViewModel DLAddress { get; set; } = new RMGProdDLAddressViewModel();
        public RMGProdBrandViewModel Brand { get; set; } = new RMGProdBrandViewModel();

        internal void ToAudit(UserInfoViewModel loginInfo, bool v)
        {
            throw new NotImplementedException();
        }
    }
}
