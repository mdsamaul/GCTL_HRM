using GCTL.Core.ViewModels.EachGcFilterRequest;
using GCTL.Core.ViewModels.GcAccessFilterRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.GcAccessFilterService
{
    public interface IGcAccessFilterService
    {
        Task<PagedAccessResultDto<GcAccessItemDto>> GetCompanyListByAccessAsync(GcAccessFilterRequestDto req);
        Task<PagedAccessResultDto<GcAccessItemDto>> GetBranchListByAccessAsync(GcAccessFilterRequestDto req);
        Task<PagedAccessResultDto<GcAccessItemDto>> GetDivisionListByAccessAsync(GcAccessFilterRequestDto req);
        Task<PagedAccessResultDto<GcAccessItemDto>> GetDepartmentListByAccessAsync(GcAccessFilterRequestDto req);
        Task<PagedAccessResultDto<GcAccessItemDto>> GetDesignationListByAccessAsync(GcAccessFilterRequestDto req);
        Task<PagedAccessResultDto<GcAccessItemDto>> GetEmployeeListByAccessAsync(GcAccessFilterRequestDto req);

    }
}
