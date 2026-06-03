using AutoMapper;
using ClosedXML.Excel;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.Designations;
using GCTL.Data.Models;
using GCTL.Service.Common;
using GCTL.Service.Designations;
using GCTL.UI.Core.ViewModels.Designations;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class DesignationsController : BaseController
    {
        private readonly IDesignationService designationService;
        private readonly ICommonService commonService;
        private readonly IMapper mapper;
        string strMaxNO = "";
        public DesignationsController(
            IDesignationService designationService,
            ICommonService commonService,
            IMapper mapper
        )
        {
            this.designationService = designationService;
            this.commonService = commonService;
            this.mapper = mapper;
        }

        public IActionResult Index(bool child = false)
        {
            DesignationPageViewModel model = new DesignationPageViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };
            commonService.FindMaxNo(ref strMaxNO, "DesignationCode", "HRM_Def_Designation", 3);
            model.Setup = new DesignationSetupViewModel
            {
                DesignationCode = strMaxNO
            };

            if (child)
                return PartialView(model);

            return View(model);
        }

        public IActionResult Setup(string id)
        {
            DesignationSetupViewModel model = new DesignationSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "DesignationCode", "HRM_Def_Designation", 3);
            var result = designationService.GetDesignation(id);
            if (result != null)
            {
                model = mapper.Map<DesignationSetupViewModel>(result);
                model.Code = id;
                model.Id = (int)result.AutoId;
            }
            else
            {
                model.DesignationCode = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Setup(DesignationSetupViewModel model)
        {
            if (designationService.IsDesignationExist(model.DesignationName, model.DesignationCode))
            {
                return Json(new { isSuccess = false, message = "Already Exists" });
            }

            if (ModelState.IsValid)
            {
                if (designationService.IsDesignationExistByCode(model.DesignationCode))
                {
                    var hasPermission = designationService.UpdatePermission(LoginInfo.AccessCode);
                    if (hasPermission)
                    {
                        HrmDefDesignation designation = designationService.GetDesignation(model.DesignationCode) ?? new HrmDefDesignation();
                        model.ToAudit(LoginInfo, model.AutoId > 0);
                        mapper.Map(model, designation);
                        model.BanglaShortName = string.Empty;
                        model.BanglaDesignation = string.Empty;
                        model.CompanyCode = string.Empty;
                        model.EmployeeId = string.Empty;
                        model.MobileAllowanceId = string.Empty;
                        designationService.SaveDesignation(designation);
                        return Json(new { isSuccess = true, message = "Update Successfully", lastCode = designation.DesignationCode });
                    }
                    else
                    {

                        return Json(new { isSuccess = false, message = "You have no access" });
                    }

                }
                else
                {
                    var hasPermission = designationService.SavePermission(LoginInfo.AccessCode);
                    if (hasPermission)
                    {
                        HrmDefDesignation designation = designationService.GetDesignation(model.DesignationCode) ?? new HrmDefDesignation();
                        model.ToAudit(LoginInfo, model.Id > 0);
                        mapper.Map(model, designation);
                        model.BanglaShortName = string.Empty;
                        model.BanglaDesignation = string.Empty;
                        model.CompanyCode = string.Empty;
                        model.EmployeeId = string.Empty;
                        model.MobileAllowanceId = string.Empty;
                        designationService.SaveDesignation(designation);
                        return Json(new { isSuccess = true, message = "Saved Successfully", lastCode = designation.DesignationCode });
                    }
                    else
                    {

                        return Json(new { isSuccess = false, message = "You have no access" });
                    }
                }


            }

            return Json(new { success = false, message = ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage });
        }

        public ActionResult Grid()
        {
            var resutl = designationService.GetDesignations();
            return Json(new { data = resutl });
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            var hasPermission = designationService.DeletePermission(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access" });
            }

            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(new { success = false, message = "No IDs provided" });
            }

            var results = new List<(bool success, bool refSuccess, string message)>();
            var ids = id.Split(",", StringSplitOptions.RemoveEmptyEntries)
                 .Select(x => x.Trim())
                 .ToList();

            DeleteHistoryViewModel model = new DeleteHistoryViewModel();
            model.ToAudit(LoginInfo);
            model.CompanyCode = LoginInfo.CompanyCode;

            var result = await designationService.DeleteDesignationAsync(ids, model);

            results.Add(result);

            // Aggregate outcome
            bool overallSuccess = results.All(r => r.success);
            bool refSuccess = results.All(r => r.refSuccess);
            string combinedMessage = overallSuccess
                ? "Deleted Successfully"
                : string.Join("; ", results.Where(r => !r.success).Select(r => r.message));

            return Json(new { success = overallSuccess, message = combinedMessage, refSuccess = refSuccess });
        }

        [HttpPost]
        public JsonResult CheckAvailability(string name, string code)
        {
            if (designationService.IsDesignationExist(name, code))
            {
                return Json(new { isSuccess = true, message = "Already Exists" });
            }

            return Json(new { isSuccess = false });
        }

        #region Xls
        public async Task<IActionResult> ExportToExcel()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Designations");

                // Add title
                worksheet.Cell(1, 1).Value = "Designation Information";
                worksheet.Range(1, 1, 1, 4).Merge(); // Merge cells across the header columns
                worksheet.Row(1).Style.Font.Bold = true;
                worksheet.Row(1).Style.Font.FontSize = 14;
                worksheet.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Row(1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                worksheet.Range(2, 1, 2, 4).Merge();
                // Leave an empty row
                int dataStartRow = 3;

                // Add headers
                worksheet.Cell(dataStartRow, 1).Value = "Designation Id";
                worksheet.Cell(dataStartRow, 2).Value = "Designation Name";
                worksheet.Cell(dataStartRow, 3).Value = "Short Name";
                worksheet.Cell(dataStartRow, 4).Value = "Designation (বাংলা)";
                worksheet.Row(dataStartRow).Style.Font.Bold = true;
                worksheet.Row(dataStartRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Row(dataStartRow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                // Add data
                var designations = designationService.GetDesignations();
                int row = dataStartRow + 1;
                foreach (var designation in designations)
                {


                    worksheet.Cell(row, 1).Value = "'" + designation.DesignationCode.PadLeft(2, '0');
                    worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Cell(row, 2).Value = designation.DesignationName;
                    worksheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    worksheet.Cell(row, 3).Value = designation.DesignationShortName;
                    worksheet.Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Cell(row, 4).Value = designation.BanglaDesignation;
                    worksheet.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    row++;
                }

                worksheet.Columns().AdjustToContents();

                // Save to a stream
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(),
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "Designations.xlsx");
                }
            }
        }

        #endregion

        #region Pdf

        public async Task<IActionResult> ExportToPdf()
        {           
            using (var stream = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(stream);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                // Add title
                PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                // Add Title
                Paragraph title = new Paragraph("Designation Information")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(12)
                    .SetFont(font);
                document.Add(title);

                // Add some space
                document.Add(new Paragraph("\n"));

                // Create a table with 4 columns
                Table table = new Table(UnitValue.CreatePercentArray(4)).UseAllAvailableWidth();

                // Add headers
                string[] headers = { "Designation Id", "Designation Name", "Short Name", "Designation (বাংলা)" };
                float headerFontSize = 10f;
                float dataFontSize = 9f;

                foreach (var header in headers)
                {
                    table.AddHeaderCell(
                        new Cell()
                            .Add(new Paragraph(header)
                                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                                .SetFontSize(headerFontSize))
                                .SetTextAlignment(TextAlignment.CENTER)
                                .SetBorder(new SolidBorder(0.5f)));
                }

                // Add data
                var designations = designationService.GetDesignations();
                foreach (var designation in designations)
                {
                    table.AddCell(new Cell()
                        .Add(new Paragraph(designation.DesignationCode?.PadLeft(2, '0') ?? "")
                            .SetFontSize(dataFontSize))
                        .SetTextAlignment(TextAlignment.CENTER));

                    table.AddCell(new Cell()
                        .Add(new Paragraph(designation.DesignationName ?? "")
                            .SetFontSize(dataFontSize))
                        .SetTextAlignment(TextAlignment.LEFT));

                    table.AddCell(new Cell()
                        .Add(new Paragraph(designation.DesignationShortName ?? "")
                            .SetFontSize(dataFontSize))
                        .SetTextAlignment(TextAlignment.CENTER));

                    table.AddCell(new Cell()
                        .Add(new Paragraph(designation.BanglaDesignation ?? "")
                            .SetFontSize(dataFontSize))
                        .SetTextAlignment(TextAlignment.LEFT));
                }

                // Add the table to the document
                document.Add(table);

                // Close the document
                document.Close();

                // Return the PDF as a file
                return File(stream.ToArray(), "application/pdf", "Designations.pdf");
            }
        }



        #endregion

    }
}