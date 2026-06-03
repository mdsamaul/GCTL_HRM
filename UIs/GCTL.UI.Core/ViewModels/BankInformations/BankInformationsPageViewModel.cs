using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.BankInformations;
using GCTL.Core.ViewModels.HrmDefEmpTypes;

namespace GCTL.UI.Core.ViewModels.BankInformations
{
    public class BankInformationsPageViewModel:BaseViewModel
    {
        public BankInformationsSetupViewModel Setup { get; set; } = new BankInformationsSetupViewModel();
        public List<BankInformationsSetupViewModel> tableDataList { get; set; } = new List<BankInformationsSetupViewModel>();
    }
}
