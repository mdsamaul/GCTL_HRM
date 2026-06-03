using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.Country;

namespace GCTL.UI.Core.ViewModels.Country
{
    public class CountryPageViewModel : BaseViewModel
    {
        public CountrySetuoViewModel Setup { get; set; } = new CountrySetuoViewModel();
        public List<CountrySetuoViewModel> CountryList { get; set; } = new List<CountrySetuoViewModel>();
    }
}
