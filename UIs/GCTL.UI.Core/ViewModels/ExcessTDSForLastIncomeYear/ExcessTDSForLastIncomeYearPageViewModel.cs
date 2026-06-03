using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.ExcessTDSForLastIncomeYear;

namespace GCTL.UI.Core.ViewModels.ExcessTDSForLastIncomeYear
{
    public class ExcessTDSForLastIncomeYearPageViewModel : BaseViewModel
    {
        public ExcessTDSForLastIncomeYearSetupViewModel Setup { get; set; } = new ExcessTDSForLastIncomeYearSetupViewModel();
        public List<ExcessTDSForLastIncomeYearSetupViewModel> ExcessTDSForLastIncomeYearList2 { get; set; } = new List<ExcessTDSForLastIncomeYearSetupViewModel>();
        public List<ExcessTDSForLastIncomeYearSetupViewModel> ExcessTDSForLastIncomeYearList { get; set; } = new List<ExcessTDSForLastIncomeYearSetupViewModel>();
    }
}
