using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.AccFinancialYears;

namespace GCTL.UI.Core.ViewModels.AccFinancialYears
{
    public class AccFinancialYearPageViewModel : BaseViewModel
    {
        public AccFinancialYearSetupViewModel Setup { get; set; } = new AccFinancialYearSetupViewModel();

        public List<AccFinancialYearSetupViewModel> FinancialYearList { get; set; } = new List<AccFinancialYearSetupViewModel>();
    }
}
