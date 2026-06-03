using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.ItemMasterInformation;
using GCTL.UI.Core.ViewModels.INV_Catagory;

namespace GCTL.UI.Core.ViewModels.ItemMasterInformation
{
    public class ItemMasterInformationViewModel : BaseViewModel
    {
      public  ItemMasterInformationSetupViewModel Setup { get; set; } = new ItemMasterInformationSetupViewModel();
    }
}
