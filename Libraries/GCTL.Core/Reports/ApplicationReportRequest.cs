using System.Collections;

namespace GCTL.Core.Reports
{
    public class ApplicationReportRequest
    {
        public ReportType ReportType { get; set; }
        public Dictionary<string, IEnumerable> Sources { get; set; }
        public IEnumerable Source { get; set; }
        public ReportRenderingType ReportRenderType { get; set; } = ReportRenderingType.PDF;
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
        public string ReportPath { get; set; }
        public string ImagePath { get; set; }
        public bool IsPreview { get; set; }
        public string PreviewPath { get; set; }
    }
}
