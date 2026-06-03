using GCTL.Core.Data;
using GCTL.Core.ViewModels.TransportExpenseStatementReport;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using CoreAccessCode = GCTL.Data.Models.CoreAccessCode;

namespace GCTL.Service.TransportExpenseStatementReportService
{
    public class TransportExpenseStatementReportServices : AppService<TransportExpenseStatementReportSetupViewModel>, ITransportExpenseStatementReportServices
    {
        private readonly IRepository<TransportExpenseStatementReportSetupViewModel> TransportExpenseStatementReportSetupViewModelRepo;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly string _connectionString;

        public TransportExpenseStatementReportServices(
           IRepository<TransportExpenseStatementReportSetupViewModel> TransportExpenseStatementReportSetupViewModelRepo,
           IRepository<CoreAccessCode> accessCodeRepository,
           IConfiguration configuration
            ) : base(TransportExpenseStatementReportSetupViewModelRepo)
        {
            this.TransportExpenseStatementReportSetupViewModelRepo = TransportExpenseStatementReportSetupViewModelRepo;
            this.accessCodeRepository = accessCodeRepository;
            _connectionString = configuration.GetConnectionString("ApplicationDbConnection");
        }

        public async Task<bool> PagePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Transport Expense Statement" && x.TitleCheck);

        }
        public async Task<TransportExpenseStatementReportFilterResultListDto>
  GetAllTransportExpenseStatementDropdownSelectReportAsync(TransportExpenseStatementReportFilterDataDto filter)
        {
            try
            {
                var dropdownData = new TransportExpenseStatementReportFilterResultListDto
                {
                    TransportTypeIds = new List<DropdownItemDto>(),
                    TransportIds = new List<DropdownItemDto>(),
                    DriverEmployeeIds = new List<DropdownItemDto>()
                };

                using (var conn = new SqlConnection(_connectionString))
                {
                    if (conn.State != ConnectionState.Open)
                        await conn.OpenAsync();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "dbo.USP_TransportExpenseReport";
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Convert List<string> → comma separated string (or DBNull.Value if null/empty)
                        cmd.Parameters.Add(new SqlParameter("@TransportTypeIDs",
                            (filter.TransportTypeIds != null && filter.TransportTypeIds.Any())
                                ? string.Join(",", filter.TransportTypeIds)
                                : (object)DBNull.Value));

                        cmd.Parameters.Add(new SqlParameter("@TransportNos",
                            (filter.TransportIds != null && filter.TransportIds.Any())
                                ? string.Join(",", filter.TransportIds)
                                : (object)DBNull.Value));

                        cmd.Parameters.Add(new SqlParameter("@Drivers",
                            (filter.DriverEmployeeIds != null && filter.DriverEmployeeIds.Any())
                                ? string.Join(",", filter.DriverEmployeeIds)
                                : (object)DBNull.Value));

                        cmd.Parameters.Add(new SqlParameter("@FromDate", filter.FromDate ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@ToDate", filter.ToDate ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@Month", filter.Month ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@Year", filter.Year ?? (object)DBNull.Value));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                // Transport Type
                                var transportTypeId = reader["VehicleTypeID"]?.ToString();
                                var transportTypeName = reader["VehicleType"]?.ToString();
                                if (!string.IsNullOrEmpty(transportTypeId) &&
                                    !dropdownData.TransportTypeIds.Exists(x => x.Id == transportTypeId))
                                {
                                    dropdownData.TransportTypeIds.Add(new DropdownItemDto
                                    {
                                        Id = transportTypeId,
                                        Name = transportTypeName
                                    });
                                }

                                // Transport
                                var vehicleId = reader["VehicleID"]?.ToString();
                                var vehicleNo = reader["VehicleNo"]?.ToString();
                                if (!string.IsNullOrEmpty(vehicleId) &&
                                    !dropdownData.TransportIds.Exists(x => x.Id == vehicleId))
                                {
                                    dropdownData.TransportIds.Add(new DropdownItemDto
                                    {
                                        Id = vehicleId,
                                        Name = vehicleNo
                                    });
                                }

                                // Driver
                                var employeeId = reader["EmployeeID"]?.ToString();
                                var fullName = reader["FullName"]?.ToString();
                                if (!string.IsNullOrEmpty(employeeId) &&
                                    !dropdownData.DriverEmployeeIds.Exists(x => x.Id == employeeId))
                                {
                                    dropdownData.DriverEmployeeIds.Add(new DropdownItemDto
                                    {
                                        Id = employeeId,
                                        Name = fullName
                                    });
                                }
                            }
                        }
                    }
                }

                return dropdownData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching Transport Expense Statement Report dropdown data", ex);
            }
        }
        public async Task<List<TransportExpenseStatementReportSetupViewModel>> GetAllTransportExpenseStatementResultReportAsync(
    TransportExpenseStatementReportFilterDataDto filter)
        {
            try
            {
                var result = new List<TransportExpenseStatementReportSetupViewModel>();

                using (var conn = new SqlConnection(_connectionString))
                {
                    if (conn.State != ConnectionState.Open)
                        await conn.OpenAsync();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "dbo.USP_TransportExpenseReport";
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Convert list to comma separated string (or DBNull if null/empty)
                        cmd.Parameters.Add(new SqlParameter("@TransportTypeIDs",
                            (filter.TransportTypeIds != null && filter.TransportTypeIds.Any())
                                ? string.Join(",", filter.TransportTypeIds)
                                : (object)DBNull.Value));

                        cmd.Parameters.Add(new SqlParameter("@TransportNos",
                            (filter.TransportIds != null && filter.TransportIds.Any())
                                ? string.Join(",", filter.TransportIds)
                                : (object)DBNull.Value));

                        cmd.Parameters.Add(new SqlParameter("@Drivers",
                            (filter.DriverEmployeeIds != null && filter.DriverEmployeeIds.Any())
                                ? string.Join(",", filter.DriverEmployeeIds)
                                : (object)DBNull.Value));

                        cmd.Parameters.Add(new SqlParameter("@FromDate", filter.FromDate ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@ToDate", filter.ToDate ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@Month", filter.Month ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@Year", filter.Year ?? (object)DBNull.Value));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                result.Add(new TransportExpenseStatementReportSetupViewModel
                                {
                                    VehicleID = reader["VehicleID"]?.ToString(),
                                    VehicleNo = reader["VehicleNo"]?.ToString(),
                                    VehicleTypeID = reader["VehicleTypeID"].ToString(),
                                    VehicleType = reader["VehicleType"]?.ToString(),
                                    TransportCapacity = reader["TransportCapacity"] as decimal?,
                                    EmployeeID = reader["EmployeeID"]?.ToString(),
                                    FullName = reader["FullName"]?.ToString(),
                                    HelperId = reader["HelperId"]?.ToString(),
                                    HelperName = reader["Helper Name"]?.ToString(),

                                    CNGGasBill = reader["CNG /Gas Bill"] != DBNull.Value ? Convert.ToDecimal(reader["CNG /Gas Bill"]) : 0,
                                    RMBill = reader["R/M Bill"] != DBNull.Value ? Convert.ToDecimal(reader["R/M Bill"]) : 0,
                                    FuelOctaneBill = reader["Fuel/Octane bill"] != DBNull.Value ? Convert.ToDecimal(reader["Fuel/Octane bill"]) : 0,
                                    PoliceDonation = reader["Police Donation"] != DBNull.Value ? Convert.ToDecimal(reader["Police Donation"]) : 0,
                                    TollOthersBill = reader["Toll/others Bill"] != DBNull.Value ? Convert.ToDecimal(reader["Toll/others Bill"]) : 0,
                                    TAXFitnessAndRutePermit = reader["TAX ,Fitness And Rute Permit"] != DBNull.Value ? Convert.ToDecimal(reader["TAX ,Fitness And Rute Permit"]) : 0,
                                    SalaryDriverHelper = reader["Salary(Driver,Helper)"] != DBNull.Value ? Convert.ToDecimal(reader["Salary(Driver,Helper)"]) : 0,
                                    MechanicSalary = reader["Mechanic Salary"] != DBNull.Value ? Convert.ToDecimal(reader["Mechanic Salary"]) : 0,
                                    MonthlyPoliceDonation = reader["Monthly Police Donation"] != DBNull.Value ? Convert.ToDecimal(reader["Monthly Police Donation"]) : 0,
                                    MonthlyEngOilPurchase = reader["Monthly Eng.Oil Purchase"] != DBNull.Value ? Convert.ToDecimal(reader["Monthly Eng.Oil Purchase"]) : 0,
                                    AkeshTechnology = reader["Akesh Technology"] != DBNull.Value ? Convert.ToDecimal(reader["Akesh Technology"]) : 0,
                                    GarageRent = reader["Garage Rent"] != DBNull.Value ? Convert.ToDecimal(reader["Garage Rent"]) : 0,
                                    TotalExpense = reader["TotalExpense"] != DBNull.Value ? Convert.ToDecimal(reader["TotalExpense"]) : 0
                                });
                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching Transport Expense Statement Report data", ex);
            }
        }
        public async Task<List<TransportExpenseStatementReportSetupViewModel>> GetAllTransportExpenseStatementResultReportExcelAsync(
  TransportExpenseStatementReportFilterDataDto filter)
        {
            try
            {
                var result = new List<TransportExpenseStatementReportSetupViewModel>();

                using (var conn = new SqlConnection(_connectionString))
                {
                    if (conn.State != ConnectionState.Open)
                        await conn.OpenAsync();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "dbo.USP_TransportExpenseReport";
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Convert list to comma separated string (or DBNull if null/empty)
                        cmd.Parameters.Add(new SqlParameter("@TransportTypeIDs",
                            (filter.TransportTypeIds != null && filter.TransportTypeIds.Any())
                                ? string.Join(",", filter.TransportTypeIds)
                                : (object)DBNull.Value));

                        cmd.Parameters.Add(new SqlParameter("@TransportNos",
                            (filter.TransportIds != null && filter.TransportIds.Any())
                                ? string.Join(",", filter.TransportIds)
                                : (object)DBNull.Value));

                        cmd.Parameters.Add(new SqlParameter("@Drivers",
                            (filter.DriverEmployeeIds != null && filter.DriverEmployeeIds.Any())
                                ? string.Join(",", filter.DriverEmployeeIds)
                                : (object)DBNull.Value));

                        cmd.Parameters.Add(new SqlParameter("@FromDate", filter.FromDate ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@ToDate", filter.ToDate ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@Month", filter.Month ?? (object)DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@Year", filter.Year ?? (object)DBNull.Value));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                result.Add(new TransportExpenseStatementReportSetupViewModel
                                {
                                    VehicleID = reader["VehicleID"]?.ToString(),
                                    VehicleNo = reader["VehicleNo"]?.ToString(),
                                    VehicleTypeID = reader["VehicleTypeID"].ToString(),
                                    VehicleType = reader["VehicleType"]?.ToString(),
                                    TransportCapacity = reader["TransportCapacity"] as decimal?,
                                    EmployeeID = reader["EmployeeID"]?.ToString(),
                                    FullName = reader["FullName"]?.ToString(),
                                    HelperId = reader["HelperId"]?.ToString(),
                                    HelperName = reader["Helper Name"]?.ToString(),

                                    CNGGasBill = reader["CNG /Gas Bill"] != DBNull.Value ? Convert.ToDecimal(reader["CNG /Gas Bill"]) : 0,
                                    RMBill = reader["R/M Bill"] != DBNull.Value ? Convert.ToDecimal(reader["R/M Bill"]) : 0,
                                    FuelOctaneBill = reader["Fuel/Octane bill"] != DBNull.Value ? Convert.ToDecimal(reader["Fuel/Octane bill"]) : 0,
                                    PoliceDonation = reader["Police Donation"] != DBNull.Value ? Convert.ToDecimal(reader["Police Donation"]) : 0,
                                    TollOthersBill = reader["Toll/others Bill"] != DBNull.Value ? Convert.ToDecimal(reader["Toll/others Bill"]) : 0,
                                    TAXFitnessAndRutePermit = reader["TAX ,Fitness And Rute Permit"] != DBNull.Value ? Convert.ToDecimal(reader["TAX ,Fitness And Rute Permit"]) : 0,
                                    SalaryDriverHelper = reader["Salary(Driver,Helper)"] != DBNull.Value ? Convert.ToDecimal(reader["Salary(Driver,Helper)"]) : 0,
                                    MechanicSalary = reader["Mechanic Salary"] != DBNull.Value ? Convert.ToDecimal(reader["Mechanic Salary"]) : 0,
                                    MonthlyPoliceDonation = reader["Monthly Police Donation"] != DBNull.Value ? Convert.ToDecimal(reader["Monthly Police Donation"]) : 0,
                                    MonthlyEngOilPurchase = reader["Monthly Eng.Oil Purchase"] != DBNull.Value ? Convert.ToDecimal(reader["Monthly Eng.Oil Purchase"]) : 0,
                                    AkeshTechnology = reader["Akesh Technology"] != DBNull.Value ? Convert.ToDecimal(reader["Akesh Technology"]) : 0,
                                    GarageRent = reader["Garage Rent"] != DBNull.Value ? Convert.ToDecimal(reader["Garage Rent"]) : 0,
                                    TotalExpense = reader["TotalExpense"] != DBNull.Value ? Convert.ToDecimal(reader["TotalExpense"]) : 0
                                });
                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching Transport Expense Statement Report data", ex);
            }
        }


    }
}
