using ClosedXML.Excel;
using GCTL.Core.Data;
using GCTL.Core.ViewModels.EmployeeGeneralInfoReport;

using GCTL.Data.Models;
using GCTL.Service.EmployeeGeneralInfoReport;

using GCTL.Service.HrmEmployees2;
using GCTL.UI.Core.ViewModels.EmployeeGeneralInfoReport;

using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout.Borders;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Geom;
using System.Globalization;


namespace GCTL.UI.Core.Controllers
{
    public class EmployeeGeneralInfoReportController : BaseController
    {
        private readonly IEmployeeGeneralInfoReportService  employeeGeneralInfoReportService;
        private readonly IRepository<HrmEmployee> hrmEmployee;
        private readonly IRepository<CoreBranch> coreBranch;
        private readonly IRepository<CoreCompany> coreCompany;
        private readonly IRepository<HrmDefDepartment> deptment;
        private readonly IRepository<HrmDefDesignation> designation;
        private readonly IRepository<HrmEmployeeOfficialInfo> empOfficial;
        private readonly IHrmEmployee2Service hrmEmployee2Service;
        private readonly IRepository<HrmDefSex> sex;
        private readonly IRepository<HrmDefNationality> nationality;
        private readonly IRepository<HrmDefMaritalStatus> maritalStatus;
        private readonly IRepository<HrmDefReligion> religion;
        private readonly IRepository<HrmDefBloodGroup> bloodGroup;


        public EmployeeGeneralInfoReportController(IRepository<HrmEmployee> hrmEmployee,
            IRepository<CoreBranch> coreBranch,
            IRepository<CoreCompany> coreCompany,
            IRepository<HrmDefDepartment> deptment,
            IRepository<HrmDefDesignation> designation,
            IEmployeeGeneralInfoReportService  employeeGeneralInfoReportService,
            IHrmEmployee2Service hrmEmployee2Service,
            IRepository<HrmDefSex> sex,
            IRepository<HrmDefNationality> nationality,
            IRepository<HrmDefMaritalStatus> maritalStatus,
            IRepository<HrmDefReligion> religion,
            IRepository<HrmDefBloodGroup> bloodGroup)
        {
            this.hrmEmployee = hrmEmployee;
            this.coreBranch = coreBranch;
            this.coreCompany = coreCompany;
            this.deptment = deptment;
            this.designation = designation;
            this.hrmEmployee2Service = hrmEmployee2Service;
            this.employeeGeneralInfoReportService = employeeGeneralInfoReportService;
            this.sex = sex;
            this.nationality = nationality;
            this.maritalStatus = maritalStatus;
            this.religion = religion;
            this.bloodGroup = bloodGroup;
        }

        public async Task<IActionResult> Index(bool child = false)
        {
            var hasPermission = await employeeGeneralInfoReportService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var model = new EmployeeGeneralInfoReportPageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {

                model.Setup = new EmployeeGeneralInfoReportSetupViewModel();

            }
            catch (Exception ex)
            {


                model.Setup = new EmployeeGeneralInfoReportSetupViewModel();
                Console.WriteLine("Error" + ex.Message);
            }


            if (child)
                return PartialView(model);


            var comapanies = coreCompany.All().ToList();
            string selectedCompanyCode = comapanies.Count() == 1 ? comapanies.First().CompanyCode : null;
            ViewBag.CoreCompanyDD = new SelectList(coreCompany.All().ToList(), "CompanyCode", "CompanyName", selectedCompanyCode);

            var branch = coreBranch.All().ToList();
            var selectedBranch = branch.Count() == 1 ? branch.First().BranchCode : null;
            ViewBag.CoreBranchDD = new SelectList(coreBranch.All(), "BranchCode", "BranchName", selectedBranch);

           
            ViewBag.DeptDD = new SelectList(deptment.All(), "DepartmentCode", "DepartmentName");
            ViewBag.DesigDD = new SelectList(designation.All(), "DesignationCode", "DesignationName");
           
            ViewBag.EmpDD = new SelectList(await hrmEmployee2Service.GetEmployeeDropSelections(), "Code", "Name");
         

            var employeesWithNationalID = hrmEmployee.All().Where(x => !string.IsNullOrEmpty(x.NationalIdno)).ToList();
            Console.WriteLine("NationalIDNO Count: " + employeesWithNationalID.Count);

            ViewBag.NationalDD = employeesWithNationalID;

            ViewBag.MaritalStatusCodeDD = new SelectList(maritalStatus.All(), "MaritalStatusCode", "MaritalStatus");
            ViewBag.GenderCodeDD = new SelectList(sex.All(), "SexCode", "Sex");
            ViewBag.BloodGroupCodeDD = new SelectList(bloodGroup.All(), "BloodGroupCode", "BloodGroup");
            ViewBag.ReligionCodeDD = new SelectList(religion.All(), "ReligionCode", "Religion");
            ViewBag.NationalityCodeDD = new SelectList(nationality.All(), "NationalityCode", "Nationality");
            return View(model);
        }


        //public IActionResult Setup(int id)
        //{
        //    //try
        //    //{
        //    //    var model = new OfficialInfoViewModel();
        //    //    return PartialView("_Setup", model);
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    return Content($"Error: {ex.Message}");
        //    //}
        //}




        [HttpGet]
        public async Task<IActionResult> GetBranchByCode(string companyCode)
        {
            if (companyCode == null)
            {
                return Json(new { error = "No data found" });
            }
            var Codes = companyCode.Split(',').ToList();
            var result = await employeeGeneralInfoReportService.GetComapnyByBranchCode(Codes);
            if (result == null)
            {
                return Json(new { error = "No data found" });
            }
            return Json(result);
        }


        #region XLs

        [HttpGet]
        public async Task<IActionResult> Employee_Personal_Info_Report(string departmentCode, string designationCode,
            string employeeCode, string branchCode, string companyCode, 
            string nationalityCode, string genderCode, string bloodGroupCode,
        string religionCode, string maritalStatusCode

           )
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Employee Personal Info Report");

                // Row 1: "Data Path" centered
                worksheet.Cell(1, 1).Value = "Employee Personal Info Report";
                worksheet.Range(1, 1, 1, 17).Merge();
                worksheet.Row(1).Style.Font.Bold = true;
                worksheet.Row(1).Style.Font.FontSize = 14;
                worksheet.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                var dataRange = worksheet.Range(1, 1, 2, 17);
                dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                var departmentCodes = !string.IsNullOrEmpty(departmentCode) ? departmentCode.Split(',').ToList() : new List<string>();
                var designationCodes = !string.IsNullOrEmpty(designationCode) ? designationCode.Split(',').ToList() : new List<string>();
                var employeeCodes = !string.IsNullOrEmpty(employeeCode) ? employeeCode.Split(',').ToList() : new List<string>();
                var branchCodes = !string.IsNullOrEmpty(branchCode) ? branchCode.Split(',').ToList() : new List<string>();
                var companyCodes = !string.IsNullOrEmpty(companyCode) ? companyCode.Split(',').ToList() : new List<string>();

                var employees = await employeeGeneralInfoReportService.EmployeeGeneralInfoReport(departmentCodes, designationCodes, employeeCodes,
                    branchCodes, companyCodes,  nationalityCode,  genderCode,  bloodGroupCode, religionCode,  maritalStatusCode
                    );


                if (employees == null || !employees.Any())

                {
                    worksheet.Cell(3, 1).Value = "No Data Available";
                    worksheet.Range(3, 1, 3, 17).Merge();
                    worksheet.Row(3).Style.Font.Bold = true;
                    worksheet.Row(3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }
                else
                {
                    int currentRow = 2; // Start below headers
                    int grandTotal = 0;
                    foreach (var department in employees.GroupBy(e => e.CompanyName))

                    {
                        // Row: Display Department Name
                        worksheet.Cell(currentRow, 1).Value = "Company: " + department.Key;
                        worksheet.Range(currentRow, 1, currentRow, 16).Merge();
                        worksheet.Row(currentRow).Style.Font.Bold = true;
                        worksheet.Row(currentRow).Style.Font.FontSize = 15;
                        currentRow++;

                        // Headers
                        worksheet.Cell(currentRow, 1).Value = "Employee ID";
                        worksheet.Cell(currentRow, 2).Value = "Name";
                        worksheet.Cell(currentRow, 3).Value = " Father's Name";
                        worksheet.Cell(currentRow, 4).Value = "Mother's Name";
                        worksheet.Cell(currentRow, 5).Value = "Nationality";
                      
                        worksheet.Cell(currentRow, 6).Value = "National ID";
                        worksheet.Cell(currentRow, 7).Value = " Date of Birth";
                        worksheet.Cell(currentRow, 8).Value = "Place of Birth";
                        worksheet.Cell(currentRow, 8).Style.Alignment.WrapText = true;
                        worksheet.Cell(currentRow, 9).Value = "Sex";
                        worksheet.Cell(currentRow, 10).Value = "Blood Group";
                        worksheet.Cell(currentRow, 11).Value = "Religion";
                        worksheet.Cell(currentRow, 12).Value = "Present Address";
                        worksheet.Cell(currentRow, 12).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        worksheet.Cell(currentRow, 12).Style.Alignment.WrapText = true;
                        worksheet.Cell(currentRow, 13).Value = "Permanent Address";
                        worksheet.Cell(currentRow, 13).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        worksheet.Cell(currentRow, 13).Style.Alignment.WrapText = true;
                        worksheet.Cell(currentRow, 14).Value = "Personal Phone";
                        worksheet.Cell(currentRow, 15).Value = "Personal Email";
                        worksheet.Cell(currentRow, 16).Value = "TIN No";
                        worksheet.Cell(currentRow, 17).Value = "Marital Status";


                        worksheet.Row(currentRow).Style.Font.Bold = true;

                        worksheet.Row(currentRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;  // Centers horizontally

                        worksheet.Row(currentRow).Height = 60;
                        worksheet.Row(currentRow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center; 
                        worksheet.Row(currentRow).Style.Alignment.WrapText = true; 


                        var headerRange = worksheet.Range(currentRow, 1, currentRow, 17);
                        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        currentRow++;


                        foreach (var emp in department)
                        {


                            worksheet.Cell(currentRow, 1).Value = emp.EmployeeID;
                            worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                            var cell = worksheet.Cell(currentRow, 2);
                            cell.Value = emp.EmployeeName;
                            cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);


                            worksheet.Cell(currentRow, 3).Value = emp.FatherName;
                            worksheet.Cell(currentRow, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                            worksheet.Cell(currentRow, 4).Value = emp.MotherName;
                            worksheet.Cell(currentRow, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;



                            worksheet.Cell(currentRow, 5).Value = emp.Nationality;
                            worksheet.Cell(currentRow, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                           // worksheet.Cell(currentRow, 6).Value = emp.NationalIDNO;
                            worksheet.Cell(currentRow, 6).SetDataType(XLDataType.Text);
                            worksheet.Cell(currentRow, 6).Value ="'"+ emp.NationalIDNO.ToString();
                            worksheet.Cell(currentRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        

                            worksheet.Cell(currentRow, 7).Value = emp.DateOfBirthCertificate;
                            worksheet.Cell(currentRow, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                            worksheet.Cell(currentRow, 8).Value = emp.PlaceOfBirth;
                            worksheet.Cell(currentRow, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                            worksheet.Cell(currentRow, 9).Value = emp.Sex;
                            worksheet.Cell(currentRow, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                            worksheet.Cell(currentRow, 10).Value = emp.BloodGroup;
                            worksheet.Cell(currentRow, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                            worksheet.Cell(currentRow, 11).Value = emp.Religion;
                            worksheet.Cell(currentRow, 11).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                            worksheet.Cell(currentRow, 12).Value = emp.PresentAddress;
                            worksheet.Cell(currentRow, 12).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            worksheet.Cell(currentRow, 12).Style.Alignment.WrapText = true;

                            worksheet.Cell(currentRow, 13).Value = emp.ParmanentAddress;
                            worksheet.Cell(currentRow, 13).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            worksheet.Cell(currentRow, 13).Style.Alignment.WrapText = true;


                            worksheet.Cell(currentRow, 14).Value = "'" + emp.Telephone;
                            worksheet.Cell(currentRow, 14).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                            worksheet.Cell(currentRow, 15).Value = emp.PersonalEmail;
                            worksheet.Cell(currentRow, 15).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                            //worksheet.Cell(currentRow, 16).Value =emp.TinNo;
                            worksheet.Cell(currentRow, 16).SetDataType(XLDataType.Text);
                            worksheet.Cell(currentRow, 16).Value ="'" +emp.TinNo.ToString();
                            worksheet.Cell(currentRow, 16).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                            worksheet.Cell(currentRow, 17).Value = emp.MaritalStatus;
                            worksheet.Cell(currentRow, 17).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                            worksheet.Row(currentRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;



                            currentRow++;

                        }


                        worksheet.Cell(currentRow, 1).Value = "Total" + ": " + department.Count();
                        worksheet.Row(currentRow).Style.Font.FontSize = 14;
                        worksheet.Row(currentRow).Style.Font.Bold = true;
                        worksheet.Range(currentRow, 1, currentRow, 17).Merge();
                        grandTotal += department.Count();
                        currentRow += 1;
                    }

                    worksheet.Cell(currentRow, 1).Value = "Total Employees: " + grandTotal;
                    worksheet.Row(currentRow).Style.Font.FontSize = 14;
                    worksheet.Row(currentRow).Style.Font.Bold = true;
                    worksheet.Range(currentRow, 1, currentRow, 17).Merge();

                    currentRow++;

                    var dataRangee = worksheet.Range(3, 1, currentRow - 1, 17);
                    dataRangee.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    dataRangee.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    worksheet.Columns().AdjustToContents();
                    worksheet.Rows().AdjustToContents();
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employee_Personal_Info_Report.xlsx");
                }
            }
        }
        #endregion


        #region Pdf 


        // [HttpGet]
        // public async Task<IActionResult> Employee_Personal_Info_ReportPdf(

        //   string departmentCode, string designationCode,
        //     string employeeCode, string branchCode, string companyCode,
        //     string nationalityCode, string genderCode, string bloodGroupCode,
        // string religionCode, string maritalStatusCode

        //)
        // {
        //     using (var stream = new MemoryStream())
        //     {
        //         PdfWriter writer = new PdfWriter(stream);
        //         PdfDocument pdf = new PdfDocument(writer);
        //         Document document = new Document(pdf, PageSize.A4.Rotate());
        //         document.SetMargins(35, 35, 35, 35);

        //         PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

        //         Paragraph title = new Paragraph("Employee Personal Info Report")
        //             .SetTextAlignment(TextAlignment.CENTER)
        //             .SetFontSize(14)
        //             .SetFont(font);
        //         document.Add(title);


        //         document.Add(new Paragraph("\n"));
        //         var departmentCodes = !string.IsNullOrEmpty(departmentCode) ? departmentCode.Split(',').ToList() : new List<string>();
        //         var designationCodes = !string.IsNullOrEmpty(designationCode) ? designationCode.Split(',').ToList() : new List<string>();
        //         var employeeCodes = !string.IsNullOrEmpty(employeeCode) ? employeeCode.Split(',').ToList() : new List<string>();
        //         var branchCodes = !string.IsNullOrEmpty(branchCode) ? branchCode.Split(',').ToList() : new List<string>();
        //         var companyCodes = !string.IsNullOrEmpty(companyCode) ? companyCode.Split(',').ToList() : new List<string>();

        //         var employees = await employeeGeneralInfoReportService.EmployeeGeneralInfoReport(
        //             departmentCodes, designationCodes, employeeCodes,
        //             branchCodes, companyCodes, nationalityCode, genderCode, bloodGroupCode, religionCode, maritalStatusCode
        //             );
        //         int grandTotalEmployees = 0; // De
        //         if (employees == null || !employees.Any())
        //         {
        //             document.Add(new Paragraph("No Data Available").SetTextAlignment(TextAlignment.CENTER).SetFontSize(12));
        //         }
        //         else
        //         {



        //             foreach (var department in employees.GroupBy(e => e.DepartmentName))
        //             {
        //                 //  DepartmentName
        //                 document.Add(new Paragraph("Department: " + department.Key)
        //                     .SetFontSize(14)
        //                     .SetFont(font));
        //                 string[] headers = new string[]
        //                 {
        //                  "Employee ID", "Name", "Father's Name", "Mother's Name", "Nationality",
        //                  "National ID", "Date Of Birth", "Place Of Birth", "Sex",
        //                  "Blood Group", "Religion", "Present Address", "Permanent Address",
        //                 "Personal Phone", "Personal Email","TIN No", "Marital Status" 
        //                 };

        //                 // Get total columns and distribute width equally
        //                 float[] columnWidths = Enumerable.Repeat(1f, headers.Length).ToArray();

        //                 Table table = new Table(UnitValue.CreatePercentArray(columnWidths)).UseAllAvailableWidth();
        //                 table.SetKeepTogether(true);


        //                 // Add header cells
        //                 foreach (var header in headers)
        //                 {
        //                     table.AddHeaderCell(
        //                         new Cell()
        //                             .Add(new Paragraph(header)
        //                                 .SetFont(font)
        //                                 .SetFontSize(8f)
        //                                 .SetTextAlignment(TextAlignment.CENTER)
        //                                 .SetVerticalAlignment(VerticalAlignment.MIDDLE)
        //                             )
        //                             .SetBorder(new SolidBorder(0.5f))
        //                     );
        //                 }







        //                 foreach (var emp in department)
        //                 {
        //                     table.AddCell(new Cell().Add(new Paragraph(emp.EmployeeID ?? "")
        //                         .SetTextAlignment(TextAlignment.CENTER).SetFontSize(8f)));

        //                     table.AddCell(new Cell().Add(new Paragraph(emp.EmployeeName ?? "")
        //                         .SetTextAlignment(TextAlignment.LEFT)
        //                         .SetFontSize(8f)));

        //                     table.AddCell(new Cell().Add(new Paragraph(emp.FatherName ?? "")
        //                         .SetTextAlignment(TextAlignment.LEFT).SetFontSize(8f)));

        //                     table.AddCell(new Cell().Add(new Paragraph(emp.MotherName ?? "")
        //                         .SetTextAlignment(TextAlignment.LEFT)
        //                         .SetVerticalAlignment(VerticalAlignment.MIDDLE)
        //                         .SetFontSize(8f)));

        //                     table.AddCell(new Cell().Add(new Paragraph(emp.Nationality ?? "")
        //                         .SetTextAlignment(TextAlignment.CENTER)
        //                         .SetVerticalAlignment(VerticalAlignment.MIDDLE)
        //                         .SetFontSize(8f)));

        //                     table.AddCell(new Cell().Add(new Paragraph(emp.NationalIDNO ?? "")
        //                         .SetTextAlignment(TextAlignment.CENTER)
        //                         .SetVerticalAlignment(VerticalAlignment.MIDDLE)
        //                         .SetFontSize(8f)));


        //                     table.AddCell(new Cell().Add(new Paragraph(emp.DateOfBirthCertificate?.ToString("dd/MM/yyyy") ?? ""))
        //          .SetTextAlignment(TextAlignment.CENTER)
        //          .SetVerticalAlignment(VerticalAlignment.MIDDLE)
        //          .SetFontSize(8f));



        //                     table.AddCell(new Cell().Add(new Paragraph(emp.PlaceOfBirth) 
        //                              .SetTextAlignment(TextAlignment.CENTER)
        //                              .SetVerticalAlignment(VerticalAlignment.MIDDLE)
        //                              .SetFontSize(8f)));


        //                     table.AddCell(new Cell().Add(new Paragraph(emp.Sex ?? "")
        //                         .SetTextAlignment(TextAlignment.CENTER)
        //                         .SetVerticalAlignment(VerticalAlignment.MIDDLE)
        //                         .SetFontSize(8f)));

        //                     table.AddCell(new Cell().Add(new Paragraph(emp.BloodGroup ?? "")
        //                         .SetTextAlignment(TextAlignment.CENTER)
        //                         .SetVerticalAlignment(VerticalAlignment.MIDDLE)
        //                         .SetFontSize(8f)));

        //                     table.AddCell(new Cell().Add(new Paragraph(emp.Religion ?? "")
        //                         .SetTextAlignment(TextAlignment.CENTER)
        //                         .SetVerticalAlignment(VerticalAlignment.MIDDLE)
        //                         .SetFontSize(8f)));

        //                     table.AddCell(new Cell().Add(new Paragraph(emp.PresentAddress ?? "")
        //                         .SetTextAlignment(TextAlignment.CENTER)
        //                         .SetVerticalAlignment(VerticalAlignment.MIDDLE)
        //                         .SetFontSize(8f)));

        //                     table.AddCell(new Cell().Add(new Paragraph(emp.ParmanentAddress ?? "")
        //                         .SetTextAlignment(TextAlignment.CENTER)
        //                         .SetVerticalAlignment(VerticalAlignment.MIDDLE)
        //                         .SetFontSize(8f)));

        //                     table.AddCell(new Cell().Add(new Paragraph(emp.Telephone ?? "")
        //                         .SetTextAlignment(TextAlignment.CENTER)
        //                         .SetVerticalAlignment(VerticalAlignment.MIDDLE)
        //                         .SetFontSize(8f)));

        //                     table.AddCell(new Cell().Add(new Paragraph(emp.PersonalEmail ?? "")
        //                         .SetTextAlignment(TextAlignment.CENTER)
        //                         .SetVerticalAlignment(VerticalAlignment.MIDDLE)
        //                         .SetFontSize(8f)));

        //                     table.AddCell(new Cell().Add(new Paragraph(emp.TinNo ?? "")
        //                     .SetTextAlignment(TextAlignment.CENTER)
        //                     .SetVerticalAlignment(VerticalAlignment.MIDDLE)
        //                     .SetFontSize(8f)));

        //                     table.AddCell(new Cell().Add(new Paragraph(emp.MaritalStatus ?? "")
        //                   .SetTextAlignment(TextAlignment.CENTER)
        //                   .SetVerticalAlignment(VerticalAlignment.MIDDLE)
        //                   .SetFontSize(8f)));
        //                 }

        //                 document.Add(table);
        //                 int departmentTotal = department.Count();
        //                 grandTotalEmployees += departmentTotal;
        //                 document.Add(new Paragraph("Total: " + department.Count()).SetFontSize(8).SetFont(font));
        //                 document.Add(new Paragraph("\n"));
        //             }
        //             document.Add(new Paragraph("Total Employees: " + grandTotalEmployees).SetFontSize(14)
        //             .SetFont(font)
        //             .SetTextAlignment(TextAlignment.LEFT));
        //         }




        //         document.Close();

        //         // 3. Add Page Numbers After Closing
        //         PdfReader reader = new PdfReader(new MemoryStream(stream.ToArray()));
        //         MemoryStream finalStream = new MemoryStream();
        //         PdfWriter finalWriter = new PdfWriter(finalStream);
        //         PdfDocument finalPdf = new PdfDocument(reader, finalWriter);
        //         Document finalDocument = new Document(finalPdf);

        //         int totalPages = finalPdf.GetNumberOfPages();
        //         string dateTimeNow = $"Print Date: {DateTime.Now.ToString("dd/MM/yyyy  hh:mm:ss tt")}";
        //         for (int i = 1; i <= totalPages; i++)
        //         {
        //             finalDocument.ShowTextAligned(new Paragraph($"Page {i} of {totalPages}").SetFontSize(8f),
        //                 finalPdf.GetPage(i).GetPageSize().GetWidth() - 35, 20, i,
        //                 TextAlignment.RIGHT, VerticalAlignment.BOTTOM, 0);
        //             finalDocument.ShowTextAligned(new Paragraph(dateTimeNow).SetFontSize(8f), 35, 20, i, // Position: Bottom left
        //             TextAlignment.LEFT, VerticalAlignment.BOTTOM, 0);
        //         }

        //         finalDocument.Close();


        //         return File(finalStream.ToArray(), "application/pdf", "EmployeeGeneralInfoReport.pdf");
        //     }

        // }


        [HttpGet]
        public async Task<IActionResult> Employee_Personal_Info_ReportPdf(
    string departmentCode, string designationCode,
    string employeeCode, string branchCode, string companyCode,
    string nationalityCode, string genderCode, string bloodGroupCode,
    string religionCode, string maritalStatusCode)
        {
            using (var stream = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(stream);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf, PageSize.A4.Rotate());
                document.SetMargins(20, 20, 20, 20);

                PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                // Report Title
                document.Add(new Paragraph("Data Path")
                   .SetTextAlignment(TextAlignment.CENTER)
                   .SetFontSize(14)
                   .SetFont(font));

                document.Add(new Paragraph("Employee Personal Info Report")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(14)
                    .SetFont(font));

                document.Add(new Paragraph("\n"));

                var departmentCodes = !string.IsNullOrEmpty(departmentCode) ? departmentCode.Split(',').ToList() : new List<string>();
                var designationCodes = !string.IsNullOrEmpty(designationCode) ? designationCode.Split(',').ToList() : new List<string>();
                var employeeCodes = !string.IsNullOrEmpty(employeeCode) ? employeeCode.Split(',').ToList() : new List<string>();
                var branchCodes = !string.IsNullOrEmpty(branchCode) ? branchCode.Split(',').ToList() : new List<string>();
                var companyCodes = !string.IsNullOrEmpty(companyCode) ? companyCode.Split(',').ToList() : new List<string>();

                var employees = await employeeGeneralInfoReportService.EmployeeGeneralInfoReport(
                    departmentCodes, designationCodes, employeeCodes,
                    branchCodes, companyCodes, nationalityCode, genderCode, bloodGroupCode, religionCode, maritalStatusCode
                );

                int grandTotalEmployees = 0;
                if (employees == null || !employees.Any())
                {
                    document.Add(new Paragraph("No Data Available").SetTextAlignment(TextAlignment.CENTER).SetFontSize(12));
                }
                else
                {
                    bool isSingleCompany = companyCodes.Count == 1;
                    var groupedData = isSingleCompany ?
                        employees.GroupBy(e => e.DepartmentName) :
                        employees.GroupBy(e => e.CompanyName).SelectMany(company =>
                            company.GroupBy(e => e.DepartmentName));

                    foreach (var group in groupedData)
                    {
                        document.Add(new Paragraph(isSingleCompany ? $"Department: {group.Key}" : $"Company: {group.First().CompanyName}\nDepartment: {group.Key}")
                            .SetFontSize(14)
                            .SetFont(font));

                        string[] headers = new string[]
                        {
                    "Employee ID", "Name", "Father's Name", "Mother's Name", "Nationality",
                    "National ID", "Date Of Birth", "Place Of Birth", "Sex",
                    "Blood Group", "Religion", "Present Address", "Permanent Address",
                    "Personal Phone", "Personal Email","TIN No", "Marital Status"
                        };

                        float[] columnWidths = Enumerable.Repeat(1f, headers.Length).ToArray();
                        Table table = new Table(UnitValue.CreatePercentArray(columnWidths)).UseAllAvailableWidth();
                        table.SetKeepTogether(true);

                        // Add Table Headers
                        foreach (var header in headers)
                        {
                            table.AddHeaderCell(new Cell().Add(new Paragraph(header)
                                .SetFont(font)
                                .SetFontSize(8f)
                                .SetTextAlignment(TextAlignment.CENTER)
                                .SetVerticalAlignment(VerticalAlignment.MIDDLE))
                                .SetBorder(new SolidBorder(0.5f)));
                        }

                        // Add Data Rows
                        foreach (var emp in group)
                        {
                            table.AddCell(new Cell().Add(new Paragraph(emp.EmployeeID ?? "").SetTextAlignment(TextAlignment.CENTER).SetFontSize(8f)));
                            table.AddCell(new Cell().Add(new Paragraph(emp.EmployeeName ?? "").SetTextAlignment(TextAlignment.LEFT).SetFontSize(8f)));
                            table.AddCell(new Cell().Add(new Paragraph(emp.FatherName ?? "").SetTextAlignment(TextAlignment.LEFT).SetFontSize(8f)));
                            table.AddCell(new Cell().Add(new Paragraph(emp.MotherName ?? "").SetTextAlignment(TextAlignment.LEFT).SetFontSize(8f)));
                            table.AddCell(new Cell().Add(new Paragraph(emp.Nationality ?? "").SetTextAlignment(TextAlignment.CENTER).SetFontSize(8f)));
                           
                            table.AddCell(new Cell()
                .Add(new Paragraph(emp.NationalIDNO ?? "").SetTextAlignment(TextAlignment.LEFT).SetFontSize(8f)
                .SetWidth(30).SetMaxWidth(30).SetWordSpacing(1f).SetKeepTogether(false)).SetPadding(2));



                            DateTime dob;
                            bool isValidDate = DateTime.TryParse(emp.DateOfBirthCertificate, out dob); table.AddCell(new Cell().Add(new Paragraph(
                            isValidDate && dob != new DateTime(1900, 1, 1)? dob.ToString("dd/MM/yyyy"): "" ).SetTextAlignment(TextAlignment.CENTER) .SetVerticalAlignment(VerticalAlignment.MIDDLE) .SetFontSize(8f)));


                            table.AddCell(new Cell().Add(new Paragraph(emp.PlaceOfBirth ?? "").SetTextAlignment(TextAlignment.CENTER).SetFontSize(8f)));
                            table.AddCell(new Cell().Add(new Paragraph(emp.Sex ?? "").SetTextAlignment(TextAlignment.CENTER).SetFontSize(8f)));
                            table.AddCell(new Cell().Add(new Paragraph(emp.BloodGroup ?? "").SetTextAlignment(TextAlignment.CENTER).SetFontSize(8f)));
                            table.AddCell(new Cell().Add(new Paragraph(emp.Religion ?? "").SetTextAlignment(TextAlignment.CENTER).SetFontSize(8f)));
                            table.AddCell(new Cell().Add(new Paragraph(emp.PresentAddress ?? "").SetTextAlignment(TextAlignment.CENTER).SetFontSize(8f)));
                            table.AddCell(new Cell().Add(new Paragraph(emp.ParmanentAddress ?? "").SetTextAlignment(TextAlignment.CENTER).SetFontSize(8f)));
                            table.AddCell(new Cell().Add(new Paragraph(emp.Telephone ?? "").SetTextAlignment(TextAlignment.CENTER).SetFontSize(8f)));
                            table.AddCell(new Cell().Add(new Paragraph(emp.PersonalEmail ?? "").SetTextAlignment(TextAlignment.CENTER).SetFontSize(8f)));
                           // table.AddCell(new Cell().Add(new Paragraph(emp.TinNo ?? "").SetTextAlignment(TextAlignment.CENTER).SetFontSize(8f)));
                            table.AddCell(new Cell()
                     .Add(new Paragraph(emp.TinNo ?? "").SetTextAlignment(TextAlignment.LEFT).SetFontSize(8f)
                     .SetWidth(30).SetMaxWidth(30).SetWordSpacing(1f).SetKeepTogether(false)).SetPadding(2));

                            table.AddCell(new Cell().Add(new Paragraph(emp.MaritalStatus ?? "").SetTextAlignment(TextAlignment.CENTER).SetFontSize(8f)));
                        }

                        document.Add(table);
                        int departmentTotal = group.Count();
                        grandTotalEmployees += departmentTotal;
                        document.Add(new Paragraph($"Total: {departmentTotal}").SetFontSize(8).SetFont(font));
                        document.Add(new Paragraph("\n"));
                    }

                    document.Add(new Paragraph($"Total Employees: {grandTotalEmployees}").SetFontSize(14)
                        .SetFont(font)
                        .SetTextAlignment(TextAlignment.LEFT));
                }

                document.Close();

                // Add Page Numbers
                PdfReader reader = new PdfReader(new MemoryStream(stream.ToArray()));
                MemoryStream finalStream = new MemoryStream();
                PdfWriter finalWriter = new PdfWriter(finalStream);
                PdfDocument finalPdf = new PdfDocument(reader, finalWriter);
                Document finalDocument = new Document(finalPdf);

                int totalPages = finalPdf.GetNumberOfPages();
                string dateTimeNow = $"Print Date: {DateTime.Now:dd/MM/yyyy  hh:mm:ss tt}";

                for (int i = 1; i <= totalPages; i++)
                {
                    finalDocument.ShowTextAligned(new Paragraph($"Page {i} of {totalPages}").SetFontSize(8f),
                        finalPdf.GetPage(i).GetPageSize().GetWidth() - 35, 20, i,
                        TextAlignment.RIGHT, VerticalAlignment.BOTTOM, 0);
                    finalDocument.ShowTextAligned(new Paragraph(dateTimeNow).SetFontSize(8f), 35, 20, i,
                        TextAlignment.LEFT, VerticalAlignment.BOTTOM, 0);
                }

                finalDocument.Close();

                return File(finalStream.ToArray(), "application/pdf", "EmployeeGeneralInfoReport.pdf");
            }
        }



        #endregion

        #region Preview
        [HttpGet]
        public async Task<IActionResult> ExportEmployeeInfoToPdfPreView(string departmentCode, string designationCode,
            string employeeCode, string branchCode, string companyCode,
            string nationalityCode, string genderCode, string bloodGroupCode,
        string religionCode, string maritalStatusCode, bool preview = false)
        {
            using (var stream = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(stream);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf, PageSize.A4.Rotate());
                document.SetMargins(20, 20, 20, 20);

                PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                // Add Report Title
                Paragraph title = new Paragraph("Employee Personal Info Report")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(14)
                    .SetFont(font);
                document.Add(title);


                document.Add(new Paragraph("\n"));

                // Fetch employee data
                var departmentCodes = !string.IsNullOrEmpty(departmentCode) ? departmentCode.Split(',').ToList() : new List<string>();
                var designationCodes = !string.IsNullOrEmpty(designationCode) ? designationCode.Split(',').ToList() : new List<string>();
                var employeeCodes = !string.IsNullOrEmpty(employeeCode) ? employeeCode.Split(',').ToList() : new List<string>();
                var branchCodes = !string.IsNullOrEmpty(branchCode) ? branchCode.Split(',').ToList() : new List<string>();
                var companyCodes = !string.IsNullOrEmpty(companyCode) ? companyCode.Split(',').ToList() : new List<string>();

                var employees = await employeeGeneralInfoReportService.EmployeeGeneralInfoReport(
                    departmentCodes, designationCodes, employeeCodes, branchCodes, companyCodes, nationalityCode, genderCode, bloodGroupCode, religionCode, maritalStatusCode
                );

                int grandTotal = 0;
                if (employees == null || !employees.Any())
                {
                    document.Add(new Paragraph("No Data Available").SetTextAlignment(TextAlignment.CENTER).SetFontSize(12));
                }
                else
                {
                    //

                    foreach (var department in employees.GroupBy(e => e.CompanyName))
                    {
                        document.Add(new Paragraph("Department: " + department.Key).SetFontSize(14).SetFont(font));

                        float[] columnWidths = { 8, 12, 12, 10, 10, 12, 12, 10, 12, 15, 12, 12, 10, 10, 10,12,10 };
                        Table table = new Table(UnitValue.CreatePercentArray(columnWidths)).UseAllAvailableWidth();
                        table.SetKeepTogether(true);

                        string[] headers = {  "Employee ID", "Name", "Father's Name", "Mother's Name", "Nationality",
                         "National ID", "Date Of Birth", "Place Of Birth", "Sex",
                         "Blood Group", "Religion", "Present Address", "Permanent Address",
                        "Personal Phone", "Personal Email","TIN No", "Marital Status"  };

                        foreach (var header in headers)
                        {
                            table.AddHeaderCell(new Cell().Add(new Paragraph(header)
                                .SetFont(font)
                                .SetFontSize(8f)
                                .SetTextAlignment(TextAlignment.CENTER)
                                .SetVerticalAlignment(VerticalAlignment.MIDDLE))
                                .SetBorder(new SolidBorder(0.5f)));
                        }


                        foreach (var emp in department)
                        {
                            table.AddCell(new Cell().Add(new Paragraph(emp.EmployeeID ?? "").SetTextAlignment(TextAlignment.CENTER).SetFontSize(8f)));
                            table.AddCell(new Cell().Add(new Paragraph(emp.EmployeeName ?? "").SetTextAlignment(TextAlignment.LEFT).SetFontSize(8f)));
                            table.AddCell(new Cell().Add(new Paragraph(emp.FatherName ?? "").SetTextAlignment(TextAlignment.LEFT).SetFontSize(8f)));
                            table.AddCell(new Cell().Add(new Paragraph(emp.MotherName ?? "").SetTextAlignment(TextAlignment.LEFT).SetFontSize(8f)));
                            table.AddCell(new Cell().Add(new Paragraph(emp.Nationality ?? "").SetTextAlignment(TextAlignment.CENTER).SetFontSize(8f)));
                            //table.AddCell(new Cell().Add(new Paragraph(emp.NationalIDNO ?? "").SetTextAlignment(TextAlignment.CENTER).SetFontSize(8f)));


                table.AddCell(new Cell()
                .Add(new Paragraph(emp.NationalIDNO ?? "").SetTextAlignment(TextAlignment.LEFT).SetFontSize(8f)
                .SetWidth(30).SetMaxWidth(30).SetWordSpacing(1f).SetKeepTogether(false)).SetPadding(2));

                            string birthDateStr = "";
                            if (DateTime.TryParse(Convert.ToString(emp.DateOfBirthCertificate), out DateTime joiningDate))
                            {
                                birthDateStr = joiningDate.ToString("dd/MM/yyyy");
                            }


                            table.AddCell(
                                new Cell().Add(
                                    new Paragraph(birthDateStr)
                                        .SetTextAlignment(TextAlignment.CENTER)
                                        .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                                        .SetFontSize(8f)
                                )
                            );

                           
                            //
                            table.AddCell(new Cell().Add(new Paragraph(emp.PlaceOfBirth ?? "").SetTextAlignment(TextAlignment.CENTER).SetFontSize(8f)));
                            table.AddCell(new Cell().Add(new Paragraph(emp.Sex ?? "").SetTextAlignment(TextAlignment.CENTER).SetFontSize(8f)));
                            table.AddCell(new Cell().Add(new Paragraph(emp.BloodGroup ?? "").SetTextAlignment(TextAlignment.CENTER).SetFontSize(8f)));
                            table.AddCell(new Cell().Add(new Paragraph(emp.Religion ?? "").SetTextAlignment(TextAlignment.CENTER).SetFontSize(8f)));
                            table.AddCell(new Cell().Add(new Paragraph(emp.PresentAddress ?? "").SetTextAlignment(TextAlignment.CENTER).SetFontSize(8f)));
                            table.AddCell(new Cell().Add(new Paragraph(emp.ParmanentAddress ?? "").SetTextAlignment(TextAlignment.CENTER).SetFontSize(8f)));
                            table.AddCell(new Cell().Add(new Paragraph(emp.Telephone ?? "").SetTextAlignment(TextAlignment.CENTER).SetFontSize(8f)));
                            table.AddCell(new Cell().Add(new Paragraph(emp.PersonalEmail ?? "").SetTextAlignment(TextAlignment.CENTER).SetFontSize(8f)));
                            // table.AddCell(new Cell().Add(new Paragraph(emp.TinNo ?? "").SetTextAlignment(TextAlignment.CENTER).SetFontSize(8f)));


                            table.AddCell(new Cell()
                             .Add(new Paragraph(emp.TinNO ?? "").SetTextAlignment(TextAlignment.LEFT).SetFontSize(8f)
                             .SetWidth(30).SetMaxWidth(30).SetWordSpacing(1f).SetKeepTogether(false)).SetPadding(2));

                            table.AddCell(new Cell().Add(new Paragraph(emp.MaritalStatus ?? "").SetTextAlignment(TextAlignment.CENTER).SetFontSize(8f)));
                        }

                        document.Add(table);
                        document.Add(new Paragraph($"Total: {department.Count()}").SetFontSize(8).SetFont(font));
                        document.Add(new Paragraph("\n"));
                        grandTotal += department.Count();
                    }
                    document.Add(new Paragraph("Total Employees: " + grandTotal)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetFontSize(10)
                .SetFont(font));
                }

                document.Close();
                // 3. Add Page Numbers After Closing
                PdfReader reader = new PdfReader(new MemoryStream(stream.ToArray()));
                MemoryStream finalStream = new MemoryStream();
                PdfWriter finalWriter = new PdfWriter(finalStream);
                PdfDocument finalPdf = new PdfDocument(reader, finalWriter);
                Document finalDocument = new Document(finalPdf);

                int totalPages = finalPdf.GetNumberOfPages();
                string dateTimeNow = $"Print Date: {DateTime.Now.ToString("dd/MM/yyyy  hh:mm:ss tt", CultureInfo.InvariantCulture)}";
                for (int i = 1; i <= totalPages; i++)
                {
                    finalDocument.ShowTextAligned(new Paragraph($"Page {i} of {totalPages}").SetFontSize(9f),
                        finalPdf.GetPage(i).GetPageSize().GetWidth() - 35, 20, i,
                        TextAlignment.RIGHT, VerticalAlignment.BOTTOM, 0);
                    finalDocument.ShowTextAligned(new Paragraph(dateTimeNow).SetFontSize(9f), 35, 20, i, 
                    TextAlignment.LEFT, VerticalAlignment.BOTTOM, 0);
                }

                finalDocument.Close();

                if (preview)
                {
                    return File(finalStream.ToArray(), "application/pdf");
                }

                return File(finalStream.ToArray(), "application/pdf", "Employee_Personal_Info_Report.pdf");
            }
        }

        #endregion


        #region  GetBranchByCompany
        [HttpGet]
        public async Task<IActionResult> GetBranchByCompany(string companyId)
        {
            if (companyId == null || !companyId.Any())
            {
                var x = coreBranch.All().ToList();
                return Json(x);
            }

            List<string> companyIds = companyId.Split(',').ToList();
            if (companyIds == null || !companyIds.Any())
            {
                return Json(new { message = "No valid compnay provided" });
            }
            var result = await employeeGeneralInfoReportService.GetBranchByCompanyId(companyIds);
            return Json(result);
        }
        #endregion


        #region  GetDepartmentByCompany
        [HttpGet]
        public async Task<IActionResult> GetDepartmentByCompany(string companyId)
        {
            if (companyId == null || !companyId.Any())
            {
                var x = deptment.GetAll().ToList();
                return Json(x);
            }

            List<string> companyIds = companyId.Split(',').ToList();
            if (companyIds == null || !companyIds.Any())
            {
                return Json(new { message = "No valid compnay provided" });
            }
            var result = await employeeGeneralInfoReportService.GetDepartmentByCompanyId(companyIds);
            return Json(result);
        }
        #endregion


        #region  GetDesignationByCompany
        [HttpGet]
        public async Task<IActionResult> GetDesignationByCompany(string companyId)
        {
            if (companyId == null || !companyId.Any())
            {
                var x = designation.GetAll().ToList();
                return Json(x);
            }

            List<string> companyIds = companyId.Split(',').ToList();
            if (companyIds == null || !companyIds.Any())
            {
                return Json(new { message = "No valid compnay provided" });
            }
            var result = await employeeGeneralInfoReportService.GetDesignationByCompanyId(companyIds);
            return Json(result);
        }
        #endregion


        #region  GetEmployeeByCompany
        [HttpGet]
        public async Task<IActionResult> GetEmployeeByCompany(string companyId)
        {
            if (companyId == null || !companyId.Any())
            {
                var x = hrmEmployee.All().Select(e => new
                {
                    EmployeeId = e.EmployeeId,
                    EmployeeName = $"{e.FirstName} {e.LastName}"
                }).ToList();
                return Json(x);
            }

            List<string> companyIds = companyId.Split(',').ToList();
            if (companyIds == null || !companyIds.Any())
            {
                return Json(new { message = "No valid compnay provided" });
            }
            var result = await employeeGeneralInfoReportService.GetEmployeeByCompanyId(companyIds);
            return Json(result);
        }
        #endregion


        #region GetDepartmentByBranchId
        [HttpGet]
        public async Task<IActionResult> GetDepartmentByBranch(string branchId)
        {
            if (branchId == null || !branchId.Any())
            {
                var x = deptment.GetAll().ToList();
                return Json(x);
            }
            List<string> branchIds = branchId.Split(',').ToList();
            if (branchIds == null || !branchIds.Any())
            {
                return Json(new { message = "No valid branch provided" });
            }
            var result = await employeeGeneralInfoReportService.GetDepartmentByBranchId(branchIds);
            return Json(result);
        }
        #endregion


        #region GetDesignationByBranch
        [HttpGet]
        public async Task<IActionResult> GetDesignationByBranch(string branchId)
        {
            if (branchId == null || !branchId.Any())
            {
                var x = designation.GetAll().ToList();
                return Json(x);
            }
            List<string> branchIds = branchId.Split(',').ToList();
            if (branchIds == null || !branchIds.Any())
            {
                return Json(new { message = "No valid branch provided" });
            }
            var result = await employeeGeneralInfoReportService.GetDesignationByBranchId(branchIds);
            return Json(result);
        }
        #endregion


        #region GetEmployeeByBranch
        [HttpGet]
        public async Task<IActionResult> GetEmployeeByBranch(string branchId)
        {
            if (branchId == null || !branchId.Any())
            {
                var x = hrmEmployee.All().Select(e => new
                {
                    EmployeeId = e.EmployeeId,
                    EmployeeName = $"{e.FirstName} {e.LastName}"
                }).ToList();
                return Json(x);
            }
            List<string> branchIds = branchId.Split(',').ToList();
            if (branchIds == null || !branchIds.Any())
            {
                return Json(new { message = "No valid branch provided" });
            }
            var result = await employeeGeneralInfoReportService.GetEmployeeByBranchId(branchIds);
            return Json(result);
        }
        #endregion


        #region GetDesignationByDepartment
        [HttpGet]
        public async Task<IActionResult> GetDesignationByDepartment(string departmentId)
        {
            if (departmentId == null || !departmentId.Any())
            {
                var x = designation.GetAll().ToList();
                return Json(x);
            }
            List<string> departmentIds = departmentId.Split(',').ToList();
            if (departmentIds == null || !departmentIds.Any())
            {
                return Json(new { message = "No valid department provided" });
            }
            var result = await employeeGeneralInfoReportService.GetDesignationByDepartmentId(departmentIds);
            return Json(result);
        }
        #endregion


        #region GetEmployeeByDepartmentId
        [HttpGet]
        public async Task<IActionResult> GetEmployeeByDepartment(string departmentId)
        {
            if (departmentId == null || !departmentId.Any())
            {
                var x = hrmEmployee.All().Select(e => new
                {
                    EmployeeId = e.EmployeeId,
                    EmployeeName = $"{e.FirstName} {e.LastName}"
                }).ToList();
                return Json(x);
            }
            List<string> departmentIds = departmentId.Split(',').ToList();
            if (departmentIds == null || !departmentIds.Any())
            {
                return Json(new { message = "No valid department provided" });
            }
            var result = await employeeGeneralInfoReportService.GetEmployeeByDepartmentId(departmentIds);
            return Json(result);
        }
        #endregion


        #region GetEmployeeByDesignation
        [HttpGet]
        public async Task<IActionResult> GetEmployeeByDesignation(string designationId)
        {
            if (designationId == null || !designationId.Any())
            {
                var x = hrmEmployee.All().Select(e => new
                {
                    EmployeeId = e.EmployeeId,
                    EmployeeName = $"{e.FirstName} {e.LastName}"
                }).ToList();
                return Json(x);
            }
            List<string> designationIds = designationId.Split(',').ToList();
            if (designationIds == null || !designationIds.Any())
            {
                return Json(new { message = "No valid designation provided" });
            }
            var result = await employeeGeneralInfoReportService.GetEmployeeByDesignationId(designationIds);
            return Json(result);
        }
        #endregion

        //


    }
}
