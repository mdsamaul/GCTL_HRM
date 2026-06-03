

using GCTL.Core.Data;
using GCTL.Core.ViewModels.MonthlyTransportExpenseDetailsReport;
using GCTL.Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace GCTL.Service.MonthlyTransportExpenseDetailsReportService
{
    public class MonthlyTransportExpenseDetailsReportServices : AppService<MonthlyTransportExpenseDetailsReportSetupDto>, IMonthlyTransportExpenseDetailsReportService
    {
        private readonly IRepository<MonthlyTransportExpenseDetailsReportSetupDto> MonthlyTransportExpenseDetailsReportSetupDtoRepo;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly string _connectionString;

        public MonthlyTransportExpenseDetailsReportServices(
           IRepository<MonthlyTransportExpenseDetailsReportSetupDto> MonthlyTransportExpenseDetailsReportSetupDtoRepo,
           IRepository<CoreAccessCode> accessCodeRepository,
           IConfiguration configuration
            ) : base(MonthlyTransportExpenseDetailsReportSetupDtoRepo)
        {
            this.MonthlyTransportExpenseDetailsReportSetupDtoRepo = MonthlyTransportExpenseDetailsReportSetupDtoRepo;
            this.accessCodeRepository = accessCodeRepository;
            _connectionString = configuration.GetConnectionString("ApplicationDbConnection");
        }

        public async Task<bool> PagePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Monthly TransportExpense Details Report" && x.TitleCheck);

        }

        public async Task<MonthlyTransportExpenseDetailsReportFilterResultListDto>
  GetAllTransportExpenseStatementDropdownSelectReportAsync(MonthlyTransportExpenseDetailsReportFilterDataDto filter)
        {
            try
            {
                var dropdownData = new MonthlyTransportExpenseDetailsReportFilterResultListDto
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
                        cmd.CommandText = "dbo.MonthlyTransportExpenseDetailsReport";
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

        public async Task<List<MonthlyTransportExpenseDetailsReportSetupDto>> GetAllTransportExpenseStatementResultReportAsync(
         MonthlyTransportExpenseDetailsReportFilterDataDto filter)
        {
            try
            {


                var list = new List<MonthlyTransportExpenseDetailsReportListDto>();

                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("MonthlyTransportExpenseDetailsReport", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

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



                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                list.Add(new MonthlyTransportExpenseDetailsReportListDto
                                {
                                    VehicleID = reader["VehicleID"].ToString(),
                                    VehicleNo = reader["VehicleNo"].ToString(),
                                    VehicleTypeID = reader["VehicleTypeID"].ToString(),
                                    VehicleType = reader["VehicleType"].ToString(),
                                    TransportCapacity = reader["TransportCapacity"].ToString(),
                                    EmployeeID = reader.IsDBNull(reader.GetOrdinal("EmployeeID")) ? null : reader.GetString(reader.GetOrdinal("EmployeeID")),
                                    FullName = reader.GetString(reader.GetOrdinal("FullName")),
                                    HelperId = reader.IsDBNull(reader.GetOrdinal("HelperId")) ? null : reader.GetString(reader.GetOrdinal("HelperId")),
                                    HelperName = reader.GetString(reader.GetOrdinal("Helper Name")),
                                    CNGGasBill = reader.GetDecimal(reader.GetOrdinal("CNG /Gas Bill")),
                                    RMBill = reader.GetDecimal(reader.GetOrdinal("R/M Bill")),
                                    FuelOctaneBill = reader.GetDecimal(reader.GetOrdinal("Fuel/Octane bill")),
                                    PoliceDonation = reader.GetDecimal(reader.GetOrdinal("Police Donation")),
                                    TollOthersBill = reader.GetDecimal(reader.GetOrdinal("Toll/others Bill")),
                                    TaxFitnessAndRoutePermit = reader.GetDecimal(reader.GetOrdinal("TAX ,Fitness And Rute Permit")),
                                    SalaryDriverHelper = reader.GetDecimal(reader.GetOrdinal("Salary(Driver,Helper)")),
                                    MechanicSalary = reader.GetDecimal(reader.GetOrdinal("Mechanic Salary")),
                                    MonthlyPoliceDonation = reader.GetDecimal(reader.GetOrdinal("Monthly Police Donation")),
                                    MonthlyEngineOilPurchase = reader.GetDecimal(reader.GetOrdinal("Monthly Eng.Oil Purchase")),
                                    AkeshTechnology = reader.GetDecimal(reader.GetOrdinal("Akesh Technology")),
                                    GarageRent = reader.GetDecimal(reader.GetOrdinal("Garage Rent")),
                                    TotalExpense = reader.GetDecimal(reader.GetOrdinal("TotalExpense")),
                                    ReportDate = reader.GetDateTime(reader.GetOrdinal("ReportDate"))
                                });
                            }
                        }
                    }
                }

                // Group by ReportDate
                var grouped = list
                    .GroupBy(x => x.ReportDate)
                    .Select(g => new MonthlyTransportExpenseDetailsReportSetupDto
                    {
                        ReportDate = g.Key,
                        ReportList = g.ToList()
                    })
                    .OrderBy(x => x.ReportDate)
                    .ToList();

                return grouped;
            }
            catch (Exception)
            {

                throw;
            }
        }



    }
}
