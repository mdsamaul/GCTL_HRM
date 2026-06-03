using GCTL.Core.ViewModels.EachGcFilterRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.EachGcFilterRequestService
{
    public interface IGcFilterService
    {
        Task<PagedResultDto<GcItemDto>> GetCompaniesAsync(GcFilterRequestDto req);
        Task<PagedResultDto<GcItemDto>> GetBranchesAsync(GcFilterRequestDto req);
        Task<PagedResultDto<GcItemDto>> GetDivisionsAsync(GcFilterRequestDto req);
        Task<PagedResultDto<GcItemDto>> GetDepartmentsAsync(GcFilterRequestDto req);
        Task<PagedResultDto<GcItemDto>> GetDesignationsAsync(GcFilterRequestDto req);
        Task<PagedResultDto<GcItemDto>> GetEmployeesAsync(GcFilterRequestDto req);
    }
}
