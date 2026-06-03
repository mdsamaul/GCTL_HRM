using GCTL.Core.Reports;

namespace GCTL.UI.Core.Helpers.Reports
{
    public static class ReportHelpers
    {
        public static ApplicationReportRequest ProcessingRequest(this ApplicationReportRequest reportRequest, IWebHostEnvironment webHostEnvironment)
        {
            if (string.IsNullOrWhiteSpace(reportRequest.ReportPath))
                reportRequest.ReportPath = $"{webHostEnvironment.WebRootPath}\\Reports\\{reportRequest.ReportType}.rdlc";

            if (reportRequest.IsPreview)
            {
                var directory = $"{webHostEnvironment.WebRootPath}\\PDFViewer";
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                reportRequest.PreviewPath = $"{directory}\\{reportRequest.ReportType}.pdf";
            }
            return reportRequest;
        }
    }
}
