using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.Currencies;
using GCTL.Core.ViewModels.UnitTypes;

namespace GCTL.UI.Core.ViewModels.Currencies
{
    public class CurrencyPageViewModel : BaseViewModel
    {
        public CurrencySetupViewModel Setup { get; set; } = new CurrencySetupViewModel();
    }
}
