using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.INV_Catagory;

namespace GCTL.UI.Core.ViewModels.INV_Catagory
{
    public class INV_CatagoryViewModel:BaseViewModel
    {
        public INV_CatagorySetupViewModel Setup { get; set; }= new INV_CatagorySetupViewModel();
    }
}
