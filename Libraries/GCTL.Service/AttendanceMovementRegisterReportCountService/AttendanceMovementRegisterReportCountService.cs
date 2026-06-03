using Dapper;
using DocumentFormat.OpenXml.Bibliography;
using GCTL.Core.Data;
using GCTL.Core.ViewModels.Accounts;
using GCTL.Core.ViewModels.AttendanceMovementRegisterReportCount;
using GCTL.Core.ViewModels.Companies;
using GCTL.Data.Models;
using GCTL.Service.AttendanceMovementRegisterReportService;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NPOI.SS.Formula.Functions;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.Data;


namespace GCTL.Service.AttendanceMovementRegisterReportCountService
{
    public class AttendanceMovementRegisterReportCountService : AppService<HrmAtdMachineData>, IAttendanceMovementRegisterReportCountService
    {
        private readonly IRepository<HrmAtdMachineData> atdMachingDataRepo;
        private readonly IConfiguration configuration;
        private readonly IRepository<CoreCompany> coreCompany;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;

        public AttendanceMovementRegisterReportCountService(
            IRepository<HrmAtdMachineData> atdMachingDataRepo,
            IConfiguration configuration,
             IRepository<CoreCompany> coreCompany,
             IRepository<CoreAccessCode> accessCodeRepository
            ) : base(atdMachingDataRepo)
        {
            this.atdMachingDataRepo = atdMachingDataRepo;
            this.configuration = configuration;
            this.coreCompany = coreCompany;
            this.accessCodeRepository = accessCodeRepository;
        }

        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Attendance Movement Register Count" && x.TitleCheck);
        }


        public async Task<List<DepartmentAndDateGroupedData>> GetAttendanceMachineDataAsync(
    AttendanceMovementRegisterReportCountFilterData filter,
    string BaseUrl,
    UserInfoViewModel loginInfo)
        {
            try
            {
                using var connection = new SqlConnection(configuration.GetConnectionString("ApplicationDbConnection"));
                var parameters = new DynamicParameters();
                parameters.Add("@AccessCode", filter.AccessCode);
                parameters.Add("@EmployeeId", filter.EmployeeId);
                parameters.Add("@CompanyCodes", filter.CompanyCodes != null && filter.CompanyCodes.Any()
                    ? string.Join(",", filter.CompanyCodes) : null);

                parameters.Add("@BranchCodes", filter.BranchCodes != null && filter.BranchCodes.Any()
                    ? string.Join(",", filter.BranchCodes) : null);

                parameters.Add("@DepartmentCodes", filter.DepartmentCodes != null && filter.DepartmentCodes.Any()
                    ? string.Join(",", filter.DepartmentCodes) : null);

                parameters.Add("@DesignationCodes", filter.DesignationCodes != null && filter.DesignationCodes.Any()
                    ? string.Join(",", filter.DesignationCodes) : null);

                parameters.Add("@EmployeeIDs", filter.EmployeeIDs != null && filter.EmployeeIDs.Any()
                    ? string.Join(",", filter.EmployeeIDs) : null);

                parameters.Add("@FromDate", filter.FromDate);
                parameters.Add("@ToDate", filter.ToDate);

                parameters.Add("@Months", filter.MonthIDs != null && filter.MonthIDs.Any()
                    ? string.Join(",", filter.MonthIDs) : null);

                parameters.Add("@Years", filter.YearIDs != null && filter.YearIDs.Any()
                    ? string.Join(",", filter.YearIDs) : null);

                var result = await connection.QueryAsync<AttendanceMovementRegisterReportCountDto>(
                    "SP_GetAttendanceMachineDataCount",
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                var comDetails = await GetCompanyInfoAsync(loginInfo.CompanyCode);
                var baseUrl = $"{BaseUrl}/AttendanceMovementRegisterReportCount/ViewDetails";

                var groupedData = result
                    .GroupBy(r => new { r.DepartmentCode, r.DepartmentName })
                    .Select(g => new DepartmentAndDateGroupedData
                    {
                        DepartmentCode = g.Key.DepartmentCode,
                        DepartmentName = g.Key.DepartmentName,
                        CompanyName = comDetails.CompanyName,
                        CompanyAddress = comDetails.Address,
                       
                        TotalEmployees = g.Select(x => x.EmployeeID).Distinct().Count(),
                        Employees = g.Select((emp, idx) =>
                        {
                            emp.Sn = idx + 1;
                            var dateStr = emp.Date.ToString("dd-MM-yyyy");
                            emp.ViewLink = $"{baseUrl}?employeeId={emp.EmployeeID}&date={dateStr}";
                          
                            return emp;
                        }).ToList()
                    }).ToList();

                return groupedData;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving attendance machine data: {ex.Message}", ex);
            }
        }

        private async Task<(string CompanyName, string Address)> GetCompanyInfoAsync(string CompanyCode)
        {
            return await coreCompany.All()
                .AsNoTracking()
                .Where(c => c.CompanyCode == CompanyCode)
                .Select(c => new ValueTuple<string, string>(c.CompanyName, c.Address1))
                .FirstOrDefaultAsync();
        }



        private static AttendanceMovementRegisterReportCountDropdownListDto _cachedData;

        public async Task<AttendanceMovementRegisterReportCountDropdownListDto> GetAttendanceMachineDataFiltersAsync(
            AttendanceMovementRegisterReportCountFilterData filter)
        {
            try
            {
                if (_cachedData != null)
                {
                    return ApplyFilters(_cachedData, filter);
                }

                using var conn = new SqlConnection(configuration.GetConnectionString("ApplicationDbConnection"));
                await conn.OpenAsync();

                string ToCsv(List<string> list) =>
                    (list != null && list.Any()) ? string.Join(",", list) : null;

                var param = new
                {
                    CompanyCodes = ToCsv(filter.CompanyCodes),
                    BranchCodes = ToCsv(filter.BranchCodes),
                    DepartmentCodes = ToCsv(filter.DepartmentCodes),
                    DesignationCodes = ToCsv(filter.DesignationCodes),
                    EmployeeIDs = ToCsv(filter.EmployeeIDs)
                };

                using var multi = await conn.QueryMultipleAsync(
                    "SP_GetAttendanceMachineDataFilters",
                    param,
                    commandType: CommandType.StoredProcedure);

                var result = new AttendanceMovementRegisterReportCountDropdownListDto
                {
                    Companies = (await multi.ReadAsync<IdNamePair>()).DistinctBy(x => x.Id).ToList(),
                    Branches = (await multi.ReadAsync<IdNamePair>()).DistinctBy(x => x.Id).ToList(),
                    Departments = (await multi.ReadAsync<IdNamePair>()).DistinctBy(x => x.Id).ToList(),
                    Designations = (await multi.ReadAsync<IdNamePair>()).DistinctBy(x => x.Id).ToList(),
                    Employees = (await multi.ReadAsync<IdNamePair>()).DistinctBy(x => x.Id).ToList(),
                };

                _cachedData = result;

                return ApplyFilters(result, filter);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving filter data: {ex.Message}", ex);
            }
        }

        private AttendanceMovementRegisterReportCountDropdownListDto ApplyFilters(
            AttendanceMovementRegisterReportCountDropdownListDto source,
            AttendanceMovementRegisterReportCountFilterData filter)
        {
            return new AttendanceMovementRegisterReportCountDropdownListDto
            {
                Companies = FilterList(source.Companies, filter.CompanyCodes),
                Branches = FilterList(source.Branches, filter.BranchCodes),
                Departments = FilterList(source.Departments, filter.DepartmentCodes),
                Designations = FilterList(source.Designations, filter.DesignationCodes),
                Employees = FilterList(source.Employees, filter.EmployeeIDs)
            };
        }

        private List<IdNamePair> FilterList(List<IdNamePair> list, List<string> filterIds)
        {
            if (filterIds == null || !filterIds.Any())
                return list;

            return list.Where(x => filterIds.Contains(x.Id.ToString())).ToList();
        }


        public async Task<AttendanceMovementCountDetailDto> GetEmployeeMovementDetailsAsync(EmployeeMovementRequestDto requestDto)
        {
            try
            {
                using var connection = new SqlConnection(configuration.GetConnectionString("ApplicationDbConnection"));
                var parameters = new DynamicParameters();

                parameters.Add("@EmployeeId", requestDto.EmployeeId);
                parameters.Add("@Date", requestDto.Date); 

                var result = await connection.QueryAsync<AttendanceMovementCountDetailDto>(
                    "SP_GetAttendanceMachineDataCountDetails",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                var rows = result.ToList();
                if (!rows.Any()) return null;

                var first = rows.First();

                var details = new AttendanceMovementCountDetailDto
                {
                    CompanyName = first.CompanyName,
                    Address1 = first.Address1,
                    EmployeeID = first.EmployeeID,
                    FullName = first.FullName,
                    Date = first.Date,
                    BranchName = first.BranchName,
                    DepartmentName = first.DepartmentName,
                    DesignationName = first.DesignationName,
                    Movements = rows.Select(x => new AttendanceMovementCountRowDto
                    {
                        Time = DateTime.Today.Add(x.Time),
                        MachineId = x.MachineId,
                        //LocationUrl = $"https://www.google.com/maps/dir/?api=1&destination=${x.Latitude},${x.Longitude}&travelmode=driving"
                        LocationUrl = (!string.IsNullOrWhiteSpace(x.Latitude) && !string.IsNullOrWhiteSpace(x.Longitude))
    ? $"https://www.google.com/maps/dir/?api=1&destination={x.Latitude},{x.Longitude}&travelmode=driving"
    : string.Empty
                    }).ToList()
                };

                return details;
            }
            catch (Exception)
            {

                throw;
            }

        }



        public async Task<byte[]> GenerateEmployeeMovementPdf(AttendanceMovementCountDetailDto details)
        {
            try
            {
                if (details == null) return null;

                var pdfBytes = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(30);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        // Header
                        page.Header()
                            .Column(header =>
                            {
                                header.Item().Text(details.CompanyName).FontSize(14).Bold().AlignCenter();
                                header.Item().Text(details.Address1).FontSize(10).AlignCenter();
                                header.Item().Text("Attendance Movement Register Details Report").FontSize(12).Bold().AlignCenter();
                                header.Item().Text($"{details.Date:dd/MM/yyyy}").FontSize(8).AlignCenter();
                                header.Item().PaddingTop(5);
                                header.Item().Text($"Employee Id: {details.EmployeeID}");
                                header.Item().Text($"Name: {details.FullName}");
                                header.Item().Text($"Designation: {details.DesignationName}");
                                header.Item().Text($"Department: {details.DepartmentName}");
                                header.Item().Text($"Branch: {details.BranchName}");
                            });

                        // Content Table
                        page.Content().PaddingTop(20)
                            .Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                // Header row with styling (centered)
                                table.Header(headerRow =>
                                {
                                    headerRow.Cell().Element(CellHeaderStyle).AlignCenter().Text("Time").Bold();
                                    headerRow.Cell().Element(CellHeaderStyle).AlignCenter().Text("Machine Id").Bold();
                                    headerRow.Cell().Element(CellHeaderStyle).AlignCenter().Text("Location").Bold();
                                });

                                // Data rows
                                foreach (var m in details.Movements)
                                {
                                    table.Cell().Element(c => CellStyle(c)).Text(m.Time.ToString("hh:mm:ss tt")).AlignCenter();
                                    table.Cell().Element(CellStyle).AlignCenter().Text(m.MachineId);                                    
                                    if (!string.IsNullOrWhiteSpace(m.LocationUrl))
                                    {
                                        table.Cell().Element(CellStyle).AlignCenter().Hyperlink(m.LocationUrl).Text(text =>
                                        {
                                            text.Span("View Location")
                                                .FontColor(Colors.Blue.Medium)
                                                .Underline();
                                        });
                                    }
                                    else
                                    {
                                        table.Cell().Element(CellStyle).AlignCenter().Text("");
                                    }


                                }
                            });

                        // Footer
                        page.Footer()
                            .AlignCenter()
                            .Text(x => x.CurrentPageNumber());
                    });
                }).GeneratePdf();

                return pdfBytes;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Helper methods for styling
        static IContainer CellStyle(IContainer container)
        {
            return container
                .Border(1)
                .BorderColor(Colors.Grey.Darken2)
                .Padding(5)
                .AlignMiddle(); 
        }

        static IContainer CellHeaderStyle(IContainer container)
        {
            return container
                .Border(1)
                .BorderColor(Colors.Black)
                .Padding(5)
                .AlignMiddle();  
        }


        /// <summary>
        /// Full method to get PDF for employee movement by Id & Date
        /// </summary>
        public async Task<byte[]> GetEmployeeMovementPdfAsync(EmployeeMovementRequestDto requestDto)
        {
            // 1. Get details
            var details = await GetEmployeeMovementDetailsAsync(requestDto);

            if (details == null)
                return null;

            // 2. Generate PDF
            var pdfBytes = await GenerateEmployeeMovementPdf(details);

            return pdfBytes;
        }



    }
}
