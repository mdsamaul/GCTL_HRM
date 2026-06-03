using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.BuyerDepartment;

namespace GCTL.UI.Core.ViewModels.InvBuyerDepartment
{
    public class BuyerDepartmentPageViewModel : BaseViewModel
    {
        public InvBuyerDepartmentViewModel Department { get; set; } = new InvBuyerDepartmentViewModel();
    }
}
