using System.Security.Policy;
using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.InvDefSupplierType;

namespace GCTL.UI.Core.ViewModels.InvDefSupplierType
{
    public class InvDefSupplierTypeViewModel:BaseViewModel
    {
        public InvDefSupplierTypeSetupViewModel Setup { get; set; } = new InvDefSupplierTypeSetupViewModel();
    }
}
