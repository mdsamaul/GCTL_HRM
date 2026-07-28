using Dapper;
using GCTL.Core.Data;
using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.EachGcFilterRequest;
using GCTL.Core.ViewModels.HrmEmployeeSalaryInfoEntry;
using GCTL.Data.Models;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Data;
using System.Globalization;

using PdfDocument2 = iText.Layout.Document;

namespace GCTL.Service.HrmEmployeeSalaryInfoReport
{
    public class HrmEmployeeSalaryInfoReportService : AppService<HrmEmployeeOfficialInfo>, IHrmEmployeeSalaryInfoReportServices
    {
        private readonly IRepository<HrmEmployee> employeeRepo;
        private readonly IRepository<HrmEmployeeOfficialInfo> empOffRepo;
        private readonly IRepository<HrmDefDesignation> desiRepo;
        private readonly IRepository<HrmDefDepartment> depRepo;
        private readonly IRepository<CoreBranch> branchRepo;
        private readonly IRepository<CoreCompany> companyRepo;
        private readonly IRepository<HrmDefEmployeeStatus> empStatusRepo;
        private readonly IRepository<HrmDefEmpType> empTypeRepo;
        private readonly IRepository<HrmEisDefEmploymentNature> empNatureRepo;
        private readonly IRepository<HrmSeparation> sepRepo;
        private readonly IRepository<HrmEisDefDisbursementMethod> disbursementRepo;
        private readonly IRepository<HrmIncrement> incRepo;
        private readonly IRepository<CoreAccessCode> accessCodeRepo;
        private readonly string _configuration;


        public HrmEmployeeSalaryInfoReportService(
            IRepository<HrmEmployee> employeeRepo,
            IRepository<HrmEmployeeOfficialInfo> empOffRepo,
            IRepository<HrmDefDesignation> desiRepo,
            IRepository<HrmDefDepartment> depRepo,
            IRepository<CoreBranch> branchRepo,
            IRepository<CoreCompany> companyRepo,
            IRepository<HrmDefEmployeeStatus> empStatusRepo,
            IRepository<HrmDefEmpType> empTypeRepo,
            IRepository<HrmEisDefEmploymentNature> empNatureRepo,
            IRepository<HrmSeparation> sepRepo,
            IRepository<HrmEisDefDisbursementMethod> disbursementRepo,
            IRepository<HrmIncrement> incRepo,
            IRepository<CoreAccessCode> accessCodeRepo,
            IConfiguration configuration) : base(empOffRepo)
        {
            this.employeeRepo = employeeRepo;
            this.empOffRepo = empOffRepo;
            this.desiRepo = desiRepo;
            this.depRepo = depRepo;
            this.branchRepo = branchRepo;
            this.companyRepo = companyRepo;
            this.empStatusRepo = empStatusRepo;
            this.empTypeRepo = empTypeRepo;
            this.empNatureRepo = empNatureRepo;
            this.sepRepo = sepRepo;
            this.disbursementRepo = disbursementRepo;
            this.incRepo = incRepo;
            this.accessCodeRepo = accessCodeRepo;
            _configuration = configuration.GetConnectionString("ApplicationDbConnection");
        }

        public async Task<byte[]> GenerateExcelReport(List<ReportFilterResultViewModel> data)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();

            var worksheet = package.Workbook.Worksheets.Add("Employee Salary Info Report");

            worksheet.Cells["A1:K1"].Merge = true;
            worksheet.Cells["A1"].Value = data.Select(c => c.CompanyName);
            worksheet.Cells["A1"].Style.Font.Size = 16;
            worksheet.Cells["A1"].Style.Font.Bold = true;
            worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells["A2:K2"].Merge = true; 
            worksheet.Cells["A2"].Value = "Employee Salary Info Report";
            worksheet.Cells["A2"].Style.Font.Size = 16;
            worksheet.Cells["A2"].Style.Font.Bold = true;
            worksheet.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            int currentRow = 4;
            var groupedData = data.GroupBy(x => x.DepartmentName ?? "Unknown Department")
                                  .OrderBy(g => g.Key);

            decimal grandTotal = 0;
            int totalEmployees = 0;

            foreach (var departmentGroup in groupedData)
            {
                worksheet.Cells[currentRow, 1, currentRow, 8].Merge = true;
                worksheet.Cells[currentRow, 1].Value = $"Department: {departmentGroup.Key}";
                worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                worksheet.Cells[currentRow, 1].Style.Font.Size = 12;
                currentRow++;

                string[] headers = { "SL", "Employee ID", "Pay ID", "Name", "Designation", "Employee Type", "Employee Nature", "Joining Date", "Last Inc. Date", "Gross Salary", "Mode of Payment" };

                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = worksheet.Cells[currentRow, i + 1];
                    cell.Value = headers[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                currentRow++;

                int serialNo = 1;
                decimal departmentTotal = 0;
                var uniqueEmployees = new HashSet<string>();

                foreach (var item in departmentGroup.OrderBy(x => x.Code))
                {
                    var GrossSalary = item.GrossSalary ?? 0;
                    departmentTotal += GrossSalary;

                    if (!string.IsNullOrEmpty(item.Code))
                        uniqueEmployees.Add(item.Code);

                    worksheet.Cells[currentRow, 1].Value = serialNo;
                    worksheet.Cells[currentRow, 2].Value = item.Code;
                    worksheet.Cells[currentRow, 3].Value = item.PayId;
                    worksheet.Cells[currentRow, 4].Value = item.Name;
                    worksheet.Cells[currentRow, 5].Value = item.DesignationName;
                    worksheet.Cells[currentRow, 6].Value = item.EmployeeTypeName;
                    worksheet.Cells[currentRow, 7].Value = item.EmploymentNature;
                    worksheet.Cells[currentRow, 8].Value = item.JoiningDate;
                    worksheet.Cells[currentRow, 9].Value = item.LastIncDate;
                    worksheet.Cells[currentRow, 10].Value = item.GrossSalary;
                    worksheet.Cells[currentRow, 11].Value = item.DisbursementMethodName;

                    for (int i = 1; i <= 11; i++)
                    {
                        var cell = worksheet.Cells[currentRow, i];
                        cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        ExcelHorizontalAlignment alignment = i switch
                        {
                            4 or 5 => ExcelHorizontalAlignment.Left,
                            10 => ExcelHorizontalAlignment.Right,
                            _ => ExcelHorizontalAlignment.Center
                        };
                        cell.Style.HorizontalAlignment = alignment;
                    }

                    currentRow++;
                    serialNo++;
                }
                worksheet.Cells[currentRow, 9].Value = $"Total: ";
                worksheet.Cells[currentRow, 9].Style.Font.Bold = true;

                worksheet.Cells[currentRow, 10].Value = $"{departmentTotal}";
                worksheet.Cells[currentRow, 10].Style.Font.Bold = true;
                //worksheet.Cells[currentRow, 10].Style.Numberformat.Format = "#,##0.00";

                currentRow += 2;

                grandTotal += departmentTotal;
                totalEmployees += uniqueEmployees.Count;
            }

            worksheet.Cells[currentRow, 2].Value = $"Total Employees: {totalEmployees}";
            worksheet.Cells[currentRow, 2].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 2].Style.Numberformat.Format = "#,##0.00";

            worksheet.Cells[currentRow, 9].Value = $"Grand Total: ";
            worksheet.Cells[currentRow, 9].Style.Font.Bold = true;

            worksheet.Cells[currentRow, 10].Value = $"{grandTotal}";
            worksheet.Cells[currentRow, 10].Style.Font.Bold = true;
            //worksheet.Cells[currentRow, 10].Style.Numberformat.Format = "#,##0.00";

            currentRow++;

            worksheet.Cells.AutoFitColumns();

            return await package.GetAsByteArrayAsync();
        }

        public async Task<byte[]> GeneratePdfReport(List<ReportFilterResultViewModel> data, BaseViewModel model)
        {
            if (data == null || !data.Any())
                return null;

            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            using var document = new PdfDocument2(pdf, iText.Kernel.Geom.PageSize.A4.Rotate());

            string logoPath = "wwwroot/images/DP_logo.png";
            var imageData = ImageDataFactory.Create(logoPath);
            var logo = new Image(imageData);

            document.SetMargins(60, 36, 40, 36);

            var boldFont = PdfFontFactory.CreateFont(StandardFonts.TIMES_BOLD);
            var regularFont = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);

            var groupData = data
                .GroupBy(x => x.DepartmentName ?? "Unknown Department")
                .OrderBy(g => g.Key);

            decimal grandTotal = 0;
            int totalEmployee = data.Select(e => e.Code).Distinct().Count();

            //bool isFirst = true;

            bool NeedsPageBreak(iText.Layout.Document doc)
            {
                try
                {
                    var renderer = doc.GetRenderer();
                    var currentArea = renderer.GetCurrentArea();

                    if (currentArea?.GetBBox() != null)
                    {
                        float availableHeight = currentArea.GetBBox().GetHeight();
                        float pageHeight = doc.GetPdfDocument().GetDefaultPageSize().GetHeight();
                        float totalMargins = doc.GetTopMargin() + doc.GetBottomMargin();
                        float fullPageContentHeight = pageHeight - totalMargins;

                        return availableHeight < (fullPageContentHeight * 0.30f);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error checking page break: {ex.Message}");
                }
                return false;
            }

            float[] columnWidths = new float[]
            {
                3.8f, 9.4f, 3.8f, 19.8f, 19.8f, 7f, 8.4f, 8.4f, 8.4f, 8f, 8f
            };

            string[] headers = { "SN", "Employee ID", "Pay ID", "Name", "Designation", "Employee Type", "Employee Nature", "Joining Date", "Last Inc. Date", "Gross Salary", "Mode of Payment" };

            foreach (var dpGroup in groupData)
            {
                if (NeedsPageBreak(document))
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));



                var table = new Table(UnitValue.CreatePercentArray(columnWidths));
                table.SetWidth(UnitValue.CreatePercentValue(100));



                var departmentHeaderRow = new Cell(1, 11)
                    .Add(new Paragraph($"Department: {dpGroup.Key}"))
                    .SetFontSize(11).SetTextAlignment(TextAlignment.LEFT)
                    .SetPadding(5).SetFont(PdfFontFactory.CreateFont(StandardFonts.TIMES_BOLD))
                    .SetBorderTop(iText.Layout.Borders.Border.NO_BORDER)
                    .SetBorderLeft(iText.Layout.Borders.Border.NO_BORDER)
                    .SetBorderRight(iText.Layout.Borders.Border.NO_BORDER)
                    .SetBorderBottom(iText.Layout.Borders.Border.NO_BORDER);

                table.AddHeaderCell(departmentHeaderRow);

                foreach (var header in headers)
                {
                    var headerCell = new Cell()
                        .Add(new Paragraph(header).SetFont(PdfFontFactory.CreateFont(StandardFonts.TIMES_BOLD))
                        .SetFontSize(7)).SetTextAlignment(TextAlignment.CENTER)
                        .SetBorder(new iText.Layout.Borders.SolidBorder(ColorConstants.LIGHT_GRAY, 0.2f));
                    table.AddHeaderCell(headerCell);
                }

                table.SetSkipFirstHeader(false);
                table.SetSkipLastFooter(false);

                int serialNo = 1;
                decimal departmentTotal = 0;
                var uniqueEmployees = new HashSet<string>();
                var orderedItems = dpGroup.OrderBy(x => x.Code).ToList();

                foreach (var item in orderedItems)
                {
                    var grossSalary = item.GrossSalary ?? 0;
                    departmentTotal += grossSalary;

                    var values = new[]
                    {
                        serialNo.ToString(),
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


                    for (int i = 0; i < values.Length; i++)
                    {
                        TextAlignment alignment = i switch
                        {
                            3 or 4 => TextAlignment.LEFT,
                            9 => TextAlignment.RIGHT,
                            _ => TextAlignment.CENTER
                        };

                        var cell = new Cell().Add(new Paragraph(values[i]))
                            .SetFontSize(8)
                            .SetFont(PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN))
                            .SetTextAlignment(alignment)
                            .SetBorder(new iText.Layout.Borders.SolidBorder(ColorConstants.LIGHT_GRAY, 0.2f));


                        table.AddCell(cell);

                    }
                    serialNo++;
                }

                for (int i = 0; i < 8; i++)
                {
                    var emptyCell = new Cell().Add(new Paragraph(""))
                        .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                        .SetFontSize(7);
                    table.AddCell(emptyCell);
                }

                var totalLabelCell = new Cell().Add(new Paragraph($"Total: "))
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                    .SetFontSize(8);
                table.AddCell(totalLabelCell);

                var amountCell = new Cell().Add(new Paragraph($"{departmentTotal.ToString("G29")}"))
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                    .SetFontSize(8);
                table.AddCell(amountCell);

                for (int i = 0; i < 1; i++)
                {
                    var emptyCell = new Cell().Add(new Paragraph(""))
                        .SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetFontSize(8);
                    table.AddCell(emptyCell);
                }
                document.Add(table);

                grandTotal += departmentTotal;
            }

            var summaryTable = new Table(new float[] { 1, 1, 1, 1 });
            summaryTable.SetWidth(UnitValue.CreatePercentValue(100)).SetMarginTop(20);

            summaryTable.AddCell(new Cell().Add(new Paragraph($"Total Employees: {totalEmployee}"))
              .SetTextAlignment(TextAlignment.LEFT)
              .SetBorder(iText.Layout.Borders.Border.NO_BORDER));

            summaryTable.AddCell(new Cell().Add(new Paragraph($"Grand Total : {grandTotal:N2}"))
              .SetTextAlignment(TextAlignment.RIGHT)
              .SetBorder(iText.Layout.Borders.Border.NO_BORDER));

            document.Add(summaryTable);

            document.Close();

            using var readerStream = new MemoryStream(stream.ToArray());
            using var reader = new PdfReader(readerStream);
            using var outputStream = new MemoryStream();
            using var outputWriter = new PdfWriter(outputStream);
            using var outputPdf = new PdfDocument(reader, outputWriter);

            var totalPages = outputPdf.GetNumberOfPages();
            for (int i = 1; i <= totalPages; i++)
            {
                var page = outputPdf.GetPage(i);
                var pageSize = page.GetPageSize();
                var canvas = new PdfCanvas(page);

                try
                {
                    float logoWidth = 90;
                    float logoHeight = 25;
                    float logoX = 30; // Left margin
                    float logoY = pageSize.GetTop() - 35;

                    logo.SetFixedPosition(i, logoX, logoY);
                    logo.ScaleAbsolute(logoWidth, logoHeight);

                    var logoCanvas = new Canvas(canvas, pageSize);
                    logoCanvas.Add(logo);
                    logoCanvas.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not load logo: {ex.Message}");
                }

                float headerStartX = pageSize.GetWidth() / 2;
                float headerTopY = pageSize.GetTop() - 25;

                var companyFont = PdfFontFactory.CreateFont(StandardFonts.TIMES_BOLD);
                float companyFontSize = 14;
                string companyText = data.Select(c => c.CompanyName).Distinct().FirstOrDefault();
                float companyTextWidth = companyFont.GetWidth(companyText, companyFontSize);

                canvas.BeginText()
                    .SetFontAndSize(companyFont, companyFontSize)
                    .MoveText(headerStartX - (companyTextWidth / 2), headerTopY)
                    .ShowText(companyText)
                    .EndText();

                var reportFont = PdfFontFactory.CreateFont(StandardFonts.TIMES_BOLD);
                float reportFontSize = 12;
                string reportText = "Employee Salary Info Report";
                float reportTextWidth = reportFont.GetWidth(reportText, reportFontSize);

                canvas.BeginText()
                    .SetFontAndSize(reportFont, reportFontSize)
                    .MoveText(headerStartX - (reportTextWidth / 2), headerTopY - 18)
                    .ShowText(reportText)
                    .EndText();

                var font = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);
                float fontSize = 8;
                float margin = 36;
                float yPosition = pageSize.GetBottom() + 20;
                float pageWidth = pageSize.GetWidth();

                canvas.BeginText()
                    .SetFontAndSize(font, fontSize)
                    .MoveText(margin, yPosition)
                    .ShowText($"Print Datetime: {(model.Ldate.HasValue ? model.Ldate.Value.ToString("dd/MM/yyyy hh:mm:ss tt") : DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"))}")
                    .EndText();

                string userText = $"GCTL Infosys - HRM & Finance System";
                float userTextWidth = font.GetWidth(userText, fontSize);
                canvas.BeginText()
                    .SetFontAndSize(font, fontSize)
                    .SetFillColor(ColorConstants.GRAY)
                    .MoveText((pageWidth - userTextWidth) / 2, yPosition)
                    .ShowText(userText)
                    .EndText();

                string pageText = $"Page {i} of {totalPages}";
                float pageTextWidth = font.GetWidth(pageText, fontSize);
                canvas.BeginText()
                    .SetFontAndSize(font, fontSize)
                    .MoveText(pageWidth - margin - pageTextWidth, yPosition)
                    .ShowText(pageText)
                    .EndText();

                canvas.Release();
            }

            outputPdf.Close();
            return outputStream.ToArray();
        }

        public async Task<ReportFilterListViewModel> GetFilterDataAsync(ReportFilterViewModel filter)
        {
            var empStatus = await empStatusRepo.All().AsNoTracking().Where(x => x.EmployeeStatusId != null && x.EmployeeStatus != null)
                .OrderBy(x => x.EmployeeStatusId)
                .Select(x => new ReportFilterResultViewModel
                {
                    Code = x.EmployeeStatusId,
                    Name = x.EmployeeStatus
                })
                .Distinct().ToListAsync();

            var empNatures = await empNatureRepo.All().AsNoTracking().Where(x => x.EmploymentNatureId != null && x.EmploymentNature != null)
                .OrderBy(x => x.EmploymentNatureId)
                .Select(x => new ReportFilterResultViewModel
                {
                    Code = x.EmploymentNatureId,
                    Name = x.EmploymentNature
                })
                .Distinct().ToListAsync();

            var empTypes = await empTypeRepo.All().AsNoTracking().Where(x => x.EmpTypeCode != null && x.EmpTypeName != null)
                .OrderBy(x => x.EmpTypeCode)
                .Select(x => new ReportFilterResultViewModel
                {
                    Code = x.EmpTypeCode,
                    Name = x.EmpTypeName
                })
                .Distinct().ToListAsync();

            var result = new ReportFilterListViewModel
            {
                EmpStatuses = empStatus,

                EmpNatures = empNatures,

                EmpTypes = empTypes
            };

            return result;
        }
        public async Task<List<ReportFilterResultViewModel>> GetDataAsync(ReportFilterViewModel filter)
        {
            var query = from e in empOffRepo.All().AsNoTracking()
                        join emp in employeeRepo.All().AsNoTracking() on e.EmployeeId equals emp.EmployeeId
                        join c in companyRepo.All().AsNoTracking() on e.CompanyCode equals c.CompanyCode
                        join b in branchRepo.All().AsNoTracking() on e.BranchCode equals b.BranchCode into branchGroup
                        from b in branchGroup.DefaultIfEmpty()
                        join d in depRepo.All().AsNoTracking() on e.DepartmentCode equals d.DepartmentCode into deptGroup
                        from d in deptGroup.DefaultIfEmpty()
                        join ds in desiRepo.All().AsNoTracking() on e.DesignationCode equals ds.DesignationCode into desigGroup
                        from ds in desigGroup.DefaultIfEmpty()
                        join status in empStatusRepo.All().AsNoTracking() on e.EmployeeStatus equals status.EmployeeStatusId into statusGroup
                        from status in statusGroup.DefaultIfEmpty()
                        join emptype in empTypeRepo.All().AsNoTracking() on e.EmpTypeCode equals emptype.EmpTypeCode into empTypeGroup
                        from emptype in empTypeGroup.DefaultIfEmpty()
                        join empNature in empNatureRepo.All().AsNoTracking() on e.EmploymentNatureId equals empNature.EmploymentNatureId into empNatureGroup
                        from empNature in empNatureGroup.DefaultIfEmpty()
                        join des in disbursementRepo.All().AsNoTracking() on e.DisbursementMethodId equals des.DisbursementMethodId into disbursGroup
                        from des in disbursGroup.DefaultIfEmpty()
                        join sep in sepRepo.All().AsNoTracking() on e.EmployeeId equals sep.EmployeeId into separationGroup
                        from sep in separationGroup.DefaultIfEmpty()
                        let latestInc = incRepo.All()
                            .AsNoTracking()
                            .Where(x => x.EmployeeId == e.EmployeeId)
                            .OrderByDescending(x => x.Wef)
                            .FirstOrDefault()
                        select new
                        {
                            EmployeeId = e.EmployeeId,
                            PayId = e.PayId,
                            FirstName = emp.FirstName,
                            LastName = emp.LastName,
                            e.BranchCode,
                            BranchName = b != null ? b.BranchName : null,
                            e.CompanyCode,
                            CompanyName = c.CompanyName,
                            e.DesignationCode,
                            DesignationName = ds != null ? ds.DesignationName : null,
                            e.DepartmentCode,
                            DepartmentName = d != null ? d.DepartmentName : null,
                            e.EmpTypeCode,
                            EmpTypename = emptype != null ? emptype.EmpTypeName : null,
                            e.EmployeeStatus,
                            EmployeeStatusName = status != null ? status.EmployeeStatus : null,
                            e.EmploymentNatureId,
                            EmpNatureName = empNature != null ? empNature.EmploymentNature : null,
                            e.JoiningDate,
                            SeparationDate = sep == null ? (DateTime?)null : sep.SeparationDate,
                            e.GrossSalary,
                            e.DisbursementMethodId,
                            e.ReportingTo,
                            e.Hod,
                            DisbursementName = des != null ? des.DisbursementMethod : null,
                            LastIncrementWEF = latestInc != null ? (DateTime?)latestInc.Wef : null
                        };

            var accessCode = filter.AccessCode;
            var loggedInEmployeeId = filter.UserId;

            var isAdmin = accessCode == "0001";
            var isTeamLeader = accessCode == "0002" || accessCode == "0003";
            var isUser = accessCode == "0005";

            if (!isAdmin)
            {
                if (isTeamLeader)
                {
                    query = query.Where(x =>
                        x.EmployeeId == loggedInEmployeeId
                        || x.ReportingTo == loggedInEmployeeId
                        || x.Hod == loggedInEmployeeId);
                }
                else if (isUser)
                {
                    query = query.Where(x => x.EmployeeId == loggedInEmployeeId);
                }
                else
                {
                    query = query.Where(x => false);
                }
            }

            if (filter.CompanyCodes?.Any() == true)
                query = query.Where(x => x.CompanyCode != null && filter.CompanyCodes.Contains(x.CompanyCode));

            if (filter.BranchCodes?.Any() == true)
                query = query.Where(x => x.BranchCode != null && filter.BranchCodes.Contains(x.BranchCode));

            if (filter.DepartmentCodes?.Any() == true)
                query = query.Where(x => x.DepartmentCode != null && filter.DepartmentCodes.Contains(x.DepartmentCode));

            if (filter.DesignationCodes?.Any() == true)
                query = query.Where(x => x.DesignationCode != null && filter.DesignationCodes.Contains(x.DesignationCode));

            if (filter.EmployeeIDs?.Any() == true)
                query = query.Where(x => x.EmployeeId != null && filter.EmployeeIDs.Contains(x.EmployeeId));

            if (filter.EmpStatuses?.Any() == true)
                query = query.Where(x => x.EmployeeStatus != null && filter.EmpStatuses.Contains(x.EmployeeStatus));

            if (filter.EmpTypes?.Any() == true)
                query = query.Where(x => x.EmpTypeCode != null && filter.EmpTypes.Contains(x.EmpTypeCode));

            if (filter.EmpNatures?.Any() == true)
                query = query.Where(x => x.EmploymentNatureId != null && filter.EmpNatures.Contains(x.EmploymentNatureId));

            if (!string.IsNullOrWhiteSpace(filter.JoiningDateFrom) &&
    DateTime.TryParseExact(filter.JoiningDateFrom, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fromDate))
            {
                query = query.Where(x => x.JoiningDate.HasValue && x.JoiningDate.Value.Date >= fromDate.Date);
            }

            if (!string.IsNullOrWhiteSpace(filter.JoiningDateTo) &&
                DateTime.TryParseExact(filter.JoiningDateTo, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var toDate))
            {
                query = query.Where(x => x.JoiningDate.HasValue && x.JoiningDate.Value.Date <= toDate.Date);
            }

            var result = await query.Where(x => x.EmployeeId != null)
                .Select(x => new ReportFilterResultViewModel
                {
                    Code = x.EmployeeId,
                    Name = (x.FirstName ?? "") + " " + (x.LastName ?? ""),
                    PayId = x.PayId,
                    CompanyName = x.CompanyName,
                    DesignationName = x.DesignationName,
                    DepartmentName = x.DepartmentName,
                    EmployeeTypeName = x.EmpTypename,
                    EmploymentNature = x.EmpNatureName,
                    JoiningDate = x.JoiningDate.HasValue ? x.JoiningDate.Value.ToString("dd/MM/yyyy") : "",
                    SeparationDate = x.SeparationDate.HasValue ? x.SeparationDate.Value.ToString("dd/MM/yyyy") : "",
                    LastIncDate = x.LastIncrementWEF.HasValue ? x.LastIncrementWEF.Value.ToString("dd/MM/yyyy") : "",
                    GrossSalary = x.GrossSalary,
                    DisbursementMethodName = x.DisbursementName
                }).ToListAsync();

            return result;
        }

        public async Task<DropdownPagedResultViewModel> GetDropdownPagedAsync(DropdownPagedRequestViewModel request)
        {
            var filter = request.FilterData ?? new ReportFilterViewModel();
            var field = (request.DropdownField ?? "").Trim().ToLowerInvariant();
            var page = Math.Max(1, request.Page);
            var pageSize = Math.Clamp(request.PageSize, 1, 200);
            var search = (request.Search ?? "").Trim();

            // ── Base joined query (identical to GetDataAsync) ──────────────────────
            var query = from e in empOffRepo.All().AsNoTracking()
                        join emp in employeeRepo.All().AsNoTracking()
                            on e.EmployeeId equals emp.EmployeeId
                        join c in companyRepo.All().AsNoTracking()
                            on e.CompanyCode equals c.CompanyCode
                        join b in branchRepo.All().AsNoTracking()
                            on e.BranchCode equals b.BranchCode into branchGroup
                        from b in branchGroup.DefaultIfEmpty()
                        join d in depRepo.All().AsNoTracking()
                            on e.DepartmentCode equals d.DepartmentCode into deptGroup
                        from d in deptGroup.DefaultIfEmpty()
                        join ds in desiRepo.All().AsNoTracking()
                            on e.DesignationCode equals ds.DesignationCode into desigGroup
                        from ds in desigGroup.DefaultIfEmpty()
                        join status in empStatusRepo.All().AsNoTracking()
                            on e.EmployeeStatus equals status.EmployeeStatusId into statusGroup
                        from status in statusGroup.DefaultIfEmpty()
                        join emptype in empTypeRepo.All().AsNoTracking()
                            on e.EmpTypeCode equals emptype.EmpTypeCode into empTypeGroup
                        from emptype in empTypeGroup.DefaultIfEmpty()
                        join empNature in empNatureRepo.All().AsNoTracking()
                            on e.EmploymentNatureId equals empNature.EmploymentNatureId into empNatureGroup
                        from empNature in empNatureGroup.DefaultIfEmpty()
                        select new
                        {
                            e.EmployeeId,
                            e.PayId,
                            FirstName = emp.FirstName,
                            LastName = emp.LastName,
                            e.BranchCode,
                            BranchName = b != null ? b.BranchName : null,
                            e.CompanyCode,
                            CompanyName = c.CompanyName,
                            e.DesignationCode,
                            DesignationName = ds != null ? ds.DesignationName : null,
                            e.DepartmentCode,
                            DepartmentName = d != null ? d.DepartmentName : null,
                            e.EmpTypeCode,
                            EmpTypeName = emptype != null ? emptype.EmpTypeName : null,
                            e.EmployeeStatus,
                            e.EmploymentNatureId,
                            EmpNatureName = empNature != null ? empNature.EmploymentNature : null,
                            e.JoiningDate
                        };

            // ── Cascade filters (same predicates as GetDataAsync) ─────────────────
            if (filter.CompanyCodes?.Any() == true)
                query = query.Where(x => x.CompanyCode != null && filter.CompanyCodes.Contains(x.CompanyCode));

            if (filter.BranchCodes?.Any() == true)
                query = query.Where(x => x.BranchCode != null && filter.BranchCodes.Contains(x.BranchCode));

            if (filter.DepartmentCodes?.Any() == true)
                query = query.Where(x => x.DepartmentCode != null && filter.DepartmentCodes.Contains(x.DepartmentCode));

            if (filter.DesignationCodes?.Any() == true)
                query = query.Where(x => x.DesignationCode != null && filter.DesignationCodes.Contains(x.DesignationCode));

            if (filter.EmployeeIDs?.Any() == true)
                query = query.Where(x => x.EmployeeId != null && filter.EmployeeIDs.Contains(x.EmployeeId));

            if (filter.EmpStatuses?.Any() == true)
                query = query.Where(x => x.EmployeeStatus != null && filter.EmpStatuses.Contains(x.EmployeeStatus));

            if (filter.EmpTypes?.Any() == true)
                query = query.Where(x => x.EmpTypeCode != null && filter.EmpTypes.Contains(x.EmpTypeCode));

            if (filter.EmpNatures?.Any() == true)
                query = query.Where(x => x.EmploymentNatureId != null && filter.EmpNatures.Contains(x.EmploymentNatureId));

            if (!string.IsNullOrWhiteSpace(filter.JoiningDateFrom) &&
                DateTime.TryParseExact(filter.JoiningDateFrom, "yyyy-MM-dd",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var fromDate))
                query = query.Where(x => x.JoiningDate.HasValue && x.JoiningDate.Value.Date >= fromDate.Date);

            if (!string.IsNullOrWhiteSpace(filter.JoiningDateTo) &&
                DateTime.TryParseExact(filter.JoiningDateTo, "yyyy-MM-dd",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var toDate))
                query = query.Where(x => x.JoiningDate.HasValue && x.JoiningDate.Value.Date <= toDate.Date);

            // ── Field-specific paging ───────────────────────────────────────────────
            IQueryable<DropdownPagedItemViewModel> fieldQuery;

            switch (field)
            {
                case "employees":
                    fieldQuery = query
                        .Where(x => x.EmployeeId != null)
                        .Select(x => new DropdownPagedItemViewModel
                        {
                            Code = x.EmployeeId,
                            Name = (x.FirstName ?? "") + " " + (x.LastName ?? "") + " (" + x.EmployeeId + ")"
                        })
                        .Distinct();

                    if (!string.IsNullOrEmpty(search))
                        fieldQuery = fieldQuery.Where(x =>
                            x.Code!.Contains(search) || x.Name!.Contains(search));

                    fieldQuery = fieldQuery.OrderBy(x => x.Name);
                    break;

                case "departments":
                    fieldQuery = query
                        .Where(x => x.DepartmentCode != null && x.DepartmentName != null)
                        .Select(x => new DropdownPagedItemViewModel
                        {
                            Code = x.DepartmentCode,
                            Name = x.DepartmentName
                        })
                        .Distinct();

                    if (!string.IsNullOrEmpty(search))
                        fieldQuery = fieldQuery.Where(x => x.Name!.Contains(search));

                    fieldQuery = fieldQuery.OrderBy(x => x.Name);
                    break;

                case "designations":
                    fieldQuery = query
                        .Where(x => x.DesignationCode != null && x.DesignationName != null)
                        .Select(x => new DropdownPagedItemViewModel
                        {
                            Code = x.DesignationCode,
                            Name = x.DesignationName
                        })
                        .Distinct();

                    if (!string.IsNullOrEmpty(search))
                        fieldQuery = fieldQuery.Where(x => x.Name!.Contains(search));

                    fieldQuery = fieldQuery.OrderBy(x => x.Name);
                    break;

                case "branches":
                    fieldQuery = branchRepo.All().AsNoTracking()
                        .Where(b => b.BranchCode != null && b.BranchName != null)
                        .Select(b => new DropdownPagedItemViewModel
                        {
                            Code = b.BranchCode,
                            Name = b.BranchName
                        })
                        .Distinct();

                    if (!string.IsNullOrEmpty(search))
                        fieldQuery = fieldQuery.Where(x => x.Name!.Contains(search));

                    // Scope to currently filtered set when a company is selected
                    if (filter.CompanyCodes?.Any() == true)
                    {
                        var filteredBranchCodes = query.Select(x => x.BranchCode).Distinct();
                        fieldQuery = fieldQuery.Where(x => filteredBranchCodes.Contains(x.Code));
                    }

                    fieldQuery = fieldQuery.OrderBy(x => x.Name);
                    break;

                case "empnatures":
                    fieldQuery = query
                        .Where(x => x.EmploymentNatureId != null)
                        .Select(x => new DropdownPagedItemViewModel
                        {
                            Code = x.EmploymentNatureId,
                            Name = x.EmpNatureName
                        })
                        .Distinct();

                    if (!string.IsNullOrEmpty(search))
                        fieldQuery = fieldQuery.Where(x => x.Name!.Contains(search));

                    fieldQuery = fieldQuery.OrderBy(x => x.Name);
                    break;

                case "emptypes":
                    fieldQuery = query
                        .Where(x => x.EmpTypeCode != null)
                        .Select(x => new DropdownPagedItemViewModel
                        {
                            Code = x.EmpTypeCode,
                            Name = x.EmpTypeName
                        })
                        .Distinct();

                    if (!string.IsNullOrEmpty(search))
                        fieldQuery = fieldQuery.Where(x => x.Name!.Contains(search));

                    fieldQuery = fieldQuery.OrderBy(x => x.Name);
                    break;

                default:
                    return new DropdownPagedResultViewModel();
            }

            var totalCount = await fieldQuery.CountAsync();
            var items = await fieldQuery
                                  .Skip((page - 1) * pageSize)
                                  .Take(pageSize)
                                  .ToListAsync();

            return new DropdownPagedResultViewModel
            {
                Items = items,
                HasMore = (page * pageSize) < totalCount,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
        public async Task<ReportDropdownPageResult> GetDropdownPageAsync2(ReportDropdownPageRequest request)
        {
            // Re-use the same base query logic, but only resolve one dropdown at a time.
            var query = from e in empOffRepo.All().AsNoTracking()
                        join emp in employeeRepo.All().AsNoTracking() on e.EmployeeId equals emp.EmployeeId
                        join c in companyRepo.All().AsNoTracking() on e.CompanyCode equals c.CompanyCode
                        join b in branchRepo.All().AsNoTracking() on e.BranchCode equals b.BranchCode into branchGroup
                        from b in branchGroup.DefaultIfEmpty()
                        join d in depRepo.All().AsNoTracking() on e.DepartmentCode equals d.DepartmentCode into deptGroup
                        from d in deptGroup.DefaultIfEmpty()
                        join ds in desiRepo.All().AsNoTracking() on e.DesignationCode equals ds.DesignationCode into desigGroup
                        from ds in desigGroup.DefaultIfEmpty()
                        join status in empStatusRepo.All().AsNoTracking() on e.EmployeeStatus equals status.EmployeeStatusId into statusGroup
                        from status in statusGroup.DefaultIfEmpty()
                        join emptype in empTypeRepo.All().AsNoTracking() on e.EmpTypeCode equals emptype.EmpTypeCode into empTypeGroup
                        from emptype in empTypeGroup.DefaultIfEmpty()
                        join empNature in empNatureRepo.All().AsNoTracking() on e.EmploymentNatureId equals empNature.EmploymentNatureId into empNatureGroup
                        from empNature in empNatureGroup.DefaultIfEmpty()
                        join sep in sepRepo.All().AsNoTracking() on e.EmployeeId equals sep.EmployeeId into separationGroup
                        from sep in separationGroup.DefaultIfEmpty()
                        let latestInc = incRepo.All()
                            .AsNoTracking()
                            .Where(x => x.EmployeeId == e.EmployeeId)
                            .OrderByDescending(x => x.Wef)
                            .FirstOrDefault()
                        select new
                        {
                            EmployeeId = e.EmployeeId,
                            PayId = e.PayId,
                            FirstName = emp.FirstName,
                            LastName = emp.LastName,
                            e.BranchCode,
                            e.CompanyCode,
                            e.DesignationCode,
                            DesignationName = ds != null ? ds.DesignationName : null,
                            e.DepartmentCode,
                            DepartmentName = d != null ? d.DepartmentName : null,
                            e.EmpTypeCode,
                            EmpTypeName = emptype != null ? emptype.EmpTypeName : null,
                            e.EmployeeStatus,
                            e.EmploymentNatureId,
                            EmpNatureName = empNature != null ? empNature.EmploymentNature : null,
                            e.JoiningDate,
                            e.GrossSalary,
                            e.DisbursementMethodId,
                            SeparationDate = sep == null ? (DateTime?)null : sep.SeparationDate,
                            LastIncrementWEF = latestInc != null ? (DateTime?)latestInc.Wef : null
                        };

            // ── Apply existing cascading filters ────────────────────────────────────
            if (request.CompanyCodes?.Any() == true)
                query = query.Where(x => x.CompanyCode != null && request.CompanyCodes.Contains(x.CompanyCode));

            if (request.BranchCodes?.Any() == true)
                query = query.Where(x => x.BranchCode != null && request.BranchCodes.Contains(x.BranchCode));

            if (request.DepartmentCodes?.Any() == true)
                query = query.Where(x => x.DepartmentCode != null && request.DepartmentCodes.Contains(x.DepartmentCode));

            if (request.DesignationCodes?.Any() == true)
                query = query.Where(x => x.DesignationCode != null && request.DesignationCodes.Contains(x.DesignationCode));

            if (request.EmployeeIDs?.Any() == true)
                query = query.Where(x => x.EmployeeId != null && request.EmployeeIDs.Contains(x.EmployeeId));

            if (request.EmpStatuses?.Any() == true)
                query = query.Where(x => x.EmployeeStatus != null && request.EmpStatuses.Contains(x.EmployeeStatus));

            if (request.EmpTypes?.Any() == true)
                query = query.Where(x => x.EmpTypeCode != null && request.EmpTypes.Contains(x.EmpTypeCode));

            if (request.EmpNatures?.Any() == true)
                query = query.Where(x => x.EmploymentNatureId != null && request.EmpNatures.Contains(x.EmploymentNatureId));

            if (!string.IsNullOrWhiteSpace(request.JoiningDateFrom) &&
                DateTime.TryParseExact(request.JoiningDateFrom, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fromDate))
                query = query.Where(x => x.JoiningDate.HasValue && x.JoiningDate.Value.Date >= fromDate.Date);

            if (!string.IsNullOrWhiteSpace(request.JoiningDateTo) &&
                DateTime.TryParseExact(request.JoiningDateTo, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var toDate))
                query = query.Where(x => x.JoiningDate.HasValue && x.JoiningDate.Value.Date <= toDate.Date);

            var search = request.Search?.Trim().ToLower() ?? "";
            int skip = (request.Page - 1) * request.PageSize;
            int take = request.PageSize;

            // ── Project + paginate for the requested dropdown ────────────────────────
            switch (request.TargetDropdown?.ToLower())
            {
                case "departments":
                    {
                        var baseQ = query
                            .Where(x => x.DepartmentCode != null && x.DepartmentName != null)
                            .Select(x => new ReportFilterResultViewModel { Code = x.DepartmentCode, Name = x.DepartmentName })
                            .Distinct();

                        if (!string.IsNullOrEmpty(search))
                            baseQ = baseQ.Where(x => x.Name.ToLower().Contains(search) || x.Code.ToLower().Contains(search));

                        var total = await baseQ.CountAsync();
                        var items = await baseQ.OrderBy(x => x.Name).Skip(skip).Take(take).ToListAsync();
                        return new ReportDropdownPageResult { Items = items, TotalCount = total, Page = request.Page, HasMore = skip + items.Count < total };
                    }

                case "designations":
                    {
                        var baseQ = query
                            .Where(x => x.DesignationCode != null && x.DesignationName != null)
                            .Select(x => new ReportFilterResultViewModel { Code = x.DesignationCode, Name = x.DesignationName })
                            .Distinct();

                        if (!string.IsNullOrEmpty(search))
                            baseQ = baseQ.Where(x => x.Name.ToLower().Contains(search) || x.Code.ToLower().Contains(search));

                        var total = await baseQ.CountAsync();
                        var items = await baseQ.OrderBy(x => x.Name).Skip(skip).Take(take).ToListAsync();
                        return new ReportDropdownPageResult { Items = items, TotalCount = total, Page = request.Page, HasMore = skip + items.Count < total };
                    }


                case "employeeids":
                    {
                        var filteredIds = query.Where(x => x.EmployeeId != null);

                        if (!string.IsNullOrEmpty(search))
                            filteredIds = filteredIds.Where(x =>
                                x.FirstName.ToLower().Contains(search) ||
                                x.LastName.ToLower().Contains(search) ||
                                x.EmployeeId.ToLower().Contains(search));

                        var total = await filteredIds
                            .Select(x => x.EmployeeId)
                            .Distinct()
                            .CountAsync();

                        var pagedIds = await filteredIds
                            .OrderBy(x => x.FirstName).ThenBy(x => x.LastName)
                            .Select(x => x.EmployeeId)
                            .Distinct()
                            .Skip(skip).Take(take)
                            .ToListAsync();

                        var rawItems = await query
                            .Where(x => pagedIds.Contains(x.EmployeeId))
                            .Select(x => new { x.EmployeeId, x.FirstName, x.LastName })
                            .ToListAsync();

                        var items = rawItems
                            .OrderBy(x => x.FirstName).ThenBy(x => x.LastName)
                            .Select(x => new ReportFilterResultViewModel
                            {
                                Code = x.EmployeeId,
                                Name = (x.FirstName ?? "") + " " + (x.LastName ?? "") + $" ({x.EmployeeId})"
                            }).ToList();

                        return new ReportDropdownPageResult { Items = items, TotalCount = total, Page = request.Page, HasMore = skip + items.Count < total };
                    }


                case "employees":
                    {
                        var baseQ = query
                            .Where(x => x.EmployeeId != null)
                            .Select(x => new ReportFilterResultViewModel
                            {
                                Code = x.EmployeeId,
                                Name = (x.FirstName ?? "") + " " + (x.LastName ?? "") + " " + $"({x.EmployeeId})"
                            })
                            .Distinct();

                        if (!string.IsNullOrEmpty(search))
                            baseQ = baseQ.Where(x => x.Name.ToLower().Contains(search) || x.Code.ToLower().Contains(search));

                        var total = await baseQ.CountAsync();
                        var items = await baseQ.OrderBy(x => x.Name).Skip(skip).Take(take).ToListAsync();
                        return new ReportDropdownPageResult { Items = items, TotalCount = total, Page = request.Page, HasMore = skip + items.Count < total };
                    }


                case "empnatures":
                    {
                        var baseQ = query
                            .Where(x => x.EmploymentNatureId != null)
                            .Select(x => new ReportFilterResultViewModel { Code = x.EmploymentNatureId, Name = x.EmpNatureName })
                            .Distinct();

                        if (!string.IsNullOrEmpty(search))
                            baseQ = baseQ.Where(x => x.Name.ToLower().Contains(search));

                        var total = await baseQ.CountAsync();
                        var items = await baseQ.OrderBy(x => x.Name).Skip(skip).Take(take).ToListAsync();
                        return new ReportDropdownPageResult { Items = items, TotalCount = total, Page = request.Page, HasMore = skip + items.Count < total };
                    }

                case "emptypes":
                    {
                        var baseQ = query
                            .Where(x => x.EmpTypeCode != null)
                            .Select(x => new ReportFilterResultViewModel { Code = x.EmpTypeCode, Name = x.EmpTypeName })
                            .Distinct();

                        if (!string.IsNullOrEmpty(search))
                            baseQ = baseQ.Where(x => x.Name.ToLower().Contains(search));

                        var total = await baseQ.CountAsync();
                        var items = await baseQ.OrderBy(x => x.Name).Skip(skip).Take(take).ToListAsync();
                        return new ReportDropdownPageResult { Items = items, TotalCount = total, Page = request.Page, HasMore = skip + items.Count < total };
                    }

                default:
                    return new ReportDropdownPageResult();
            }
        }

        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepo.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Employee Salary Info" && x.TitleCheck);
        }
    }
}
