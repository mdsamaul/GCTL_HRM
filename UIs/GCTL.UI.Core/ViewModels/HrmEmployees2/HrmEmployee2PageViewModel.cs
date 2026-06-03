using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HrmEmployees2;

namespace GCTL.UI.Core.ViewModels.HrmEmployees2
{
    public class HrmEmployee2PageViewModel:BaseViewModel
    {
        public HrmEmployee2SetUpViewModel Setup=new HrmEmployee2SetUpViewModel();

        public List<HrmEmployee2SetUpViewModel> ListTableData = new List<HrmEmployee2SetUpViewModel>();

    }
}
