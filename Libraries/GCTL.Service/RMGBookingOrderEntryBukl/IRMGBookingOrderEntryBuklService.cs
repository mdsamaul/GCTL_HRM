using GCTL.Core.ViewModels.RMGBookingOrderEntryBukl;

namespace GCTL.Service.RMGBookingOrderEntryBukl
{
    public interface IRMGBookingOrderEntryBuklService
    {
        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string employeeCode, string phone, string email);

        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
        Task<(bool isSuccess, string message)> SaveBookingAsync(RMGBookingOrderEntryBuklDto dto, string companyCode);
        Task<(IEnumerable<object> data, int total, int filtered)> GetBookingListAsync(
       int start, int length, string search, string sortColumn, string sortDir);
        Task<(bool isSuccess, string message, object data)> GetBookingItemTypesAsync(string id);
        Task<(bool success, string message)> DeleteBookingOrderAsync(List<decimal> deleteBookingIds);

    }
}
