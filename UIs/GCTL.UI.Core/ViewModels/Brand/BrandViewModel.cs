using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.Brand;

namespace GCTL.UI.Core.ViewModels.Brand
{
    public class BrandViewModel:BaseViewModel
    {
        public BrandSetupViewModel Setup { get; set; } =new BrandSetupViewModel();
    }
}
