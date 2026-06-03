using Dapper;
using GCTL.Core.Data;
using GCTL.Core.ViewModels.AttendanceMovementRegisterReportDto;
using GCTL.Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace GCTL.Service.AttendanceMovementRegisterReportService
{
    public class AttendanceMovementRegisterReportService : AppService<HrmAtdMachineData>, IAttendanceMovementRegisterReportService
    {
        private readonly IRepository<HrmAtdMachineData> atdMachingDataRepo;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly IConfiguration configuration;

        public AttendanceMovementRegisterReportService(
            IRepository<HrmAtdMachineData> atdMachingDataRepo,
            IRepository<CoreAccessCode> accessCodeRepository,
            IConfiguration configuration) : base(atdMachingDataRepo)
        {
            this.atdMachingDataRepo = atdMachingDataRepo;
            this.accessCodeRepository = accessCodeRepository;
            this.configuration = configuration;
        }

        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Attendance Movement Register" && x.TitleCheck);
        }

        public async Task<List<DepartmentGroupedData>> GetAttendanceMachineDataAsync(AttendanceMovementRegisterReportFilterData filter)
        {
            try
            {
                using var connection = new SqlConnection(configuration.GetConnectionString("ApplicationDbConnection"));

                var parameters = new DynamicParameters();

                // ─── Auth ───────────────────────────────────────────────
                parameters.Add("@AccessCode", filter.AccessCode);
                parameters.Add("@EmployeeId", filter.EmployeeId);

                // ─── Filters ────────────────────────────────────────────
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

                var result = await connection.QueryAsync<AttendanceMovementRegisterReportDto>(
                    "SP_GetAttendanceMachineData",
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                // ─── Group by Department ─────────────────────────────────
                var groupedData = result
                    .GroupBy(r => new { r.DepartmentCode, r.DepartmentName })
                    .Select(g => new DepartmentGroupedData
                    {
                        DepartmentCode = g.Key.DepartmentCode,
                        DepartmentName = g.Key.DepartmentName,
                        TotalEmployees = g.Count(),
                        Employees = g.ToList()
                    })
                    .ToList();

                return groupedData;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving attendance machine data: {ex.Message}", ex);
            }
        }

        public async Task<AttendanceMovementRegisterReportDropdownListDto> GetAttendanceMachineDataFiltersAsync(AttendanceMovementRegisterReportFilterData filter)
        {
            try
            {
                using var conn = new SqlConnection(configuration.GetConnectionString("ApplicationDbConnection"));
                await conn.OpenAsync();

                // Convert list to comma separated string or null
                string ToCsv(List<string> list) =>
                    (list != null && list.Any()) ? string.Join(",", list) : null;

                string ToCsvInt(List<int> list) =>
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

                var result = new AttendanceMovementRegisterReportDropdownListDto
                {
                    // Machine data first (optional) — if needed
                    //MainData = (await multi.ReadAsync<MachineDataDto>()).ToList(),

                    Companies = (await multi.ReadAsync<IdNamePair>()).DistinctBy(x => x.Id).ToList(),
                    Branches = (await multi.ReadAsync<IdNamePair>()).DistinctBy(x => x.Id).ToList(),
                    Departments = (await multi.ReadAsync<IdNamePair>()).DistinctBy(x => x.Id).ToList(),
                    Designations = (await multi.ReadAsync<IdNamePair>()).DistinctBy(x => x.Id).ToList(),
                    Employees = (await multi.ReadAsync<IdNamePair>()).DistinctBy(x => x.Id).ToList(),                    
                };

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving filter data: {ex.Message}", ex);
            }
        }
    }
}