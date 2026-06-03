using AspNetCore.Reporting;
using GCTL.Core.Reports;
using System.Text;
using GCTL.Service.Helpers;
using Humanizer.Localisation;
using System.Reflection;

namespace GCTL.Service.Reports
{
    public class ReportService : IReportService
    {
        
        public ApplicationReportResponse GenerateReport(ApplicationReportRequest request)
        {
            ApplicationReportResponse response = new ApplicationReportResponse()
            {
                ReportRenderType = request.ReportRenderType,
                FileName = request.ReportType.ToString()
            };

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding.GetEncoding("windows-1252");

            LocalReport report = new LocalReport(request.ReportPath);
            if (request.Sources != null && request.Sources.Any())
            {
                foreach (var source in request.Sources)
                {
                    report.AddDataSource(source.Key, source.Value);
                }
            }
            int ext = (int)(DateTime.Now.Ticks >> 10);
            var result = report.Execute(request.ReportRenderType.ToRenderType(), ext, request.Parameters);
            
            response.ReportResult = result;
            if (request.IsPreview)
            {
                using (FileStream fs = new FileStream(request.PreviewPath, FileMode.Create))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        ms.Write(response.ReportResult.MainStream, 0, response.ReportResult.MainStream.Length);
                        ms.WriteTo(fs);
                    }
                }
                response.PreviewPath = $"/PDFViewer/{response.FileName}.pdf";
            }

            return response.ToResponse();
        }

        //ReportViewerCore.NETCore
        //https://github.com/lkosson/reportviewercore/blob/master/ReportViewerCore.Sample.AspNetCore
        //public ApplicationReportResponse GenerateReport2(ApplicationReportRequest request)
        //{
        //    ApplicationReportResponse response = new ApplicationReportResponse()
        //    {
        //        ReportRenderType = request.ReportRenderType,
        //        FileName = request.ReportType.ToString()
        //    };

        //    using var rs = Assembly.GetExecutingAssembly().GetManifestResourceStream("GCTL.UI.Core.Reports.Sales.rdlc");

        //    Microsoft.Reporting.NETCore.LocalReport report = new LocalReport();
        //    report.LoadReportDefinition(rs);
        //    foreach (var source in sources)
        //    {
        //        report.DataSources.Add(new ReportDataSource(source.Key, source.Value));
        //    }

        //    LocalReport report = new LocalReport(request.ReportPath);
        //    if (request.Sources != null && request.Sources.Any())
        //    {
        //        foreach (var source in request.Sources)
        //        {
        //            report.AddDataSource(source.Key, source.Value);
        //        }
        //    }

        //    var result = report.Execute(request.ReportRenderType.ToRenderType(), 1, request.Parameters);
        //    response.ReportResult = result;
        //    if (request.IsPreview)
        //    {
        //        using (FileStream fs = new FileStream(request.PreviewPath, FileMode.Create))
        //        {
        //            using (MemoryStream ms = new MemoryStream())
        //            {
        //                ms.Write(response.ReportResult.MainStream, 0, response.ReportResult.MainStream.Length);
        //                ms.WriteTo(fs);
        //            }
        //        }
        //        response.PreviewPath = $"/PDFViewer/{response.FileName}.pdf";
        //    }



        //    Stream reportDefinition; // your RDLC from file or resource
        //    IEnumerable dataSource; // your datasource for the report
        //    //using (FileStream fs = new FileStream(request.ReportPath, FileMode.Open))
        //    //{
        //    //    using (MemoryStream ms = new MemoryStream())
        //    //    {
        //    //        fs.CopyTo(ms);
        //    //        reportDefinition = ms;
        //    //    }
        //    //}
         

        //    // report.SetParameters(new[] { new ReportParameter("Parameter1", "Parameter value") });
        //    byte[] pdf = report.Render("PDF");


        //    return response.ToResponse();
        //}
    }
}
