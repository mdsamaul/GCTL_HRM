using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.TaxChallanEntry;

namespace GCTL.UI.Core.ViewModels.TaxChallanEntry
{
    public class EmployeeTaxChallanViewModel:BaseViewModel
    {
        public HrmPayMonthlyTaxDepositEntryDto Setup { get; set; } = new HrmPayMonthlyTaxDepositEntryDto();
    }
}
