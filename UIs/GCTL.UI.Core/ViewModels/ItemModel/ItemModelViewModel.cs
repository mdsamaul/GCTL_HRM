using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.ItemModel;

namespace GCTL.UI.Core.ViewModels.ItemModel
{
    public class ItemModelViewModel:BaseViewModel
    {
        public ItemModelSetupViewModel Setup{ get; set; } = new ItemModelSetupViewModel();
    }
}
