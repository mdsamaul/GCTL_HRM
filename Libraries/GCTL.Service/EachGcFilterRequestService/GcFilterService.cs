using Dapper;
using GCTL.Core.ViewModels.EachGcFilterRequest;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.EachGcFilterRequestService
{
    public class GcFilterService : IGcFilterService
    {
        private readonly string _configuration;
        public GcFilterService(IConfiguration configuration)
        {
            _configuration = configuration.GetConnectionString("ApplicationDbConnection");
        }

        private static string? ToCsv(List<string>? list)
            => (list == null || list.Count == 0) ? null : string.Join(",", list);

        private async Task<PagedResultDto<GcItemDto>> ExecAsync(string sp, object param)
        {
            using var con = new SqlConnection(_configuration);
            var rows = (await con.QueryAsync<GcSpRow>(sp, param, commandType: CommandType.StoredProcedure)).ToList();

            return new PagedResultDto<GcItemDto>
            {
                Items = rows.Select(x => new GcItemDto { Code = x.Code, Name = x.Name }).ToList(),
                More = rows.FirstOrDefault()?.More ?? false
            };
        }

        private class GcSpRow
        {
            public string? Code { get; set; }
            public string? Name { get; set; }
            public bool More { get; set; }
        }

        public Task<PagedResultDto<GcItemDto>> GetCompaniesAsync(GcFilterRequestDto req)
            => ExecAsync("dbo.sp_gc_company_list", new { req.Search, req.Page, req.PageSize });

        public Task<PagedResultDto<GcItemDto>> GetBranchesAsync(GcFilterRequestDto req)
            => ExecAsync("dbo.sp_gc_branch_list", new
            {
                CompanyCodes = ToCsv(req.CompanyCodes),
                req.Search,
                req.Page,
                req.PageSize
            });

        public Task<PagedResultDto<GcItemDto>> GetDivisionsAsync(GcFilterRequestDto req)
            => ExecAsync("dbo.sp_gc_division_list", new
            {
                CompanyCodes = ToCsv(req.CompanyCodes),
                BranchCodes = ToCsv(req.BranchCodes),
                req.Search,
                req.Page,
                req.PageSize
            });

        public Task<PagedResultDto<GcItemDto>> GetDepartmentsAsync(GcFilterRequestDto req)
            => ExecAsync("dbo.sp_gc_department_list", new
            {
                CompanyCodes = ToCsv(req.CompanyCodes),
                BranchCodes = ToCsv(req.BranchCodes),
                DivisionCodes = ToCsv(req.DivisionCodes),
                req.Search,
                req.Page,
                req.PageSize
            });

        public Task<PagedResultDto<GcItemDto>> GetDesignationsAsync(GcFilterRequestDto req)
            => ExecAsync("dbo.sp_gc_designation_list", new
            {
                CompanyCodes = ToCsv(req.CompanyCodes),
                BranchCodes = ToCsv(req.BranchCodes),
                DivisionCodes = ToCsv(req.DivisionCodes),
                DepartmentCodes = ToCsv(req.DepartmentCodes),
                req.Search,
                req.Page,
                req.PageSize
            });

        public Task<PagedResultDto<GcItemDto>> GetEmployeesAsync(GcFilterRequestDto req)
            => ExecAsync("dbo.sp_gc_employee_list", new
            {
                CompanyCodes = ToCsv(req.CompanyCodes),
                BranchCodes = ToCsv(req.BranchCodes),
                DivisionCodes = ToCsv(req.DivisionCodes),
                DepartmentCodes = ToCsv(req.DepartmentCodes),
                DesignationCodes = ToCsv(req.DesignationCodes),
                EmployeeStatuses = ToCsv(req.EmployeeStatuses),
                req.Search,
                req.Page,
                req.PageSize
            });
    }
}
