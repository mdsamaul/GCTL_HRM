using GCTL.Core.Reports;

namespace GCTL.Service.Reports
{
    public interface IReportService
    {
        ApplicationReportResponse GenerateReport(ApplicationReportRequest reportRequest);
    }
}