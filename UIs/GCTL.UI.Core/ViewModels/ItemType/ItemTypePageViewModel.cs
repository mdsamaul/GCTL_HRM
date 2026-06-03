using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.ItemType;

namespace GCTL.UI.Core.ViewModels.ItemType
{
    public class ItemTypePageViewModel : BaseViewModel
    {
        public ItemTypeSetupViewModel Setup { get; set; } = new ItemTypeSetupViewModel();
        public List<ItemTypeSetupViewModel> ItemList { get; set; } = new List<ItemTypeSetupViewModel>();
    }
}

