using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.ViewModels.PFAssignEntry;

namespace GCTL.Service.PFAssignEntryReport
{
    public interface IPFAssignEntryReportServices
    {
        Task<bool> PagePermissionAsync(string accessCode);
        //Task<PFAssignEntryFilterListDto> GetPFDataAsync(PFAssignEntryFilterDto FilterData);
        Task<PFAssignEntryFilterListDto> GetPFBaseAndFilteredDataAsync(PFAssignEntryFilterDto FilterData);
        Task<PFAssignEntryFilterListDto> GetPFDataPdfAsync(PFAssignEntryFilterDto FilterData);
    }
}
