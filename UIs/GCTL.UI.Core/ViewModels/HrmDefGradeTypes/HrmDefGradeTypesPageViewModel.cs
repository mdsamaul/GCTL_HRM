using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HrmDefGradeTypes;

namespace GCTL.UI.Core.ViewModels.HrmDefGradeTypes
{
    public class HrmDefGradeTypesPageViewModel:BaseViewModel
    {
        public HrmDefGradeTypesSetupViewModel Setup { get; set; } = new HrmDefGradeTypesSetupViewModel();
        public List<HrmDefGradeTypesSetupViewModel> TableListData { get; set; } = new List<HrmDefGradeTypesSetupViewModel>();
    }
}
