using AutoMapper;
using Dapper;
using GCTL.Core.Data;
using GCTL.Core.ViewModels.EmployeeOfficialInfoReport;
using GCTL.Data.Models;
using GCTL.Service.Common;
using GCTL.Service.HrmEmployees2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace GCTL.Service.EmployeeOfficialInfoReport
{
    public class EmployeeOfficialInfoReportService : IEmployeeOfficialInfoReportService
    {
        #region Repository & Services
        private readonly IRepository<HrmEmployee> _hrmEmployeeRepository;
        private readonly IRepository<CoreBranch> coreBranch;
        private readonly IRepository<CoreCompany> coreCompany;
        private readonly IRepository<HrmDefDepartment> deptment;
        private readonly IRepository<HrmDefDesignation> designation;
        private readonly IRepository<HrmAtdShift> shift;
        private readonly IRepository<HrmEmployeeOfficialInfo> _empOfficialRepository;
        private readonly IRepository<HrmLeaveApplicationEntry> leaveEntry;
        private readonly IRepository<HrmSeparation> separation;
        private readonly IRepository<HrmDefEmpType> empType;
        private readonly IRepository<HrmEmployeeAdditionalInfo> empAddInfo;
        private readonly IRepository<HrmEisDefEmploymentNature> empNature;
        private readonly IRepository<HrmDefEmployeeStatus> _employeeStatusRepository;
        private readonly IHrmEmployee2Service hrmEmployee2Service;
        private readonly IMapper mapper;
        private readonly IConfiguration _configuration;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;
        private readonly IMemoryCache _cache;

        public EmployeeOfficialInfoReportService(
            IRepository<HrmEmployee> hrmEmployeeRepository,
            IConfiguration configuration,
            IRepository<CoreBranch> coreBranch, 
            IRepository<CoreCompany> coreCompany,
            IRepository<HrmDefDepartment> deptment,
            IRepository<HrmDefDesignation> designation,
            IRepository<HrmAtdShift> shift,
            IRepository<HrmEmployeeOfficialInfo> empOfficialRepository,
            IRepository<HrmLeaveApplicationEntry> leaveEntry,
            IRepository<HrmSeparation> separation, 
            IRepository<HrmDefEmpType> empType, 
            IRepository<HrmEmployeeAdditionalInfo> empAddInfo,
            IRepository<HrmEisDefEmploymentNature> empNature,
            IRepository<HrmDefEmployeeStatus> employeeStatusRepository,
            IHrmEmployee2Service hrmEmployee2Service, IMapper mapper,
            IRepository<CoreAccessCode> accessCodeRepository,
            ICommonService commonService,
            IMemoryCache cache
            )
        {
            _hrmEmployeeRepository = hrmEmployeeRepository;
            this.coreBranch = coreBranch;
            this.coreCompany = coreCompany;
            this.deptment = deptment;
            this.designation = designation;
            this.shift = shift;
            _empOfficialRepository = empOfficialRepository;
            this.leaveEntry = leaveEntry;
            this.separation = separation;
            this.empType = empType;
            this.empAddInfo = empAddInfo;
            this.empNature = empNature;
            _employeeStatusRepository = employeeStatusRepository;
            this.hrmEmployee2Service = hrmEmployee2Service;
            this.mapper = mapper;
            _configuration = configuration;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
            this._cache = cache;
        }

        #endregion

        #region GetOfficialInfoDropdownAsync 
    

        public async Task<OfficialInfoDropdownResponse> GetOfficialInfoDropdownAsync(OfficialInfoFilterVm filters)
        {
            try
            {
                var cacheKey = $"OfficialInfoDropdown_{filters?.GetHashCode()}";

                if (_cache.TryGetValue(cacheKey, out OfficialInfoDropdownResponse cachedResponse))
                {
                    return cachedResponse;
                }

                var vm = new OfficialInfoDropdownResponse();

                await using var connection = new SqlConnection(_configuration.GetConnectionString("ApplicationDbConnection"));

                var p = new DynamicParameters();
                p.Add("@CompanyCodes", filters?.CompanyCodes?.Any() == true ? string.Join(",", filters.CompanyCodes) : null);
                p.Add("@BranchCodes", filters?.BranchCodes?.Any() == true ? string.Join(",", filters.BranchCodes) : null);
                p.Add("@DepartmentCodes", filters?.DepartmentCodes?.Any() == true ? string.Join(",", filters.DepartmentCodes) : null);
                p.Add("@DesignationCodes", filters?.DesignationCodes?.Any() == true ? string.Join(",", filters.DesignationCodes) : null);
                p.Add("@EmployeeCodes", filters?.EmployeeCodes?.Any() == true ? string.Join(",", filters.EmployeeCodes) : null);
                p.Add("@EmployeeTypeCode", string.IsNullOrWhiteSpace(filters?.EmployeeTypeCode) ? null : filters.EmployeeTypeCode);
                p.Add("@EmploymentNatureId", string.IsNullOrWhiteSpace(filters?.EmploymentNatureId) ? null : filters.EmploymentNatureId);
                p.Add("@NationalId", string.IsNullOrWhiteSpace(filters?.NationalId) ? null : filters.NationalId);
                p.Add("@TinNo", string.IsNullOrWhiteSpace(filters?.TinNo) ? null : filters.TinNo);
                p.Add("@PassportNo", string.IsNullOrWhiteSpace(filters?.PassportNo) ? null : filters.PassportNo);
                p.Add("@DrivingLicense", string.IsNullOrWhiteSpace(filters?.DrivingLicense) ? null : filters.DrivingLicense);
                p.Add("@ImmediateSup", string.IsNullOrWhiteSpace(filters?.ImmediateSup) ? null : filters.ImmediateSup);
                p.Add("@HOD", string.IsNullOrWhiteSpace(filters?.HOD) ? null : filters.HOD);
                p.Add("@ShiftCode", string.IsNullOrWhiteSpace(filters?.ShiftCode) ? null : filters.ShiftCode);
                p.Add("@EmployeeStatus", string.IsNullOrWhiteSpace(filters?.EmployeeStatus) ? null : filters.EmployeeStatus);
                p.Add("@IsExpatriate", string.IsNullOrWhiteSpace(filters?.IsExpatriate) ? null : filters.IsExpatriate);

                var result = await connection.QueryAsync<OfficialInfoDto>(
                    "GetOfficialInfoDropdown", p, commandType: CommandType.StoredProcedure);

                foreach (var item in result)
                {
                    AddToListIfNotExists(vm.Companies, item.CompanyId, item.CompanyValue);
                    AddToListIfNotExists(vm.Employees, item.EmployeeId, item.EmployeeValue);
                    AddToListIfNotExists(vm.Designations, item.DesignationId, item.DesignationValue);
                    AddToListIfNotExists(vm.Departments, item.DepartmentId, item.DepartmentValue);
                    AddToListIfNotExists(vm.Branches, item.BranchId, item.BranchValue);
                    AddToListIfNotExists(vm.EmploymentNatures, item.EmploymentNatureId, item.EmploymentNatureValue);
                    AddToListIfNotExists(vm.EmployeeTypes, item.EmpTypeId, item.EmpTypeValue);
                    AddToListIfNotExists(vm.Shifts, item.ShiftId, item.ShiftValue);
                    AddToListIfNotExists(vm.ImmediateSupervisors, item.ImmediateSupervisorId, item.ImmediateSupervisorValue);
                    AddToListIfNotExists(vm.HODs, item.HODId, item.HODValue);
                    AddToListIfNotExists(vm.ActivityStatuses, item.EmployeeStatusId, item.EmployeeStatusValue);
                    AddToListIfNotExists(vm.NationalIds, item.NationalId, item.NationalValue);
                    AddToListIfNotExists(vm.TinNumbers, item.TinId, item.TinValue);
                    AddToListIfNotExists(vm.Passports, item.PassportId, item.PassportValue);
                    AddToListIfNotExists(vm.DrivingLicenses, item.LicenseId, item.LicenseValue);
                }

                _cache.Set(cacheKey, vm, TimeSpan.FromMinutes(10));

                return vm;
            }
            catch (Exception)
            {
                throw;
            }
        }


        private void AddToListIfNotExists(List<DropdownDto> list, string id, string value)
        {
            if (!string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(value))
            {
                if (!list.Any(x => x.Id == id))
                {
                    list.Add(new DropdownDto { Id = id, Value = value });
                }
            }
        }

        #endregion

        #region GetEmployeeOfficialInfoReport

        public async Task<EmployeeReportGroupedDto> GetEmployeeOfficialInfoReport(OfficialInfoReportFilterVm ModelData)
        {
            try
            {
                var queryParameters = new DynamicParameters();

                // Handle null/empty lists
                queryParameters.Add("@DepartmentCodes",
                    ModelData?.DepartmentCodes?.Any() == true ? string.Join(",", ModelData.DepartmentCodes) : null);
                queryParameters.Add("@DesignationCodes",
                    ModelData?.DesignationCodes?.Any() == true ? string.Join(",", ModelData.DesignationCodes) : null);
                queryParameters.Add("@EmployeeCodes",
                    ModelData?.EmployeeCodes?.Any() == true ? string.Join(",", ModelData.EmployeeCodes) : null);
                queryParameters.Add("@BranchCodes",
                    ModelData?.BranchCodes?.Any() == true ? string.Join(",", ModelData.BranchCodes) : null);
                queryParameters.Add("@CompanyCodes",
                    ModelData?.CompanyCodes?.Any() == true ? string.Join(",", ModelData.CompanyCodes) : null);

                // Single value parameters
                queryParameters.Add("@EmployeeTypeCode", string.IsNullOrWhiteSpace(ModelData?.EmployeeTypeCode) ? null : ModelData.EmployeeTypeCode);
                queryParameters.Add("@EmploymentNatureId", string.IsNullOrWhiteSpace(ModelData?.EmploymentNatureId) ? null : ModelData.EmploymentNatureId);
                queryParameters.Add("@NationalId", string.IsNullOrWhiteSpace(ModelData?.NationalId) ? null : ModelData.NationalId);
                queryParameters.Add("@TinNo", string.IsNullOrWhiteSpace(ModelData?.TinNo) ? null : ModelData.TinNo);
                queryParameters.Add("@PassportNo", string.IsNullOrWhiteSpace(ModelData?.PassportNo) ? null : ModelData.PassportNo);
                queryParameters.Add("@DrivingLicense", string.IsNullOrWhiteSpace(ModelData?.DrivingLicense) ? null : ModelData.DrivingLicense);
                queryParameters.Add("@IsExpatriate", string.IsNullOrWhiteSpace(ModelData?.IsExpatriate) ? null : ModelData.IsExpatriate);
                queryParameters.Add("@ImmediateSup", string.IsNullOrWhiteSpace(ModelData?.ImmediateSup) ? null : ModelData.ImmediateSup);
                queryParameters.Add("@HOD", string.IsNullOrWhiteSpace(ModelData?.HOD) ? null : ModelData.HOD);
                queryParameters.Add("@ShiftCode", string.IsNullOrWhiteSpace(ModelData?.ShiftCode) ? null : ModelData.ShiftCode);
                queryParameters.Add("@EmployeeStatus", string.IsNullOrWhiteSpace(ModelData?.EmployeeStatus) ? null : ModelData.EmployeeStatus);

                // Numeric parameters
                queryParameters.Add("@SalaryFrom", ModelData?.SalaryFrom ?? (object)null);
                queryParameters.Add("@SalaryTo", ModelData?.SalaryTo ?? (object)null);

                // Date parameters
                queryParameters.Add("@AppointmentDateFrom", ModelData?.AppointmentDateFrom ?? (object)null);
                queryParameters.Add("@AppointmentDateTo", ModelData?.AppointmentDateTo ?? (object)null);
                queryParameters.Add("@JoiningDateFrom", ModelData?.JoiningDateFrom ?? (object)null);
                queryParameters.Add("@JoiningDateTo", ModelData?.JoiningDateTo ?? (object)null);
                queryParameters.Add("@TerminationDateFrom", ModelData?.TerminationDateFrom ?? (object)null);
                queryParameters.Add("@TerminationDateTo", ModelData?.TerminationDateTo ?? (object)null);
                queryParameters.Add("@ProbationDateFrom", ModelData?.ProbationDateFrom ?? (object)null);
                queryParameters.Add("@ProbationDateTo", ModelData?.ProbationDateTo ?? (object)null);
                queryParameters.Add("@ConfirmationDateFrom", ModelData?.ConfirmationDateFrom ?? (object)null);
                queryParameters.Add("@ConfirmationDateTo", ModelData?.ConfirmationDateTo ?? (object)null);

                using (var connection = new SqlConnection(_configuration.GetConnectionString("ApplicationDbConnection")))
                {
                    await connection.OpenAsync();

                    var result22 = (await connection.QueryAsync<EmployeeOfficialInfoDto>(
                        "GetOfficialInfoFilter",
                        queryParameters,
                        commandType: CommandType.StoredProcedure
                    )).ToList();
                    var result = (await connection.QueryAsync<EmployeeOfficialInfoDto>(
                        "GetOfficialInfoFilter",
                        queryParameters,
                        commandType: CommandType.StoredProcedure
                    )).ToList();

                    // Group by department
                    var grouped = new EmployeeReportGroupedDto();

                    if (result.Count == 0)
                    {
                        return grouped; // Empty
                    }

                    var groupedByDept = result
                        .GroupBy(e => e.DepartmentName)
                        .OrderBy(g => g.Key)
                        .ToList();

                    foreach (var deptGroup in groupedByDept)
                    {
                        var deptDto = new DepartmentEmployeeGroupDto
                        {
                            DepartmentName = deptGroup.Key ?? "Unknown Department",
                            Employees = deptGroup.ToList()
                        };

                        grouped.DepartmentGroups.Add(deptDto);
                    }

                    return grouped;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating report: {ex.Message}");
                return new EmployeeReportGroupedDto();
            }
        }

        #endregion

        #region PagePermissionAsync

        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Employee Official Info Report(List)" && x.TitleCheck);
        }

        #endregion

    }
}
