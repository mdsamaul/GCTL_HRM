using DocumentFormat.OpenXml.Math;
using GCTL.Core.ViewModels.TaxChallanEntry;
using GCTL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.TaxChallanEntryService
{
    public interface ITaxChallanEntryService
    {
        Task<EmployeeTaxChallanDropdownListResultViewModel> EmployeeTaxChallanDropdownList(EmployeeTaxChallanFilterViewModel filterData);
        Task<string> GetBankBranchAddressAsync(string branchId);
        Task<List<TaxChallanEntryBankDetailsResult>> GetBankDetails(string bankId);
        Task<(bool isSuccess, string message, object)> TaxChallanSaveEditAsync(HrmPayMonthlyTaxDepositEntryDto formData, string CompanyCode);
        Task<string> TaxDipositAutoIdAsync();
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
        //Task<List<HrmPayMonthlyTaxDepositEntryDto>> GetchallanEntryGridAsync();
        Task<(bool isSuccess, string message, object data)> DeleteTaxChallanEntryGridAsync(List<string> selectedIds);
        Task<List<AccFinancialYear>> FinancialYearGetAsync();
        Task<(List<HrmPayMonthlyTaxDepositEntryDto> Data, int TotalRecords, int FilteredRecords)> GetchallanEntryGridServerSideAsync(DataTableRequest request);
    }
}
