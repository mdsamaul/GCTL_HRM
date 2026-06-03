using ClosedXML.Excel;
using GCTL.Core.Data;
using GCTL.Core.ViewModels.EmployeeOfficialInfoReport;
using GCTL.Data.Models;
using GCTL.Service.EmployeeOfficialInfoReport;
using GCTL.Service.HrmEmployees2;
using GCTL.UI.Core.ViewModels.EmployeeOfficialInfoReport;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace GCTL.UI.Core.Controllers
{
    public class EmployeeOfficialInfoReportController : BaseController
    {
        private readonly IEmployeeOfficialInfoReportService employeeOfficialInfoReportService;
        private readonly IRepository<HrmEmployee> hrmEmployee;
        private readonly IRepository<CoreBranch> coreBranch;
        private readonly IRepository<CoreCompany> coreCompany;
        private readonly IRepository<HrmDefDepartment> deptment;
        private readonly IRepository<HrmDefDesignation> designation;
        private readonly IRepository<HrmAtdShift> shift;
        private readonly IRepository<HrmEmployeeOfficialInfo> empOfficial;
        private readonly IRepository<HrmLeaveApplicationEntry> leaveEntry;
        private readonly IRepository<HrmSeparation> separation;
        private readonly IRepository<HrmDefEmpType> empType;
        private readonly IRepository<HrmEmployeeAdditionalInfo> empAddInfo;
        private readonly IRepository<HrmEisDefEmploymentNature> empNature;
        private readonly IRepository<HrmDefEmployeeStatus> _employeeStatusRepository;
        private readonly IHrmEmployee2Service hrmEmployee2Service;
        private readonly IRepository<CorePeriodInfo> corePeriodInforepository;

        public EmployeeOfficialInfoReportController(IRepository<HrmEmployee> hrmEmployee,
            IRepository<CoreBranch> coreBranch,
            IRepository<CoreCompany> coreCompany,
            IRepository<HrmDefDepartment> deptment,
            IRepository<HrmDefDesignation> designation,
            IRepository<HrmAtdShift> shift,
            IRepository<HrmEmployeeOfficialInfo> empOfficial,
            IRepository<HrmLeaveApplicationEntry> leaveEntry,
            IRepository<HrmSeparation> separation,
            IRepository<HrmDefEmpType> empType,
            IRepository<HrmEmployeeAdditionalInfo> empAddInfo,
            IEmployeeOfficialInfoReportService employeeOfficialInfoReportService,
            IRepository<HrmEisDefEmploymentNature> empNature,
            IRepository<HrmDefEmployeeStatus> employeeStatusRepository,
            IRepository<CorePeriodInfo> corePeriodInforepository,
            IHrmEmployee2Service hrmEmployee2Service)
        {
            this.hrmEmployee = hrmEmployee;
            this.coreBranch = coreBranch;
            this.coreCompany = coreCompany;
            this.deptment = deptment;
            this.designation = designation;
            this.shift = shift;
            this.empOfficial = empOfficial;
            this.leaveEntry = leaveEntry;
            this.separation = separation;
            this.empType = empType;
            this.empAddInfo = empAddInfo;
            this.empNature = empNature;
            _employeeStatusRepository = employeeStatusRepository;
            this.hrmEmployee2Service = hrmEmployee2Service;
            this.corePeriodInforepository = corePeriodInforepository;
            this.employeeOfficialInfoReportService = employeeOfficialInfoReportService;
        }

        public async Task<IActionResult> Index(bool child = false)
        {
            //var hasPermission = await hrmDefExamtitleService.PagePermissionAsync(LoginInfo.AccessCode);
            //if (!hasPermission)
            //{
            //    return RedirectToAction("Login", "Accounts");
            //}

            var model = new EmployeeOfficialInfoReportPageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {

                model.Setup = new EmployeeOfficialInfoReportSetupViewModel();

            }
            catch (Exception ex)
            {


                model.Setup = new EmployeeOfficialInfoReportSetupViewModel();
                Console.WriteLine("Error" + ex.Message);
            }

            if (child)
                return PartialView(model);

            return View(model);
        }

        public IActionResult Setup(int id)
        {
            try
            {
                var model = new OfficialInfoViewModel();
                return PartialView("_Setup", model);
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message}");
            }
        }



        [HttpPost]
        public async Task<IActionResult> GetDropdown([FromBody] OfficialInfoFilterVm filter)
        {
            try
            {
                var result = await employeeOfficialInfoReportService.GetOfficialInfoDropdownAsync(filter);

                if (result != null)
                {
                    return Json(new { isSuccess = true, message = "successed data load", data = result });
                }
                return Json(new { isSuccess = false, message = "Data load Failed" });
            }
            catch (Exception)
            {

                throw;
            }

        }

        #region Excel

        [HttpPost]
        public async Task<IActionResult> GetEmployeeOfficialInfo([FromBody] OfficialInfoReportFilterVm filter)
        {
            try
            {
                if (filter == null)
                    return BadRequest("Filter data is required");

                var groupedData = await employeeOfficialInfoReportService.GetEmployeeOfficialInfoReport(filter);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Employee Official Info Report");

                    // Set column widths
                    worksheet.Column(1).Width = 15;
                    worksheet.Column(2).Width = 25;
                    worksheet.Column(3).Width = 22;
                    worksheet.Column(4).Width = 22;
                    worksheet.Column(5).Width = 18;
                    worksheet.Column(6).Width = 16;
                    worksheet.Column(7).Width = 14;
                    worksheet.Column(8).Width = 14;
                    worksheet.Column(9).Width = 14;
                    worksheet.Column(10).Width = 12;
                    worksheet.Column(11).Width = 25;
                    worksheet.Column(12).Width = 25;
                    worksheet.Column(13).Width = 20;
                    worksheet.Column(14).Width = 28;
                    worksheet.Column(15).Width = 18;


                    var companyInfo = await GetCompanyInfoAsync();

                    // Row-1: Company Name
                    var row1 = worksheet.Cells[1, 1, 1, 15];
                    row1.Merge = true;
                    row1.Value = companyInfo.CompanyName ?? "Data Path";
                    row1.Style.Font.Bold = true;
                    row1.Style.Font.Size = 14;
                    row1.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    row1.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    row1.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    row1.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    row1.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);

                    // Row-2: Company Address
                    var row2 = worksheet.Cells[2, 1, 2, 15];
                    row2.Merge = true;
                    row2.Value = companyInfo.Address ?? "";
                    row2.Style.Font.Bold = false;
                    row2.Style.Font.Size = 10;
                    row2.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    row2.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    row2.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    row2.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    row2.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);

                    // Row-3: Report Title
                    var row3 = worksheet.Cells[3, 1, 3, 15];
                    row3.Merge = true;
                    row3.Value = "Employee Official Info Report";
                    row3.Style.Font.Bold = true;
                    row3.Style.Font.Size = 15;
                    row3.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    row3.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    row3.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    row3.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    row3.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);

                    int currentRow = 4;


                    if (groupedData?.DepartmentGroups == null || !groupedData.DepartmentGroups.Any())
                    {
                        var noDataCell = worksheet.Cells[3, 1, 3, 15];
                        noDataCell.Merge = true;
                        noDataCell.Value = "No Data Available";
                        noDataCell.Style.Font.Bold = true;
                        noDataCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    else
                    {
                        string[] headers = new string[]
                        {
                    "Employee ID", "Name", "Designation", "Branch", "Employment Nature",
                    "Employee Type", "Joining Date", "Termination Date", "Service Length",
                    "Shift", "Immediate Supervisor", "Head of Department", "Official Phone",
                    "Official Email", "Activity Status"
                        };

                        foreach (var deptGroup in groupedData.DepartmentGroups)
                        {
                            // Department Header
                            var deptHeaderCell = worksheet.Cells[currentRow, 1, currentRow, 15];
                            deptHeaderCell.Merge = true;
                            deptHeaderCell.Value = "Department: " + deptGroup.DepartmentName;
                            deptHeaderCell.Style.Font.Bold = true;
                            deptHeaderCell.Style.Font.Size = 15;
                            deptHeaderCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            deptHeaderCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            deptHeaderCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                            deptHeaderCell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            currentRow++;

                            // Column Headers
                            for (int i = 0; i < headers.Length; i++)
                            {
                                var headerCell = worksheet.Cells[currentRow, i + 1];
                                headerCell.Value = headers[i];
                                headerCell.Style.Font.Bold = true;
                                headerCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                headerCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                headerCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                headerCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                                headerCell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            }
                            currentRow++;

                            // Employee Data
                            foreach (var emp in deptGroup.Employees)
                            {
                                worksheet.Cells[currentRow, 1].Value = emp.EmployeeID ?? "";
                                worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                worksheet.Cells[currentRow, 2].Value = emp.EmpName ?? "";
                                worksheet.Cells[currentRow, 3].Value = emp.DesignationName ?? "";
                                worksheet.Cells[currentRow, 4].Value = emp.BranchName ?? "";
                                worksheet.Cells[currentRow, 5].Value = emp.EmploymentNature ?? "";
                                worksheet.Cells[currentRow, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                worksheet.Cells[currentRow, 6].Value = emp.EmpTypeName ?? "";
                                worksheet.Cells[currentRow, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                worksheet.Cells[currentRow, 7].Value = emp.JoiningDate ?? "";
                                worksheet.Cells[currentRow, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                worksheet.Cells[currentRow, 8].Value = emp.SeparationDate ?? "";
                                worksheet.Cells[currentRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                worksheet.Cells[currentRow, 9].Value = emp.ServiceLength ?? "";
                                worksheet.Cells[currentRow, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                worksheet.Cells[currentRow, 10].Value = emp.ShiftName ?? "";
                                worksheet.Cells[currentRow, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                worksheet.Cells[currentRow, 11].Value = emp.ImmediateSupervisorName ?? "";
                                worksheet.Cells[currentRow, 12].Value = emp.HeadOfDepartmentName ?? "";

                                worksheet.Cells[currentRow, 13].Value = emp.MobileNo ?? "";
                                worksheet.Cells[currentRow, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                worksheet.Cells[currentRow, 14].Value = emp.Email ?? "";
                                worksheet.Cells[currentRow, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                worksheet.Cells[currentRow, 15].Value = emp.EmployeeStatus ?? "";
                                worksheet.Cells[currentRow, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                // Add borders
                                for (int col = 1; col <= 15; col++)
                                {
                                    worksheet.Cells[currentRow, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                }

                                currentRow++;
                            }

                            // Department Total
                            var deptTotalCell = worksheet.Cells[currentRow, 1, currentRow, 15];
                            deptTotalCell.Merge = true;
                            deptTotalCell.Value = "Total Employees in " + deptGroup.DepartmentName + ": " + deptGroup.TotalCount;
                            deptTotalCell.Style.Font.Bold = true;
                            deptTotalCell.Style.Font.Size = 12;
                            deptTotalCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            deptTotalCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            deptTotalCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                            deptTotalCell.Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            currentRow += 2;
                        }

                        // Grand Total
                        var grandTotalCell = worksheet.Cells[currentRow, 1, currentRow, 15];
                        grandTotalCell.Merge = true;
                        grandTotalCell.Value = "Grand Total Employees: " + groupedData.GrandTotal;
                        grandTotalCell.Style.Font.Bold = true;
                        grandTotalCell.Style.Font.Size = 12;
                        grandTotalCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        grandTotalCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        grandTotalCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                        grandTotalCell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }

                    byte[] fileBytes = package.GetAsByteArray();
                    return File(fileBytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Employee_Official_Info_Report.xlsx");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
                return StatusCode(500, new { message = "Error generating report", error = ex.Message });
            }
        }

        #endregion


        #region Pdf
        [HttpPost]
        public async Task<IActionResult> ExportEmployeeOfficialInfoToPdf([FromBody] OfficialInfoReportFilterVm filter)
        {
            try
            {
                // Get employee data using service
                var employees = await employeeOfficialInfoReportService.GetEmployeeOfficialInfoReport(filter);
                var employeeList = employees.DepartmentGroups;

                if (employeeList == null || !employeeList.Any())
                {
                    return Json(new { isSuccess = false, message = "No employees found." });
                }

                // Fetch company information
                var companyInfo = await GetCompanyInfoAsync();

                // Generate JSON data for jsPDF
                var pdfData = new
                {
                    isSuccess = true,
                    companyName = companyInfo.CompanyName,
                    companyAddress = companyInfo.Address,
                    departmentGroups = employeeList.Select(dept => new
                    {
                        departmentName = dept.DepartmentName,
                        employees = dept.Employees.Select(emp => new
                        {
                            employeeID = emp.EmployeeID ?? "",
                            empName = emp.EmpName ?? "",
                            designationName = emp.DesignationName ?? "",
                            branchName = emp.BranchName ?? "",
                            employmentNature = emp.EmploymentNature ?? "",
                            empTypeName = emp.EmpTypeName ?? "",
                            joiningDate = emp.JoiningDate ?? "",
                            separationDate = emp.SeparationDate ?? "",
                            serviceLength = emp.ServiceLength ?? "",
                            shiftName = emp.ShiftName ?? "",
                            immediateSupervisorName = emp.ImmediateSupervisorName ?? "",
                            headOfDepartmentName = emp.HeadOfDepartmentName ?? "",
                            mobileNo = emp.MobileNo ?? "",
                            email = emp.Email ?? "",
                            employeeStatus = emp.EmployeeStatus ?? ""
                        }).ToList()
                    }).ToList()
                };

                return Json(pdfData);
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = $"Error: {ex.Message}" });
            }
        }

        private async Task<(string CompanyName, string Address)> GetCompanyInfoAsync()
        {
            return await coreCompany.All()
                .AsNoTracking()
                .Where(c => c.CompanyCode == LoginInfo.CompanyCode)
                .Select(c => new ValueTuple<string, string>(c.CompanyName, c.Address1))
                .FirstOrDefaultAsync();
        }
        #endregion

    }
}
