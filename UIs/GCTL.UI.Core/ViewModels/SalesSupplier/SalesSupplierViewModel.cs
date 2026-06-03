using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.SalesSupplier;

namespace GCTL.UI.Core.ViewModels.SalesSupplier
{
    public class SalesSupplierViewModel:BaseViewModel
    {
        public SalesSupplierSetupViewModel Setup { get; set; }=new SalesSupplierSetupViewModel();
    }
}
