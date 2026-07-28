
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HrmEmployeeSalaryInfoEntry;
using GCTL.Service.HrmEmployeeSalaryInfoReport;
using GCTL.UI.Core.ViewModels.HrmEmpSalaryInfoEntry;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class HrmEmployeeSalaryInfoReportController : BaseController
    {
        private readonly IHrmEmployeeSalaryInfoReportServices reportService;

        public HrmEmployeeSalaryInfoReportController(IHrmEmployeeSalaryInfoReportServices reportService)
        {
            this.reportService = reportService;
        }

        public async Task<IActionResult> Index()
        {
            var hasPermission = true;// await reportService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
                return RedirectToAction("Login", "Accounts");

            HrmEmpSalaryInfoEntryViewModel model = new HrmEmpSalaryInfoEntryViewModel()
            {
                PageUrl = Url.Action(nameof(Index)),
                AccessCode = LoginInfo.AccessCode
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetAllFilterEmp([FromBody] ReportFilterViewModel filterDto)
        {
            try
            {
                var result = await reportService.GetFilterDataAsync(filterDto);
                if (result != null)
                {
                    return Json(new { isSuccess = true, message = "Data loaded successfully", data = result });
                }
                return Json(new { isSuccess = false, message = "Data load failed" });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = "An error occurred while loading data" });
            }
        }

        [HttpPost("HrmEmployeeSalaryInfoReport/PreviewReport")]
        public async Task<IActionResult> PreviewReport([FromBody] ReportExportRequest request)
        {
            BaseViewModel model = new BaseViewModel();
            model.ToAudit(LoginInfo);

            request.FilterData.AccessCode = LoginInfo.AccessCode;
            request.FilterData.UserId = LoginInfo.EmployeeId;

            var data = await reportService.GetDataAsync(request.FilterData);
            if (data == null || !data.Any())
            {
                return NotFound();
            }
            var pdfBytes = await reportService.GeneratePdfReport(data, model);
            return File(pdfBytes, "application/pdf");
        }

        [HttpPost("HrmEmployeeSalaryInfoReport/ExportReport")]
        public async Task<IActionResult> ExportReport([FromBody] ReportExportRequest request)
        {

            try
            {
                BaseViewModel model = new BaseViewModel();
                model.ToAudit(LoginInfo);

                request.FilterData.AccessCode = LoginInfo.AccessCode;
                request.FilterData.UserId = LoginInfo.EmployeeId;
                var data = await reportService.GetDataAsync(request.FilterData);

                byte[] fileBytes;
                string fileName;
                string contentType;

                string baseFileName = $"EmployeeSalaryInfoReport_{DateTime.Now:yyyyMMdd_HHmmss}";

                switch (request.ExportFormat.ToLower())
                {
                    case "pdf":
                        fileBytes = await reportService.GeneratePdfReport(data, model);
                        fileName = $"{baseFileName}.pdf";
                        contentType = "application/pdf";
                        break;

                    case "excel":
                        fileBytes = await reportService.GenerateExcelReport(data);
                        fileName = $"{baseFileName}.xlsx";
                        contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        break;

                    case "word":
                        fileBytes = GenerateWordReport(data);
                        fileName = $"{baseFileName}.docx";
                        contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        break;

                    case "download":
                        fileBytes = await reportService.GeneratePdfReport(data, model);
                        fileName = $"{baseFileName}.pdf";
                        contentType = "application/pdf";
                        Response.Headers.Add("Content-Disposition", $"inline; filename=\"{fileName}\"");
                        return File(fileBytes, contentType);

                    default:
                        return BadRequest();

                }
                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        private byte[] GenerateWordReport(List<ReportFilterResultViewModel> employees)
        {
            using var stream = new MemoryStream();
            using var document = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document);

            var mainPart = document.AddMainDocumentPart();
            mainPart.Document = new Document();
            var body = new Body();

            var sectionProps = new SectionProperties(
                new PageSize { Width = 16838, Height = 11906, Orient = PageOrientationValues.Landscape },
                new PageMargin { Top = 518, Right = 518, Bottom = 518, Left = 518 }
            );

            TableCell CreateCell(string text, int width, bool bold = false, TableCellBorders borders = null, bool centerAlign = false, bool rightAlign = false)
            {
                var props = new TableCellProperties(
                    new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = width.ToString() }
                );

                if (borders != null)
                    props.Append(borders);

                var runProps = new RunProperties(
                    new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                    new FontSize() { Val = "18" }
                );

                if (bold)
                    runProps.Append(new Bold());

                var run = new Run(runProps, new Text(text));

                var paragraphProps = new ParagraphProperties(
                    new SpacingBetweenLines()
                    {
                        Before = "0",
                        After = "0",
                        Line = "240",
                        LineRule = LineSpacingRuleValues.Exact
                    }
                );

                if (centerAlign)
                    paragraphProps.Append(new Justification() { Val = JustificationValues.Center });
                else if (rightAlign)
                    paragraphProps.Append(new Justification() { Val = JustificationValues.Right });

                return new TableCell(props, new Paragraph(paragraphProps, run));
            }


            body.Append(
                new Paragraph(
                    new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Center },
                        new SpacingBetweenLines() { After = "240" } // Add space after header
                    ),
                    new Run(new RunProperties(new Bold(), new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" }, new FontSize() { Val = "28" }), new Text(employees.Select(e => e.CompanyName).Distinct().FirstOrDefault())), // 14pt
                    new Run(new Break()),
                    new Run(new RunProperties(new Bold(), new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" }, new FontSize() { Val = "24" }), new Text("Employee Salary Info Report")) // 12pt
                )
            );

            var departmentGroupedData = employees
                .GroupBy(x => x.DepartmentName ?? "Unknown Department")
                .OrderBy(x => x.Key);

            var uniqueEmployees = new HashSet<String>();
            decimal grandTotal = 0;
            int totalEmployees = 0;

            foreach (var departmentGroup in departmentGroupedData)
            {
                body.Append(
                    new Paragraph(
                        new ParagraphProperties(
                            new SpacingBetweenLines() { Before = "100", After = "50" }
                        ),
                        new Run(new RunProperties(new Bold(), new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" }, new FontSize() { Val = "20" }), new Text($"Department: {departmentGroup.Key}"))
                    )
                );

                var table = new Table();

                var tableProps = new TableProperties(
                    new TableWidth() { Width = "15120", Type = TableWidthUnitValues.Dxa },
                    new TableBorders(
                        new TopBorder() { Val = BorderValues.Single, Size = 4, Color = "D3D3D3" },
                        new BottomBorder() { Val = BorderValues.Single, Size = 4, Color = "D3D3D3" },
                        new LeftBorder() { Val = BorderValues.Single, Size = 4, Color = "D3D3D3" },
                        new RightBorder() { Val = BorderValues.Single, Size = 4, Color = "D3D3D3" },
                        new InsideHorizontalBorder() { Val = BorderValues.Single, Size = 4, Color = "D3D3D3" },
                        new InsideVerticalBorder() { Val = BorderValues.Single, Size = 4, Color = "D3D3D3" }
                    )
                );

                table.AppendChild(tableProps);

                string[] headers = { "SL", "Employee ID", "Pay ID", "Name", "Designation", "Employee Type", "Employee Nature", "Joining Date", "Last Inc. Date", "Gross Salary", "Mode of Payment" };

                int[] columnWidths = new int[]
                {
                    526,
                    1548,
                    726,
                    3254,
                    3254,
                    1148,
                    1375,
                    1375,
                    1375,
                    1322,
                    1322
                };


                var headerRow = new TableRow();

                for (int i = 0; i < headers.Length; i++)
                {
                    var borders = new TableCellBorders(
                        new TopBorder() { Val = BorderValues.Single, Size = 4, Color = "D3D3D3" },
                        new BottomBorder() { Val = BorderValues.Single, Size = 4, Color = "D3D3D3" },
                        new LeftBorder() { Val = BorderValues.Single, Size = 4, Color = "D3D3D3" },
                        new RightBorder() { Val = BorderValues.Single, Size = 4, Color = "D3D3D3" }
                    );
                    headerRow.Append(CreateCell(headers[i], columnWidths[i], bold: true, borders, centerAlign: true));
                }

                table.Append(headerRow);

                int sn = 1;
                decimal departmentTotal = 0;

                var orderItems = departmentGroup.OrderBy(x => x.Code).ToList();

                foreach (var item in orderItems)
                {
                    var gSalary = item.GrossSalary ?? 0.00m;

                    departmentTotal += gSalary;

                    if (!string.IsNullOrEmpty(item.Code))
                        uniqueEmployees.Add(item.Code);

                    var values = new[]
                    {
                        sn.ToString(),
                        item.Code??"",
                        item.PayId?? "",
                        item.Name?? "",
                        item.DesignationName?? "",
                        item.EmployeeTypeName?? "",
                        item.EmploymentNature??"",
                        item.JoiningDate?? "",
                        item.LastIncDate?? "",
                        item.GrossSalary.Value.ToString("G29") ?? "",
                        item.DisbursementMethodName??""
                    };

                    var dataRow = new TableRow();

                    for (int i = 0; i < values.Length; i++)
                    {
                        bool centerAlign = i != 2 && i != 3 && i != 4;
                        bool rightAlign = i == 6;

                        var borders = new TableCellBorders(
                            new TopBorder() { Val = BorderValues.Single, Size = 4, Color = "D3D3D3" },
                            new BottomBorder() { Val = BorderValues.Single, Size = 4, Color = "D3D3D3" },
                            new LeftBorder() { Val = BorderValues.Single, Size = 4, Color = "D3D3D3" },
                            new RightBorder() { Val = BorderValues.Single, Size = 4, Color = "D3D3D3" }
                        );

                        dataRow.Append(CreateCell(values[i], columnWidths[i], false, borders, centerAlign, rightAlign));
                    }

                    table.Append(dataRow);
                    sn++;
                }

                var totalRow = new TableRow();
                for (int i = 0; i < headers.Length; i++)
                {

                    string cellText = "";
                    bool bold = false;
                    bool rightAlign = false;

                    if (i == 8)
                    {
                        cellText = $"Total: ";
                        bold = true;
                    }
                    if (i == 9)
                    {
                        cellText = $"{departmentTotal.ToString("G29")}";
                        bold = true;
                    }

                    var borders = new TableCellBorders(
                        new TopBorder() { Val = BorderValues.None, Size = 0 },
                        new BottomBorder() { Val = BorderValues.None, Size = 0 },
                        new LeftBorder() { Val = BorderValues.None, Size = 0 },
                        new RightBorder() { Val = BorderValues.None, Size = 0 }
                    );

                    totalRow.Append(CreateCell(cellText, columnWidths[i], bold, null, false, rightAlign));
                }

                table.Append(totalRow);
                body.Append(table);

                grandTotal += departmentTotal;
                totalEmployees += uniqueEmployees.Count;
            }
            body.Append(new Paragraph(new Run()));
            var summaryTable = new Table();
            var summaryProps = new TableProperties(
                new TableWidth() { Width = "15120", Type = TableWidthUnitValues.Dxa },
                new TableBorders(
                    new TopBorder() { Val = BorderValues.None },
                    new BottomBorder() { Val = BorderValues.None },
                    new LeftBorder() { Val = BorderValues.None },
                    new RightBorder() { Val = BorderValues.None },
                    new InsideHorizontalBorder() { Val = BorderValues.None },
                    new InsideVerticalBorder() { Val = BorderValues.None }
                )
            );
            summaryTable.Append(summaryProps);

            var summaryRow = new TableRow();
            summaryRow.Append(
                CreateCell($"No. of Employee: {totalEmployees}", 5040, false, null, centerAlign: false),
                CreateCell($"Grand Total: {grandTotal:N2}", 5040, false, null, rightAlign: true)
            );
            summaryTable.Append(summaryRow);

            body.Append(summaryTable);
            body.Append(sectionProps);

            mainPart.Document.Append(body);
            mainPart.Document.Save();
            document.Dispose();

            return stream.ToArray();
        }



        //[HttpPost("company")]
        //public async Task<IActionResult> Company([FromBody] GcFilterRequestDto req)
        //    => Json(new { isSuccess = true, data = await _svc.GetCompaniesAsync(req) });

        //[HttpPost("branch")]
        //public async Task<IActionResult> Branch([FromBody] GcFilterRequestDto req)
        //    => Json(new { isSuccess = true, data = await _svc.GetBranchesAsync(req) });

        //[HttpPost("division")]
        //public async Task<IActionResult> Division([FromBody] GcFilterRequestDto req)
        //    => Json(new { isSuccess = true, data = await _svc.GetDivisionsAsync(req) });

        //[HttpPost("department")]
        //public async Task<IActionResult> Department([FromBody] GcFilterRequestDto req)
        //    => Json(new { isSuccess = true, data = await _svc.GetDepartmentsAsync(req) });

        //[HttpPost("designation")]
        //public async Task<IActionResult> Designation([FromBody] GcFilterRequestDto req)
        //    => Json(new { isSuccess = true, data = await _svc.GetDesignationsAsync(req) });

        //[HttpPost("SalaryInfo/Employee")]
        //public async Task<IActionResult> Employee([FromBody] ReportFilterViewModel req)
        //    => Json(new { isSuccess = true, data = await reportService.GetEmployeesAsync(req) });

        //[HttpPost]
        //public async Task<IActionResult> GetDropdownPaged([FromBody] DropdownPagedRequestViewModel request)
        //{
        //    try
        //    {
        //        if (request is null || string.IsNullOrWhiteSpace(request.DropdownField))
        //            return Json(new { isSuccess = false, message = "Invalid request." });

        //        var result = await reportService.GetDropdownPagedAsync(request);

        //        return Json(new
        //        {
        //            isSuccess = true,
        //            message = "Data loaded successfully.",
        //            data = result.Items,
        //            hasMore = result.HasMore,
        //            totalCount = result.TotalCount,
        //            page = result.Page,
        //            pageSize = result.PageSize
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log ex as appropriate for your project
        //        return Json(new { isSuccess = false, message = "An error occurred while loading dropdown data." });
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> GetDropdownPage([FromBody] ReportDropdownPageRequest request)
        //{
        //    try
        //    {
        //        var result = await reportService.GetDropdownPageAsync2(request);
        //        return Json(new { isSuccess = true, data = result });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { isSuccess = false, message = "Failed to load dropdown data" });
        //    }
        //}


        //[HttpPost("HrmEmployeeSalaryInfoReport/employee")]
        //public async Task<IActionResult> Employee([FromBody] ReportFilterViewModel req)
        //    => Json(new { isSuccess = true, data = await reportService.GetEmployeesAsync(req) });

    }
}
