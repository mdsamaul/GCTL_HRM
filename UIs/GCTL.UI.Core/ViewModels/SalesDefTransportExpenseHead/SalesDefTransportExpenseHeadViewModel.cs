using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.SalesDefTransportExpenseHead;

namespace GCTL.UI.Core.ViewModels.SalesDefTransportExpenseHead
{
    public class SalesDefTransportExpenseHeadViewModel : BaseViewModel
    {
        public SalesDefTransportExpenseHeadSetupViewModel Setup { get; set; } = new SalesDefTransportExpenseHeadSetupViewModel();
    }
}
