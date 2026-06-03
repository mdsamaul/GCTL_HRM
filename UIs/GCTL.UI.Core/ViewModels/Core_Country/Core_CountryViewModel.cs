using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.Core_Country;

namespace GCTL.UI.Core.ViewModels.Core_Country
{
    public class Core_CountryViewModel:BaseViewModel
    {
        public Core_CountrySetupViewModel Setup { get; set; }= new Core_CountrySetupViewModel();
    }
}
