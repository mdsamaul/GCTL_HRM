using Dapper;
using GCTL.Core.Data;
using GCTL.Core.ViewModels.HRLettersReportViewModel;
using GCTL.Data.Models;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace GCTL.Service.HRLettersReport
{
    public class HRLettersReportService : AppService<HrmDefHrletters>, IHRLettersReportService
    {
        private readonly string _connectionString;
        private readonly IRepository<HrmNocinfo> nocRepo;
        private readonly IRepository<HrmDefHrletters> hrLettersRepo;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;

        public HRLettersReportService(
            IConfiguration configuration, 
            IRepository<HrmNocinfo> nocRepo,
            IRepository<HrmDefHrletters> hrLettersRepo,
            IRepository<CoreAccessCode> accessCodeRepository
            ):base(hrLettersRepo)
        {
            _connectionString = configuration.GetConnectionString("ApplicationDbConnection");
            this.nocRepo = nocRepo;
            this.hrLettersRepo = hrLettersRepo;
            this.accessCodeRepository = accessCodeRepository;
        }


        #region Permission all type

        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AsNoTracking().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "HR Letters Report" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AsNoTracking().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "HR Letters Report" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AsNoTracking().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "HR Letters Report" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AsNoTracking().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "HR Letters Report" && x.CheckDelete);
        }

        #endregion

        // ── 1. Employee Details ───────────────────────────────────────────
        public async Task<FullEmployeeDetailsGetByIdViewModel> GetByEmployeeCodeAsync(string employeeCode)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<FullEmployeeDetailsGetByIdViewModel>(
                "FullEmployeeDetailsGetByid",
                new { EmployeeCode = employeeCode },
                commandType: CommandType.StoredProcedure
            );
        }
        public async Task<IEnumerable<EmployeeByLetterTypeDto>> GetEmployeesByLetterTypeAsync(string letterTypeId, string companyCode)
        {
            try
            {
                if (letterTypeId == "005")
                {
                    using var connection = new SqlConnection(_connectionString);
                    return await connection.QueryAsync<EmployeeByLetterTypeDto>(
                        @"SELECT DISTINCT 
                e.EmployeeID AS EmployeeId,
                LTRIM(RTRIM(e.FirstName + ' ' + e.LastName + ' (' + ' ' + e.EmployeeID + ')')) AS EmployeeName
              FROM HRM_Employee e
              INNER JOIN HRM_Separation s ON s.EmployeeID = e.EmployeeID
              ORDER BY EmployeeName",
                        new { LetterTypeId = letterTypeId, CompanyCode = companyCode }
                    );
                }
                else if (letterTypeId == "010")
                {
                    using var connection = new SqlConnection(_connectionString);
                    return await connection.QueryAsync<EmployeeByLetterTypeDto>(
                        @"SELECT DISTINCT 
                e.EmployeeID AS EmployeeId,
                LTRIM(RTRIM(e.FirstName + ' ' + e.LastName + ' (' + ' ' + e.EmployeeID + ')')) AS EmployeeName
              FROM HRM_Employee e
              INNER JOIN HRM_NOCInfo s ON s.EmployeeID = e.EmployeeID and s.NOCTypeId='travel'
              ORDER BY EmployeeName",
                        new { LetterTypeId = letterTypeId, CompanyCode = companyCode }
                    );
                }
                else if (letterTypeId == "016")
                {
                    using var connection = new SqlConnection(_connectionString);
                    return await connection.QueryAsync<EmployeeByLetterTypeDto>(
                        @"SELECT DISTINCT 
                e.EmployeeID AS EmployeeId,
                LTRIM(RTRIM(e.FirstName + ' ' + e.LastName + ' (' + ' ' + e.EmployeeID + ')')) AS EmployeeName
              FROM HRM_Employee e
              INNER JOIN HRM_NOCInfo s ON s.EmployeeID = e.EmployeeID and s.NOCTypeId='education'
              ORDER BY EmployeeName",
                        new { LetterTypeId = letterTypeId, CompanyCode = companyCode }
                    );
                }
                else if (letterTypeId == "017")
                {
                    using var connection = new SqlConnection(_connectionString);
                    return await connection.QueryAsync<EmployeeByLetterTypeDto>(
                        @"SELECT DISTINCT 
                e.EmployeeID AS EmployeeId,
                LTRIM(RTRIM(e.FirstName + ' ' + e.LastName + ' (' + ' ' + e.EmployeeID + ')')) AS EmployeeName
              FROM HRM_Employee e
              INNER JOIN HRM_EmployeeOfficialInfo ofe ON ofe.EmployeeID = e.EmployeeID
              ORDER BY EmployeeName",
                        new { LetterTypeId = letterTypeId, CompanyCode = companyCode }
                    );
                }
              //  else if (letterTypeId == "014")
              //  {
              //      using var connection = new SqlConnection(_connectionString);
              //      return await connection.QueryAsync<EmployeeByLetterTypeDto>(
              //          @"SELECT DISTINCT 
              //  e.EmployeeID AS EmployeeId,
              //  LTRIM(RTRIM(e.FirstName + ' ' + e.LastName + ' (' + ' ' + e.EmployeeID + ')')) AS EmployeeName
              //FROM HRM_Employee e
              //INNER JOIN HRM_EmployeeOfficialInfo offi ON offi.EmployeeID = e.EmployeeID
              //where offi.DesignationCode='023' and offi.EmployeeStatus ='01'
              //ORDER BY EmployeeName",
              //          new { LetterTypeId = letterTypeId, CompanyCode = companyCode }
              //      );
              //  }
                else if (letterTypeId == "019") // Offer Letter (Internship Program)
                {
                    using var connection = new SqlConnection(_connectionString);
                    return await connection.QueryAsync<EmployeeByLetterTypeDto>(
                        @"SELECT DISTINCT 
                            e.EmployeeID AS EmployeeId,
                            LTRIM(RTRIM(e.FirstName + ' ' + e.LastName + ' (' + ' ' + e.EmployeeID + ')')) AS EmployeeName
                        FROM HRM_Employee e
                        INNER JOIN HRM_EmployeeOfficialInfo ofe ON ofe.EmployeeID = e.EmployeeID
                        WHERE ofe.DesignationCode = '023' 
                          AND ofe.EmployeeStatus = '01'
                          AND (ofe.ConfirmeDate IS NULL OR ofe.ConfirmeDate = '1900-01-01 00:00:00.000')
                        ORDER BY EmployeeName",
                        new { LetterTypeId = letterTypeId, CompanyCode = companyCode }
                    );
                }
                else if (letterTypeId == "014") // Certificate of Internship
                {
                    using var connection = new SqlConnection(_connectionString);
                    return await connection.QueryAsync<EmployeeByLetterTypeDto>(
                        @"SELECT DISTINCT 
                            e.EmployeeID AS EmployeeId,
                            LTRIM(RTRIM(e.FirstName + ' ' + e.LastName + ' (' + ' ' + e.EmployeeID + ')')) AS EmployeeName
                        FROM HRM_Employee e
                        INNER JOIN HRM_EmployeeOfficialInfo offi ON offi.EmployeeID = e.EmployeeID
                        WHERE offi.DesignationCode = '023' 
                          AND offi.EmployeeStatus = '01'
                          AND offi.ConfirmeDate IS NOT NULL
                          AND offi.ConfirmeDate <> '1900-01-01 00:00:00.000'
                        ORDER BY EmployeeName",
                        new { LetterTypeId = letterTypeId, CompanyCode = companyCode }
                    );
                }
                else if (letterTypeId == "018") // Letter of Recommendation
                {
                    using var connection = new SqlConnection(_connectionString);
                    return await connection.QueryAsync<EmployeeByLetterTypeDto>(
                        @"SELECT DISTINCT 
                        e.EmployeeID AS EmployeeId,
                        LTRIM(RTRIM(e.FirstName + ' ' + e.LastName + ' (' + ' ' + e.EmployeeID + ')')) AS EmployeeName
                      FROM HRM_Employee e
                      INNER JOIN HRM_Separation s ON s.EmployeeID = e.EmployeeID
                      ORDER BY EmployeeName",
                        new { LetterTypeId = letterTypeId, CompanyCode = companyCode }
                    );
                }

                return Enumerable.Empty<EmployeeByLetterTypeDto>();
            }
            catch (Exception)
            {

                throw;
            }
        }

        // ── 3. Save or Update Letter Record ───────────────────────────────────
        public async Task<string> SaveOrUpdateLetterAsync(SaveOrUpdateLetterRequestDto dto)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                int currentYear = (dto.AppliedDate ?? DateTime.Now).Year;

                var existing = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    @"SELECT AutoId, RefNo FROM HRM_EmployeeLetters 
              WHERE EmployeeId = @EmployeeId 
                AND LetterTypeId = @LetterTypeId 
                AND CompanyCode = @CompanyCode",
                    new { dto.EmployeeId, dto.LetterTypeId, dto.CompanyCode }
                );

                if (existing != null)
                {
                    await connection.ExecuteAsync(
                        @"UPDATE HRM_EmployeeLetters 
                  SET AppliedDate = @AppliedDate,
                      LUser = @LUser,
                      LDate = @LDate,
                      LIP = @LIP,
                      ModifyDate = @ModifyDate,
                      NOCID = @NOCID
                  WHERE AutoId = @AutoId",
                        new
                        {
                            AppliedDate = dto.AppliedDate ?? DateTime.Now,
                            LUser = dto.Luser,
                            LDate = dto.Ldate ?? DateTime.Now,
                            LIP = dto.Lip,
                            ModifyDate = DateTime.Now,
                            NOCID = dto.NocId,
                            AutoId = existing.AutoId
                        }
                    );

                    return (string)existing.RefNo;
                }
                else
                {
                    // ── LetterNo: global sequence ──
                    var lastLetterNo = await connection.QueryFirstOrDefaultAsync<string>(
                        @"SELECT TOP 1 LetterNo 
                  FROM HRM_EmployeeLetters
                  WHERE CompanyCode = @CompanyCode
                  ORDER BY AutoId DESC",
                        new { dto.CompanyCode }
                    );

                    int nextLetterNumber = 1;
                    if (!string.IsNullOrEmpty(lastLetterNo) && int.TryParse(lastLetterNo, out int lastLetter))
                        nextLetterNumber = lastLetter + 1;

                    string newLetterNo = nextLetterNumber.ToString("D5");

                    // ── RefNo: year based sequence ──
                    var lastRefNo = await connection.QueryFirstOrDefaultAsync<string>(
                        @"SELECT TOP 1 RefNo 
                  FROM HRM_EmployeeLetters
                  WHERE CompanyCode = @CompanyCode
                    AND RefNo LIKE @Pattern
                  ORDER BY CAST(REVERSE(SUBSTRING(REVERSE(RefNo), 1, CHARINDEX('/', REVERSE(RefNo)) - 1)) AS INT) DESC",
                        new { dto.CompanyCode, Pattern = $"DP/HRD/{currentYear}/%" }
                    );

                    int nextRefNumber = 1;
                    if (!string.IsNullOrEmpty(lastRefNo))
                    {
                        var lastPart = lastRefNo.Split('/').LastOrDefault();
                        if (!string.IsNullOrEmpty(lastPart) && int.TryParse(lastPart, out int lastRef))
                            nextRefNumber = lastRef + 1;
                    }

                    string newRefNo = $"DP/HRD/{currentYear}/{nextRefNumber.ToString("D5")}";

                    await connection.ExecuteAsync(
                        @"INSERT INTO HRM_EmployeeLetters 
                    (LetterNo, RefNo, LetterTypeId, EmployeeId, AppliedDate, 
                     LUser, LDate, LIP, LMAC, CompanyCode, EntryUserEmployeeID, NOCID)
                  VALUES 
                    (@LetterNo, @RefNo, @LetterTypeId, @EmployeeId, @AppliedDate,
                     @LUser, @LDate, @LIP, @LMAC, @CompanyCode, @EntryUserEmployeeID, @NOCID)",
                        new
                        {
                            LetterNo = newLetterNo,
                            RefNo = newRefNo,
                            dto.LetterTypeId,
                            dto.EmployeeId,
                            AppliedDate = dto.AppliedDate ?? DateTime.Now,
                            LUser = dto.Luser,
                            LDate = dto.Ldate ?? DateTime.Now,
                            LIP = dto.Lip,
                            LMAC = dto.Lmac ?? "",
                            dto.CompanyCode,
                            EntryUserEmployeeID = dto.EntryUserEmployeeId,
                            NOCID = dto.NocId
                        }
                    );

                    return newRefNo;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // ── 2. Generate PDF ───────────────────────────────────────────────
        public async Task<byte[]> GeneratePdfAsync(HRLetterReportRequestViewModel request)
        {
            var emp = await GetByEmployeeCodeAsync(request.EmployeeCode);
            var signatory = await GetByEmployeeCodeAsync(request.SignatoryEmployeeCode);

            using var ms = new MemoryStream();
            using var writer = new PdfWriter(ms);
            using var pdf = new PdfDocument(writer);
            using var doc = new Document(pdf, iText.Kernel.Geom.PageSize.A4);

            doc.SetMargins(50, 60, 50, 60);

            var fontRegular = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);
            var fontBold = PdfFontFactory.CreateFont(StandardFonts.TIMES_BOLD);

            var elements = new List<IBlockElement>();

            switch (request.HrLetterTypeId)
            {
               
                case "005": BuildTerminationLetter(elements, emp, signatory, request, fontRegular, fontBold); break;
                case "016": BuildNocEducationlLetter(elements, emp, signatory, request, fontRegular, fontBold); break;
                case "017": BuildNocGeneralLetter(elements, emp, signatory, request, fontRegular, fontBold); break;
                case "010": BuildNocTravelLetter(elements, emp, signatory, request, fontRegular, fontBold); break;
                case "019": BuildInternshipOfferLetter(elements, emp, signatory, request, fontRegular, fontBold); break;
                case "014": BuildInternshipCertificate(elements, emp, signatory, request, fontRegular, fontBold); break;
                case "018": BuildRecommendationLetter(elements, emp, signatory, request, fontRegular, fontBold); break;
                default: BuildDischargeCertificate(elements, emp, signatory, request, fontRegular, fontBold); break;
            }

            // Vertical center — full height single-cell table
            float usableHeight = pdf.GetDefaultPageSize().GetHeight() - 50 - 50;

            var table = new Table(1).UseAllAvailableWidth().SetHeight(usableHeight);
            var cell = new Cell()
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(Border.NO_BORDER)
                .SetPaddingLeft(0)
                .SetPaddingRight(0);

            foreach (var el in elements)
                cell.Add(el);

            table.AddCell(cell);
            doc.Add(table);

            doc.Close();
            return ms.ToArray();
        }

      
        // ════════════════════════════════════════════════════════════════
        //  SHARED HELPERS — List<IBlockElement> 
        // ════════════════════════════════════════════════════════════════



        private void AddTitle(List<IBlockElement> elements, string title, PdfFont fontBold)
        {
            elements.Add(new Paragraph(title)
                .SetFont(fontBold)
                .SetFontSize(18)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetUnderline()
                .SetMarginBottom(20));
        }

        private void AddBodyParagraph(List<IBlockElement> elements, string text, PdfFont font)
        {
            elements.Add(new Paragraph(text)
                .SetFont(font)
                .SetFontSize(11)
                .SetTextAlignment(TextAlignment.JUSTIFIED)
                .SetMultipliedLeading(1.8f)
                .SetMarginBottom(12));
        }

        private Paragraph MixedPara(PdfFont fontRegular, PdfFont fontBold,
            params (string text, bool bold)[] parts)
        {
            var para = new Paragraph()
                .SetFontSize(11)
                .SetTextAlignment(TextAlignment.JUSTIFIED)
                .SetMultipliedLeading(1.8f)
                .SetMarginBottom(12);

            foreach (var (text, bold) in parts)
                para.Add(new Text(text).SetFont(bold ? fontBold : fontRegular));

            return para;
        }

        private void AddToAddress(List<IBlockElement> elements,
            FullEmployeeDetailsGetByIdViewModel emp,
            PdfFont fontRegular, PdfFont fontBold)
        {
            elements.Add(new Paragraph()
                .Add(new Text("To\n").SetFont(fontBold))
                .Add(new Text($"{emp?.EmployeeName ?? ""}\n").SetFont(fontRegular))
                .Add(new Text($"{emp?.DesignationName ?? ""}, {emp?.DepartmentName ?? ""}").SetFont(fontRegular))
                .SetFontSize(11)
                .SetMarginBottom(15));
        }


        private void AddSignature(List<IBlockElement> elements,
    FullEmployeeDetailsGetByIdViewModel sig,
    HRLetterReportRequestViewModel req,
    PdfFont fontRegular, PdfFont fontBold)
        {
            elements.Add(new Paragraph("Best Regards,")
                .SetFont(fontRegular).SetFontSize(11)
                .SetMarginTop(20).SetMarginBottom(50));

            elements.Add(new LineSeparator(new iText.Kernel.Pdf.Canvas.Draw.DottedLine())
                .SetWidth(200)
                .SetHorizontalAlignment(HorizontalAlignment.LEFT)
                .SetMarginTop(35).SetMarginBottom(5));

            elements.Add(new Paragraph(sig?.EmployeeName ?? "")
                .SetFont(fontBold).SetFontSize(11).SetMarginBottom(2));
            elements.Add(new Paragraph(req.Designation ?? "")
                .SetFont(fontRegular).SetFontSize(11).SetMarginBottom(2));
            elements.Add(new Paragraph($"Cell: {sig.OfficialPhone ?? ""}")
                .SetFont(fontRegular).SetFontSize(11).SetMarginBottom(2));
            elements.Add(new Paragraph($"Email: {sig.OfficialEmail ?? ""}")  
                .SetFont(fontRegular).SetFontSize(11));
        }

        // ════════════════════════════════════════════════════════════════
        //  LETTER TEMPLATES — List<IBlockElement> 
        // ════════════════════════════════════════════════════════════════

        // 1. DISCHARGE CERTIFICATE
        private void BuildDischargeCertificate(List<IBlockElement> elements,
            FullEmployeeDetailsGetByIdViewModel emp,
            FullEmployeeDetailsGetByIdViewModel sig,
            HRLetterReportRequestViewModel req,
            PdfFont fontRegular, PdfFont fontBold)
        {
            var refNo = emp?.LetterRefNo ?? $"DP/HRD/{DateTime.Now.Year}/-----";
            AddRefDate(elements, refNo, req.AppliedDate.ToString(), fontRegular);
            AddTitle(elements, "Discharge Certificate", fontBold);

            elements.Add(MixedPara(fontRegular, fontBold,
                ("This is to certify that ", false),
                (emp?.EmployeeName ?? "", true),
                (" had been employed as an ", false),
                (emp?.DesignationName ?? "", true),
                (" of the ", false),
                (emp?.DepartmentName ?? "", true),
                (" department at ", false),
                (emp?.CompanyName ?? "", true),
                (" as a full-time employee from ", false),
                ($"{emp?.JoiningDate} till {emp?.LeavingDate ?? req.AppliedDate.ToString()}", true),
                (". He/She has been relieved of all his/her duties from the company.", false)
            ));

            AddBodyParagraph(elements,
                "This certification is being issued upon request of the above-mentioned employee " +
                "for whatever purpose it may serve him/her best.", fontRegular);

            AddSignature(elements, sig, req, fontRegular, fontBold);
        }

        // 2. EMPLOYMENT CERTIFICATE
        private void BuildEmploymentCertificate(List<IBlockElement> elements,
            FullEmployeeDetailsGetByIdViewModel emp,
            FullEmployeeDetailsGetByIdViewModel sig,
            HRLetterReportRequestViewModel req,
            PdfFont fontRegular, PdfFont fontBold)
        {
            var refNo = emp?.LetterRefNo ?? $"EC/HRD/{DateTime.Now.Year}/-----";
            AddRefDate(elements, refNo, req.AppliedDate.ToString(), fontRegular);
            AddTitle(elements, "Employment Certificate", fontBold);

            elements.Add(MixedPara(fontRegular, fontBold,
                ("This is to certify that ", false),
                (emp?.EmployeeName ?? "", true),
                (" is employed as ", false),
                (emp?.DesignationName ?? "", true),
                (" in the ", false),
                (emp?.DepartmentName ?? "", true),
                (" department at ", false),
                (emp?.CompanyName ?? "", true),
                (" since ", false),
                (emp?.JoiningDate ?? "", true),
                (". He/She is a regular and full-time employee of our organization.", false)
            ));

            AddBodyParagraph(elements,
                "This certificate is issued upon his/her request for whatever purpose it may serve.",
                fontRegular);

            AddSignature(elements, sig, req, fontRegular, fontBold);
        }

        // 3. NOC TRAVEL
        private void BuildNocTravelLetter(List<IBlockElement> elements,
     FullEmployeeDetailsGetByIdViewModel emp,
     FullEmployeeDetailsGetByIdViewModel sig,
     HRLetterReportRequestViewModel req,
     PdfFont fontRegular, PdfFont fontBold)
        {
            try
            {
                // Gender
                bool isMale = (emp?.Gender ?? "").Trim().ToLower() is "male" or "m";
                string heShe = isMale ? "He" : "She";
                string hisHer = isMale ? "his" : "her";

                var nocData = nocRepo.All().Where(x => x.EmployeeId == emp.EmployeeID).ToList().FirstOrDefault();
                // Date format
                string leaveFrom = FormatDate(nocData.FromDate);
                string leaveTo = FormatDate(nocData.ToDate);

                var refNo = req.LetterRefNo ?? $"DP/HRD/{(req.AppliedDate.HasValue ? req.AppliedDate.Value.Year : DateTime.Now.Year)}/-----";

                AddRefDate(elements, refNo, req.AppliedDate.ToString(), fontRegular);

                AddTitle(elements, "To Whom It May Concern", fontBold);
                // Paragraph 1 — employee intro
                elements.Add(MixedPara(fontRegular, fontBold,
                    ("This is to certify that ", false),
                    (emp?.EmployeeName ?? "", true),
                    (" (Passport Number: ", false),
                    (emp.PassportNo ?? "", true),
                    (") has been serving ", false),
                    (emp?.CompanyName ?? "", true),
                    (" since ", false),
                    (FormatDate(emp?.JoiningDate), true),
                    (". ", false),
                    (heShe, false),
                    (" is working for our ", false),
                    (emp?.DepartmentName ?? "", true),
                    (" team as a ", false),
                    (emp?.DesignationName ?? "", true),
                    (".", false)
                ));

                // Paragraph 2 — leave & travel
                elements.Add(MixedPara(fontRegular, fontBold,
                    ($"{heShe} has applied for leave starting from ", false),
                    (leaveFrom, true),
                    (" to ", false),
                    (leaveTo, true),
                    (" to visit ", false),
                    (nocData.PlaceofVisit ?? "", true),
                    (". We have accepted ", false),
                    (hisHer, false),
                    (" leave application and have given ", false),
                    (hisHer, false),
                    (" our full clearance to apply for the visa to travel ", false),
                    (nocData.PlaceofVisit ?? "", true),
                    (".", false)
                ));

                // Paragraph 3 — closing
                AddBodyParagraph(elements,
                    "The No Objection Certificate (NOC) has been issued accepting the leave application.",
                    fontRegular);

                AddSignature(elements, sig, req, fontRegular, fontBold);
            }
            catch (Exception)
            {

                throw;
            }
        }
        // 3. NOC Educationl
        private void BuildNocEducationlLetter(List<IBlockElement> elements,
     FullEmployeeDetailsGetByIdViewModel emp,
     FullEmployeeDetailsGetByIdViewModel sig,
     HRLetterReportRequestViewModel req,
     PdfFont fontRegular, PdfFont fontBold)
        {
            try
            {
                // Gender
                bool isMale = (emp?.Gender ?? "").Trim().ToLower() is "male" or "m";
                string heShe = isMale ? "He" : "She";
                string hisHer = isMale ? "his" : "her";

                var nocData = nocRepo.All().Where(x => x.EmployeeId == emp.EmployeeID).ToList().FirstOrDefault();
                // Date format
                string leaveFrom = FormatDate(nocData.FromDate);
                string leaveTo = FormatDate(nocData.ToDate);

                var refNo = req.LetterRefNo ?? $"DP/HRD/{(req.AppliedDate.HasValue ? req.AppliedDate.Value.Year : DateTime.Now.Year)}/-----";

                AddRefDate(elements, refNo, req.AppliedDate.ToString(), fontRegular);

                AddTitle(elements, "No Objection Certificate", fontBold);
                // Paragraph 1 — employee intro
                elements.Add(MixedPara(fontRegular, fontBold,
                    ("This is to certify that ", false),
                    (emp?.EmployeeName ?? "", true),
                    (" is working with us as an ", false),
                    (emp?.DesignationName ?? "", true),
                    (" in the ", false),
                    (emp?.DepartmentName ?? "", true),
                    (" department. ", false),
                    (heShe, false),
                    (" joined our company on ", false),
                    (FormatDate(emp?.JoiningDate), true),
                    ("  and has been working with us for the company till now.", false)

                ));

                // Paragraph 2 — leave & travel
                elements.Add(MixedPara(fontRegular, fontBold,
                     ($"We understand that {heShe.ToLower()} has got academic commitments from the ", false),
                     (nocData.CourseName ?? "", true),
                     (" program at ", false),
                     (nocData.UniversityName ?? "", true),
                     (". ", false),
                     (heShe, false),
                     (" can maintain full time academic schedule during this ", false), (nocData.CourseName ?? "", true),
                     (" program to meet his requirements at ", false),
                     (nocData.UniversityName ?? "", true),
                     (".", false)
                 ));

                // Paragraph 3 — closing
                AddBodyParagraph(elements,
                    "This certification is issued to whatever purpose it may serve him best. We wish him all the best.",
                    fontRegular);

                AddSignature(elements, sig, req, fontRegular, fontBold);
            }
            catch (Exception)
            {

                throw;
            }
        }


        // 4. NOC GENERAL
        private void BuildNocGeneralLetter(List<IBlockElement> elements,
    FullEmployeeDetailsGetByIdViewModel emp,
    FullEmployeeDetailsGetByIdViewModel sig,
    HRLetterReportRequestViewModel req,
    PdfFont fontRegular, PdfFont fontBold)
        {
            try
            {
                // Gender
                bool isMale = (emp?.Gender ?? "").Trim().ToLower() is "male" or "m";
                string heShe = isMale ? "He" : "She";
                string himHer = isMale ? "him" : "her";

                var refNo = req.LetterRefNo ??
                            $"DP/HRD/{(req.AppliedDate.HasValue ? req.AppliedDate.Value.Year : DateTime.Now.Year)}/-----";

                AddRefDate(elements, refNo, req.AppliedDate.ToString(), fontRegular);

                AddTitle(elements, "No Objection Certificate", fontBold);

                // Paragraph 1
                elements.Add(MixedPara(fontRegular, fontBold,
                    ("This is to certify that ", false),
                    (emp?.EmployeeName ?? "", true),
                    (" is working with us as a ", false),
                    (emp?.DesignationName ?? "", true),
                    (" in the ", false),
                    (emp?.DepartmentName ?? "", true),
                    (" department. ", false),
                    (heShe, false),
                    (" joined our company on ", false),
                    (FormatDate(emp?.JoiningDate), true),
                    (" and will be working for the company till ", false),
                     (FormatDate(emp.LeavingDate) != ""? FormatDate(emp.LeavingDate): "now", true),
                     (". ", false),
                    (heShe, false),
                    (" is a sincere worker and is not involved in any illegal activities as per our knowledge.", false)
                ));

                // Paragraph 2
                AddBodyParagraph(elements,
                    $"This certification is issued to whatever purpose it may serve {himHer} best. We wish {himHer} all the best.",
                    fontRegular);

                AddSignature(elements, sig, req, fontRegular, fontBold);
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void AddRefDate(List<IBlockElement> elements, string refNo, string date, PdfFont font)
        {
            string formattedDate = FormatDate(date);
            elements.Add(new Paragraph(refNo ?? "")
                .SetFont(font).SetFontSize(11).SetMarginBottom(2));
            elements.Add(new Paragraph(formattedDate)
                .SetFont(font).SetFontSize(11).SetMarginBottom(25));
        }

        // 5. TERMINATION LETTER
        private void BuildTerminationLetter(List<IBlockElement> elements,
            FullEmployeeDetailsGetByIdViewModel emp,
            FullEmployeeDetailsGetByIdViewModel sig,
            HRLetterReportRequestViewModel req,
            PdfFont fontRegular, PdfFont fontBold)
        {
            // Gender
            bool isMale = (emp?.Gender ?? "").Trim().ToLower() is "male" or "m";
            string heShe = isMale ? "He" : "She";
            string hisHer = isMale ? "his" : "her";
            string himHer = isMale ? "him" : "her";

            // Date format
            string joiningDate = FormatDate(emp?.JoiningDate);
            string leavingDate = !string.IsNullOrEmpty(emp?.LeavingDate)
                ? FormatDate(emp.LeavingDate)
                : FormatDate(req.AppliedDate);

            var refNo = req.LetterRefNo ?? $"DP/HRD/{(req.AppliedDate.HasValue ? req.AppliedDate.Value.Year : DateTime.Now.Year)}/-----";
            AddRefDate(elements, refNo, req.AppliedDate.ToString(), fontBold); // fontBold → fontRegular
            AddTitle(elements, "Discharge Certificate", fontBold);

            elements.Add(MixedPara(fontRegular, fontBold,
                ("This is to certify that ", false),
                (emp?.EmployeeName ?? "", true),
                (" had been employed as an ", false),
                (emp?.DesignationName ?? "", true),
                (" of the ", false),
                (emp?.DepartmentName ?? "", true),
                (" department at ", false),
                (emp?.CompanyName ?? "", true),
                (" as a full-time employee from ", false),
                (joiningDate, true),
                (" till ", false),
                (leavingDate, true),
                ($". {heShe} has been relieved of all {hisHer} duties from the company.", false)
            ));

            AddBodyParagraph(elements,
                $"This certification is being issued upon request of the above-mentioned employee " +
                $"for whatever purpose it may serve {himHer} best.",
                fontRegular);

            AddSignature(elements, sig, req, fontRegular, fontBold);
        }

        private string FormatDate(string date)
        {
            if (string.IsNullOrWhiteSpace(date)) return "";

            string[] formats =
            {
        // MM/dd/yyyy formats
        "MM/dd/yyyy",
        "M/d/yyyy",
        "MM/dd/yyyy HH:mm:ss",
        "M/d/yyyy HH:mm:ss",

        // Optional: dd/MM/yyyy formats
        "dd/MM/yyyy",
        "d/M/yyyy",
        "dd/MM/yyyy HH:mm:ss",
        "d/M/yyyy HH:mm:ss",

        // Optional: dash formats
        "MM-dd-yyyy",
        "M-d-yyyy",
        "MM-dd-yyyy HH:mm:ss",
        "M-d-yyyy HH:mm:ss",

        "dd-MM-yyyy",
        "d-M-yyyy",
        "dd-MM-yyyy HH:mm:ss",
        "d-M-yyyy HH:mm:ss"
    };

            if (DateTime.TryParseExact(
                    date.Trim(),
                    formats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime parsed))
            {
                return parsed.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture);
            }

            return date;
        }

        private string FormatDate(DateTime? date)
        {
            if (!date.HasValue) return "";

            return date.Value.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture);
        }


        // ── 013. INTERNSHIP OFFER LETTER ─────────────────────────────────────────
        /// <summary>
        /// Requires req.InternUniversityName, req.InternAddress, req.InternStartDate,
        /// req.InternDurationMonths to be populated from the frontend.
        /// Falls back to placeholders if null.
        /// </summary>
        private void BuildInternshipOfferLetter(List<IBlockElement> elements,
    FullEmployeeDetailsGetByIdViewModel emp,
    FullEmployeeDetailsGetByIdViewModel sig,
    HRLetterReportRequestViewModel req,
    PdfFont fontRegular, PdfFont fontBold)
        {
            bool isMale = (emp?.Gender ?? "").Trim().ToLower() is "male" or "m";
            string hisHer = isMale ? "his" : "her";

            string universityName = emp.HighestInstitute ?? "";
            //string internAddress = emp.InternAddress ?? "";
            string internAddress = !string.IsNullOrWhiteSpace(emp.PresentAddress)
            ? $"{emp.PresentAddress}, {emp.PresentThana}, {emp.PresentDistrict} - {emp.PresentPostCode}"
            : "";
            string companyAddress = !string.IsNullOrWhiteSpace(emp?.Address1)? $"{emp.Address1}, {emp.CompanyCountry}": "";
            string startDateStr = FormatDate(emp.JoiningDate);
            string duration = emp.ProbationPeriod ?? "";

            var refNo = req.LetterRefNo
                ?? $"DP/HRD/{(req.AppliedDate.HasValue ? req.AppliedDate.Value.Year : DateTime.Now.Year)}";

            AddRefDate(elements, refNo, req.AppliedDate.ToString(), fontBold);

            elements.Add(new Paragraph()
                .Add(new Text($"{emp?.EmployeeName ?? ""}\n").SetFont(fontBold))
                .Add(new Text($"{universityName}\n").SetFont(fontBold))
                .Add(new Text(internAddress).SetFont(fontBold))
                .SetFontSize(11)
                .SetMarginBottom(15));

            elements.Add(new Paragraph()
                .Add(new Text("Subject: ").SetFont(fontBold))
                .Add(new Text("Internship Program.").SetFont(fontBold).SetUnderline(0.75f, -2f))
                .SetFontSize(11)
                .SetMarginBottom(15));

            elements.Add(new Paragraph($"Dear {emp?.EmployeeName ?? ""},")
                .SetFont(fontBold)
                .SetFontSize(11)
                .SetMarginBottom(12));

            elements.Add(MixedPara(fontRegular, fontBold,
                ("In reference to your application, we would like to congratulate you on being selected for internship with ", false),
                (emp?.CompanyName ?? "", true),
                (" based at ", false),
                (companyAddress, false),
                (". Your internship program is scheduled to start effective ", false),
                (startDateStr, true),
                (" for a period of ", false),
                (duration, true),
                (". All of us at ", false),
                (emp?.CompanyName ?? "", true),
                (" are excited that you will be joining our team!", false)
            ));

            AddBodyParagraph(elements,
                "As such, your internship will include training/orientation and focus primarily on learning and developing " +
                "new skills and gaining a deeper understanding of concepts through hands-on application of the knowledge you learned in class.",
                fontRegular);

            AddSignature(elements, sig, req, fontRegular, fontBold);
        }

        // ── 014. CERTIFICATE OF INTERNSHIP ──────────────────────────────────────
        private void BuildInternshipCertificate(List<IBlockElement> elements,
            FullEmployeeDetailsGetByIdViewModel emp,
            FullEmployeeDetailsGetByIdViewModel sig,
            HRLetterReportRequestViewModel req,
            PdfFont fontRegular, PdfFont fontBold)
        {
            bool isMale = (emp?.Gender ?? "").Trim().ToLower() is "male" or "m";
            string heShe = isMale ? "He" : "She";
            string hisHer = isMale ? "his" : "her";
            string himHer = isMale ? "him" : "her";

            string joiningDate = FormatDate(emp?.JoiningDate);
            string ConfirmationDate = !string.IsNullOrEmpty(emp?.ConfirmationDate)
                ? FormatDate(emp.ConfirmationDate)
                :"";

            var refNo = req.LetterRefNo
                ?? $"DP/HRD/{(req.AppliedDate.HasValue ? req.AppliedDate.Value.Year : DateTime.Now.Year)}/{req.AppliedDate?.Year}";

            AddRefDate(elements, refNo, req.AppliedDate.ToString(), fontRegular);
            AddTitle(elements, "Certificate of Internship", fontBold);

            // Paragraph 1
            elements.Add(MixedPara(fontRegular, fontBold,
                ("This is to certify that ", false),
                (emp?.EmployeeName ?? "", true),
                (" worked with us as an ", false),
                (emp?.DesignationName ?? "", true),
                (" in the ", false),
                (emp?.DepartmentName ?? "", true),
                (" department. ", false),
                (heShe, false),
                (" joined our company on ", false),
                (joiningDate, true),
                (" and completed ", false),
                (hisHer, false),
                (" Internship on ", false),
                (ConfirmationDate, true),
                ($". {heShe} was a sincere worker and was not involved in any illegal activities as per our knowledge.", false)
            ));

            // Paragraph 2
            AddBodyParagraph(elements,
                $"This certification is issued to whatever purpose it may serve {himHer} best. We wish {himHer} all the best.",
                fontRegular);

            AddSignature(elements, sig, req, fontRegular, fontBold);
        }


        // ── 015. LETTER OF RECOMMENDATION ───────────────────────────────────────
        private void BuildRecommendationLetter(List<IBlockElement> elements,
            FullEmployeeDetailsGetByIdViewModel emp,
            FullEmployeeDetailsGetByIdViewModel sig,
            HRLetterReportRequestViewModel req,
            PdfFont fontRegular, PdfFont fontBold)
        {
            bool isMale = (emp?.Gender ?? "").Trim().ToLower() is "male" or "m";
            string heShe = isMale ? "He" : "She";
            string hisHer = isMale ? "his" : "her";
            string himHer = isMale ? "him" : "her";

            string joiningDate = FormatDate(emp?.JoiningDate);
            string leavingDate = !string.IsNullOrEmpty(emp?.LeavingDate)
                ? FormatDate(emp.LeavingDate)
                : FormatDate(req.AppliedDate);

            // refNo: image shows "DP/HRD/2025" (no sequential number) — keep existing pattern
            var refNo = req.LetterRefNo
                ?? $"DP/HRD/{(req.AppliedDate.HasValue ? req.AppliedDate.Value.Year : DateTime.Now.Year)}";

            AddRefDate(elements, refNo, req.AppliedDate.ToString(), fontRegular);
            AddTitle(elements, "Letter of Recommendation", fontBold);

            // Paragraph 1 — tenure & qualities
            elements.Add(MixedPara(fontRegular, fontBold,
                ("I am pleased to recommend ", false),
                (emp?.EmployeeName ?? "", true),
                (", who has served as a ", false),
                (emp?.DesignationName ?? "", true),
                (" in the ", false),
                (emp?.DepartmentName ?? "", true),
                (" department from ", false),
                (joiningDate, true),
                (", to ", false),
                (leavingDate, true),
                (". During ", false),
                (hisHer, false),
                (" tenure, ", false),
                (heShe.ToLower(), false),
                (" demonstrated exceptional skills, a strong work ethic, and a passion for ", false),
                (hisHer, false),
                (" responsibilities. ", false),
                (emp?.EmployeeName ?? "", true),
                (" consistently exceeded expectations, driving our department's success through ", false),
                (hisHer, false),
                (" dedication and innovative approach. ", false),
                ($"{hisHer.Substring(0, 1).ToUpper()}{hisHer.Substring(1)} ability to manage complex projects with precision and ", false),
                (hisHer, false),
                (" commitment to excellence have been truly commendable.", false)
            ));

            // Paragraph 2 — closing confidence
            AddBodyParagraph(elements,
                $"I am confident that {hisHer} passion for work, coupled with {hisHer} exceptional work ethic, " +
                $"will make {himHer} a valuable asset for {hisHer} future endeavors. " +
                $"{heShe} has my highest recommendation.",
                fontRegular);

            AddSignature(elements, sig, req, fontRegular, fontBold);
        }
    }
}