using AutoMapper;
using ClosedXML.Excel;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.Departments;
using GCTL.Data.Models;
using GCTL.Service.Common;
using GCTL.Service.Departments;
using GCTL.Service.Designations;
using GCTL.UI.Core.ViewModels.Departments;
using iText.IO.Font;
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
    public class DepartmentsController : BaseController
    {
        private readonly IDepartmentService departmentService;
        private readonly ICommonService commonService;
        private readonly IMapper mapper;
        string strMaxNO = "";
        public DepartmentsController(
            IDepartmentService departmentService,
            ICommonService commonService,
            IMapper mapper
            
        )
        {
            this.departmentService = departmentService;
            this.commonService = commonService;
            this.mapper = mapper;
        }

        public IActionResult Index(bool child = false)
        {
            DepartmentPageViewModel model = new DepartmentPageViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };
            commonService.FindMaxNo(ref strMaxNO, "DepartmentCode", "HRM_Def_Department", 3);
            model.Setup = new DepartmentSetupViewModel
            {
                DepartmentCode = strMaxNO
            };

            if (child)
                return PartialView(model);

            return View(model);
        }


        public IActionResult Setup(string id)
        {
            DepartmentSetupViewModel model = new DepartmentSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "DepartmentCode", "HRM_Def_Department", 3);
            var result = departmentService.GetDepartment(id);
            if (result != null)
            {
                model = mapper.Map<DepartmentSetupViewModel>(result);
                model.Code = id;
                model.Id = (int)result.AutoId;
            }
            else
            {
                model.DepartmentCode = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Setup(DepartmentSetupViewModel model)
        {
            if (departmentService.IsDepartmentExist(model.DepartmentName, model.DepartmentCode))
            {
                return Json(new { isSuccess = false, message = "Already Exists" });
            }

            if (ModelState.IsValid)
            {
                if (departmentService.IsDepartmentExistByCode(model.DepartmentCode))
                {
                    var hasPermission = departmentService.UpdatePermission(LoginInfo.AccessCode);
                    if (hasPermission)
                    {
                        HrmDefDepartment department = departmentService.GetDepartment(model.DepartmentCode) ?? new HrmDefDepartment();
                        model.ToAudit(LoginInfo, model.Id > 0);
                        mapper.Map(model, department);

                        departmentService.SaveDepartment(department);
                        return Json(new { isSuccess = true, message = "Saved Successfully", lastCode = department.DepartmentCode });
                    }
                    else
                    {

                        return Json(new { isSuccess = false, message = "You have no access" });
                    }

                }
                else
                {
                    var hasPermission = departmentService.SavePermission(LoginInfo.AccessCode);
                    if (hasPermission)
                    {
                        HrmDefDepartment department = departmentService.GetDepartment(model.DepartmentCode) ?? new HrmDefDepartment();
                        model.ToAudit(LoginInfo, model.Id > 0);
                        mapper.Map(model, department);
                        departmentService.SaveDepartment(department);
                        return Json(new { isSuccess = true, message = "Saved Successfully", lastCode = department.DepartmentCode });
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
            var result = departmentService.GetDepartments();
            return Json(new { data = result });
        }


        [HttpPost]

        public async Task<ActionResult> Delete(string id)

        {

            var hasPermission = departmentService.DeletePermission(LoginInfo.AccessCode);

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
            var result = await departmentService.DeleteDepartment(ids, model);

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
            if (departmentService.IsDepartmentExist(name, code))
            {
                return Json(new { isSuccess = true, message = "Already Exists!" });
            }

            return Json(new { isSuccess = false });
        }


        #region Xls
        public async Task<IActionResult> ExportToExcel()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Departments");

                // Add title
                worksheet.Cell(1, 1).Value = "Department Information";
                worksheet.Range(1, 1, 1, 4).Merge(); // Merge cells across the header columns
                worksheet.Row(1).Style.Font.Bold = true;
                worksheet.Row(1).Style.Font.FontSize = 14;
                worksheet.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Row(1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                worksheet.Range(2, 1, 2, 4).Merge();
                // Leave an empty row
                int dataStartRow = 3;

                // Add headers
                worksheet.Cell(dataStartRow, 1).Value = "Department Id";
                worksheet.Cell(dataStartRow, 2).Value = "Department Name";
                worksheet.Cell(dataStartRow, 3).Value = "Short Name";
                worksheet.Cell(dataStartRow, 4).Value = "Department (বাংলা)";
                worksheet.Row(dataStartRow).Style.Font.Bold = true;
                worksheet.Row(dataStartRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Row(dataStartRow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                // Add data
                var designations = departmentService.GetDepartments();
                int row = dataStartRow + 1;
                foreach (var designation in designations)
                {
                    // worksheet.Cell(row, 1).Value = designation.DepartmentCode;
                    //worksheet.Cell(row, 1).Value = designation.DepartmentCode.PadLeft(2, '0');
                    worksheet.Cell(row, 1).Value = "'" + designation.DepartmentCode.PadLeft(2, '0');


                    worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Cell(row, 2).Value = designation.DepartmentName;
                    worksheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    worksheet.Cell(row, 3).Value = designation.DepartmentShortName;
                    worksheet.Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Cell(row, 4).Value = designation.BanglaDepartment;
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
                                "Departments.xlsx");
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
                Paragraph title = new Paragraph("Department Information")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(12)
                    .SetFont(font);
                document.Add(title);

                // Add some space
                document.Add(new Paragraph("\n"));

                // Create a table with 4 columns
                Table table = new Table(UnitValue.CreatePercentArray(4)).UseAllAvailableWidth();

                // Add headers
                string[] headers = { "Department Id", "Department Name", "Short Name", "Department (বাংলা)" };
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
                var departments = departmentService.GetDepartments();
                foreach (var department in departments)
                {
                    table.AddCell(new Cell()
                        .Add(new Paragraph(department.DepartmentCode.PadLeft(2, '0') ?? "")
                            .SetFontSize(dataFontSize))
                        .SetTextAlignment(TextAlignment.CENTER));

                    table.AddCell(new Cell()
                        .Add(new Paragraph(department.DepartmentName ?? "")
                            .SetFontSize(dataFontSize))
                        .SetTextAlignment(TextAlignment.LEFT));

                    table.AddCell(new Cell()
                        .Add(new Paragraph(department.DepartmentShortName ?? "")
                            .SetFontSize(dataFontSize))
                        .SetTextAlignment(TextAlignment.CENTER));

                    table.AddCell(new Cell()
                        .Add(new Paragraph(department.BanglaDepartment ?? "")
                            .SetFontSize(dataFontSize))
                        .SetTextAlignment(TextAlignment.LEFT));
                }

                // Add the table to the document
                document.Add(table);

                // Close the document
                document.Close();

                // Return the PDF as a file
                return File(stream.ToArray(), "application/pdf", "Departments.pdf");
            }
        }




        #endregion
    }
}