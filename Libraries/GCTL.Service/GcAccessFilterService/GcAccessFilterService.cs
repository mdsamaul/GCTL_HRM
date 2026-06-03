using Dapper;
using GCTL.Core.ViewModels.EachGcFilterRequest;
using GCTL.Core.ViewModels.GcAccessFilterRequest;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.GcAccessFilterService
{
    public class GcAccessFilterService : IGcAccessFilterService
    {

        private readonly string _connectionString;

        public GcAccessFilterService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ApplicationDbConnection");
        }

        // ─── Helpers ───────────────────────────────────────────────
        private static string? ToCsv(List<string>? list)
            => (list == null || list.Count == 0) ? null : string.Join(",", list);

        private async Task<PagedAccessResultDto<GcAccessItemDto>> ExecAsync(string sp, object param)
        {
            using var con = new SqlConnection(_connectionString);
            var rows = (await con.QueryAsync<GcSpRow>(sp, param, commandType: CommandType.StoredProcedure)).ToList();
            return new PagedAccessResultDto<GcAccessItemDto>
            {
                Items = rows.Select(x => new GcAccessItemDto { Code = x.Code, Name = x.Name }).ToList(),
                More = rows.FirstOrDefault()?.More ?? false
            };
        }

        private class GcSpRow
        {
            public string? Code { get; set; }
            public string? Name { get; set; }
            public bool More { get; set; }
        }

        // ─── Methods ───────────────────────────────────────────────
        public Task<PagedAccessResultDto<GcAccessItemDto>> GetCompanyListByAccessAsync(GcAccessFilterRequestDto req)
            => ExecAsync("dbo.sp_gc_company_list_by_access", new
            {
                req.AccessCode,
                req.EmployeeId,
                req.Search,
                req.Page,
                req.PageSize
            });

        public Task<PagedAccessResultDto<GcAccessItemDto>> GetBranchListByAccessAsync(GcAccessFilterRequestDto req)
            => ExecAsync("dbo.sp_gc_branch_list_by_access", new
            {
                req.AccessCode,
                req.EmployeeId,
                CompanyCodes = ToCsv(req.CompanyCodes),
                req.Search,
                req.Page,
                req.PageSize
            });

        public Task<PagedAccessResultDto<GcAccessItemDto>> GetDivisionListByAccessAsync(GcAccessFilterRequestDto req)
            => ExecAsync("dbo.sp_gc_division_list_by_access", new
            {
                req.AccessCode,
                req.EmployeeId,
                CompanyCodes = ToCsv(req.CompanyCodes),
                BranchCodes = ToCsv(req.BranchCodes),
                req.Search,
                req.Page,
                req.PageSize
            });

        public Task<PagedAccessResultDto<GcAccessItemDto>> GetDepartmentListByAccessAsync(GcAccessFilterRequestDto req)
            => ExecAsync("dbo.sp_gc_department_list_by_access", new
            {
                req.AccessCode,
                req.EmployeeId,
                CompanyCodes = ToCsv(req.CompanyCodes),
                BranchCodes = ToCsv(req.BranchCodes),
                DivisionCodes = ToCsv(req.DivisionCodes),
                req.Search,
                req.Page,
                req.PageSize
            });

        public Task<PagedAccessResultDto<GcAccessItemDto>> GetDesignationListByAccessAsync(GcAccessFilterRequestDto req)
            => ExecAsync("dbo.sp_gc_designation_list_by_access", new
            {
                req.AccessCode,
                req.EmployeeId,
                CompanyCodes = ToCsv(req.CompanyCodes),
                BranchCodes = ToCsv(req.BranchCodes),
                DivisionCodes = ToCsv(req.DivisionCodes),
                DepartmentCodes = ToCsv(req.DepartmentCodes),
                req.Search,
                req.Page,
                req.PageSize
            });

        public Task<PagedAccessResultDto<GcAccessItemDto>> GetEmployeeListByAccessAsync(GcAccessFilterRequestDto req)
            => ExecAsync("dbo.sp_gc_employee_list_by_access", new
            {
                req.AccessCode,
                req.EmployeeId,
                CompanyCodes = ToCsv(req.CompanyCodes),
                BranchCodes = ToCsv(req.BranchCodes),
                DivisionCodes = ToCsv(req.DivisionCodes),
                DepartmentCodes = ToCsv(req.DepartmentCodes),
                DesignationCodes = ToCsv(req.DesignationCodes),
                EmployeeStatuses = ToCsv(req.EmployeeStatuses),
                EmployeeNatureCodes = ToCsv(req.EmployeeNatureCodes),
                EmployeeTypes = ToCsv(req.EmployeeTypes),
                JoiningDateFrom = req.JoiningDateFrom,
                JoiningDateTO = req.JoiningDateTo,
                req.Search,
                req.Page,
                req.PageSize
            });
    }
}
