using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.ViewModels.AdvanceLoanAdjustment;
using GCTL.Core.ViewModels.DeleteHistories;

namespace GCTL.Service.AdvanceLoanAdjustment
{
    public interface IAdvanceLoanAdjustmentServices
    {
        Task<bool> PagePermissionAsync(string accessCode);

        Task<bool> SavePermissionAsync(string accessCode);

        Task<bool> UpdatePermissionAsync(string accessCode);

        Task<bool> DeletePermissionAsync(string accessCode);
        Task<List<CompanyDto>> GetAllAndFilterCompanyAsync(string searchCompanyName);
        Task<List<EmployeeAdjustmentDto>> GetEmployeesByFilterAsync(string employeeStatusId, string companyCode, string employeeName, bool loanAdjustment);
        Task<EmployeeAdjustmentDto> GetLoadEmployeeByIdAsync(string employeeId);
        Task<List<LoanDataDto>> GetLoanByEmployeeIdAsync(string employeeId);
        Task<LoanDataDto> GetLoanByIdAsync(string loanId);
        Task<(bool isSuccess, string message, object data)> SaveUpdateLoanAdjustmentAsync(AdvanceLoanAdjustmentSetupViewModel modelData);
        Task<string> AdjustmentAutoGanarateIdAsync();
        Task<List<MonthDto>> GetMonthAsync();
        Task<List<PayHeadNameDto>> GetHeadDeductionAsync();
        Task<DataTableResponse<AdvancePayViewModel>> GetAdvancePayPaged(DataTableRequest request);
        Task<(bool isSuccess, string message)> DeleteAdvancePayAsync(List<decimal> Ids, DeleteHistoryViewModel dModel);
    }
}
