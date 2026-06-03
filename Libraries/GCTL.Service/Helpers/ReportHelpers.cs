using AspNetCore.Reporting;
using GCTL.Core.Reports;
using System.Collections;
using System.Net.Mime;

namespace GCTL.Service.Helpers
{
    public static class ReportHelpers
    {
        public static ApplicationReportResponse ToResponse(this ApplicationReportResponse response)
        {
            switch (response.ReportRenderType)
            {
                case ReportRenderingType.PDF:
                    response.MimeType = MediaTypeNames.Application.Octet;
                    //application/pdf
                    response.Extension = ".pdf";
                    break;
                case ReportRenderingType.Excel:
                    response.MimeType = "application/msexcel";
                    response.Extension = ".xls";
                    break;
                case ReportRenderingType.Word:
                    response.MimeType = "application/msword";
                    response.Extension = ".doc";
                    break;
                default:
                    break;
            }

            return response;
        }

        public static RenderType ToRenderType(this ReportRenderingType reportType)
        {
            switch (reportType)
            {
                case ReportRenderingType.PDF:
                    return RenderType.Pdf;
                case ReportRenderingType.Excel:
                    return RenderType.Excel;
                case ReportRenderingType.Word:
                    return RenderType.Word;
                default:
                    return RenderType.Pdf;
            }
        }

        public static ApplicationReportRequest SetSource(this ApplicationReportRequest reportRequest, IEnumerable data)
        {
            reportRequest.Source = data;
            return reportRequest;
        }
    }
}
