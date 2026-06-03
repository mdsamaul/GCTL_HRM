using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.RMG_CostingInfo;

namespace GCTL.UI.Core.ViewModels.RMG_CostingInfo
{
    public class RMG_CostingInfoViewModel : BaseViewModel
    {
        public ProdOrderFilterDto Filter { get; set; } = new ProdOrderFilterDto();
        public FilterOptionsDto FilterOptions { get; set; } = new FilterOptionsDto();
        public List<ProdOrderReportDto> ProdOrderReports { get; set; } = new List<ProdOrderReportDto>();
        public List<ProdOrderReportRawDto> ProdOrderReportRaws { get; set; } = new List<ProdOrderReportRawDto>();
        public RmgCostingDetailsTempDto RmgCostingDetailsTemp { get; set; } = new RmgCostingDetailsTempDto();
        public RmgCostingInfoDto RmgCostingInfo { get; set; } = new RmgCostingInfoDto();
    }
}
