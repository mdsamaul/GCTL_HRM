using AspNetCore.Reporting;

namespace GCTL.Core.Reports
{
    public class ApplicationReportResponse
    {
        public ReportRenderingType ReportRenderType { get; set; }
        public ReportResult ReportResult { get; set; }
        public string MimeType { get; set; }
        public string Extension { get; set; }
        public string FileName { get; set; }
        public string PreviewPath { get; set; }
}
}
