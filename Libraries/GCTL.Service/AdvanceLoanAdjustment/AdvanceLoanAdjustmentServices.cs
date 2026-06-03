using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.Data;
using GCTL.Core.ViewModels.AdvanceLoanAdjustment;
using GCTL.Core.ViewModels.Companies;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Data.Models;
using GCTL.Service.DeleteHistories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NPOI.SS.Formula.Functions;

namespace GCTL.Service.AdvanceLoanAdjustment
{
    public class AdvanceLoanAdjustmentServices : AppService<HrmPayAdvancePay>, IAdvanceLoanAdjustmentServices
    {
        private readonly IRepository<HrmPayAdvancePay> advancePayRepo;
        private readonly IConfiguration configuration;
        private readonly IRepository<HrmEmployee> empRepo;
        private readonly IRepository<HrmPayrollLoan> payrollLoanRepo;
        private readonly IRepository<HrmPayLoanTypeEntry> loanTypeRepo;
        private readonly IRepository<HrmPayPayHeadName> payHeadRepo;
        private readonly IRepository<HrmPayMonth> monthRepo;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly IRepository<HrmPayPayHeadName> headRepo;
        private readonly IDeleteHistoryService deleteHistoryService;

        public AdvanceLoanAdjustmentServices(
            IRepository<HrmPayAdvancePay> advancePayRepo,
            IConfiguration configuration,
            IRepository<HrmEmployee> empRepo,
            IRepository<HrmPayrollLoan> payrollLoanRepo,
            IRepository<HrmPayLoanTypeEntry> LoanTypeRepo,
            IRepository<HrmPayPayHeadName> payHeadRepo,
            IRepository<HrmPayMonth> monthRepo,
            IRepository<CoreAccessCode> accessCodeRepository,
            IRepository<HrmPayPayHeadName> headRepo,
            IDeleteHistoryService deleteHistoryService
        ) : base(advancePayRepo)
        {
            this.advancePayRepo = advancePayRepo;
            this.configuration = configuration;
            this.empRepo = empRepo;
            this.payrollLoanRepo = payrollLoanRepo;
            this.loanTypeRepo = LoanTypeRepo;
            this.payHeadRepo = payHeadRepo;
            this.monthRepo = monthRepo;
            this.accessCodeRepository = accessCodeRepository;
            this.headRepo = headRepo;
            this.deleteHistoryService = deleteHistoryService;
        }
        private readonly string CreateSuccess = "Data saved successfully.";
        private readonly string CreateFailed = "Data insertion failed.";
        private readonly string UpdateSuccess = "Data updated successfully.";
        private readonly string UpdateFailed = "Data update failed.";
        private readonly string DeleteSuccess = "Data deleted successfully.";
        private readonly string DeleteFailed = "Data deletion failed.";
        private readonly string DataExists = "Data already exists.";

        #region Permission all type

        public async Task<bool> PagePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Advance/ Loan Adjustment" && x.TitleCheck);

        }

        public async Task<bool> SavePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Advance/ Loan Adjustment" && x.CheckAdd);

        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Advance/ Loan Adjustment" && x.CheckEdit);

        }

        public async Task<bool> DeletePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Advance/ Loan Adjustment" && x.CheckDelete);

        }

        #endregion



        public async Task<List<CompanyDto>> GetAllAndFilterCompanyAsync(string searchCompanyName)
        {
            List<CompanyDto> companies = new List<CompanyDto>();

            using (SqlConnection conn = new SqlConnection(configuration.GetConnectionString("ApplicationDbConnection")))
            {
                await conn.OpenAsync(); 

                using (SqlCommand cmd = new SqlCommand("GetCompanyNamesBySearch", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SearchCompanyName", searchCompanyName ?? "");

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync()) 
                    {
                        while (await reader.ReadAsync()) 
                        {
                            companies.Add(new CompanyDto
                            {
                                companyCode = reader["CompanyCode"].ToString(),
                                companyName = reader["CompanyName"].ToString()
                            });
                        }
                    }
                }
            }

            return companies;
        }
        
        public async Task<List<EmployeeAdjustmentDto>> GetEmployeesByFilterAsync(string employeeStatusId, string companyCode, string employeeName, bool loanAdjustment)
        {
            List<EmployeeAdjustmentDto> employees = new List<EmployeeAdjustmentDto>();

            try
            {
                var sPName = loanAdjustment ? "GetEmployeesByCompanyLoanAdjustment" : "GetEmployeesByCompanyAdvanceLoanAdjustment";

                using (SqlConnection conn = new SqlConnection(configuration.GetConnectionString("ApplicationDbConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand(sPName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@EmployeeStatusId", employeeStatusId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CompanyCode", companyCode ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@EmployeeName", employeeName ?? (object)DBNull.Value);    

                        await conn.OpenAsync();
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var employee = new EmployeeAdjustmentDto
                                {
                                    EmployeeId = reader["EmployeeID"].ToString(),
                                    FullName = reader["FullName"].ToString(),
                                    DepartmentName = reader["DepartmentName"].ToString(),
                                    DesignationName = reader["DesignationName"].ToString(),
                                    JoiningDate = Convert.ToDateTime(reader["JoiningDate"]).ToString("dd/MM/yyyy")
                                };

                                if (loanAdjustment && reader["LoanId"] != DBNull.Value)
                                {
                                    employee.LoanId = reader["LoanId"].ToString();
                                }

                                employees.Add(employee);
                            }
                        }
                    }
                }

                return employees;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<EmployeeAdjustmentDto> GetLoadEmployeeByIdAsync(string employeeId)
        {
            if(employeeId == null)
            {
                return null;
            }
            EmployeeAdjustmentDto employees = new EmployeeAdjustmentDto();

            try
            {
                using (SqlConnection conn = new SqlConnection(configuration.GetConnectionString("ApplicationDbConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand("GetEmployeesByCompanyAdvanceLoanAdjustment", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@EmployeeStatusId", "01" ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CompanyCode", "001" ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@EmployeeName", employeeId ?? (object)DBNull.Value);

                        await conn.OpenAsync();
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                employees = (new EmployeeAdjustmentDto
                                {
                                    EmployeeId = reader["EmployeeID"].ToString(),
                                    FullName = reader["FullName"].ToString(),
                                    DepartmentName = reader["DepartmentName"].ToString(),
                                    DesignationName = reader["DesignationName"].ToString(),
                                    //JoiningDate = Convert.ToDateTime(reader["JoiningDate"])
                                    JoiningDate = Convert.ToDateTime(reader["JoiningDate"]).ToString("dd/MM/yyyy"),
                                });
                            }
                        }
                    }
                }

                return employees;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<LoanDataDto>> GetLoanByEmployeeIdAsync(string employeeId)
        {
            if(employeeId == null)
            {
                return null;
            }
            try
            {
                var payrollLoans = payrollLoanRepo.All().Where(x => x.EmployeeId == employeeId).Select(x => new LoanDataDto
                {
                    LoanId = x.LoanId,
                    LoanDate = x.LoanDate.HasValue ? x.LoanDate.Value.ToString("dd/MM/yyyy") : "",
                    LoanType = x.LoanTypeId != null ? loanTypeRepo.All().Where(x => x.LoanTypeId == x.LoanTypeId).Select(x => x.LoanType).FirstOrDefault() : "",
                    LoanStartEndDate = (x.StartDate.HasValue ? x.StartDate.Value.ToString("dd/MM/yyyy") : "") + " - " + (x.EndDate.HasValue ? x.EndDate.Value.ToString("dd/MM/yyyy") : ""),
                    NoOfInstallment = x.NoOfInstallment != "0" ? x.NoOfInstallment : ""
                }).ToList();
                return payrollLoans;
            }catch(Exception)
            {
                throw;
            }
        }

       public async Task<LoanDataDto> GetLoanByIdAsync(string loanId)
        {
            if (loanId == null)
            {
                return null;
            }
            try
            {
                var loan = payrollLoanRepo.All().Where(x => x.LoanId == loanId).Select(x => new LoanDataDto
                {
                    LoanId = x.LoanId,
                    LoanDate = x.LoanDate.HasValue ? x.LoanDate.Value.ToString("dd/MM/yyyy") : "",
                    LoanType = x.LoanTypeId != null ? loanTypeRepo.All().Where(x => x.LoanTypeId == x.LoanTypeId).Select(x => x.LoanType).FirstOrDefault() : "",
                    LoanStartEndDate = (x.StartDate.HasValue ? x.StartDate.Value.ToString("dd/MM/yyyy") : "") + " - " + (x.EndDate.HasValue ? x.EndDate.Value.ToString("dd/MM/yyyy") : ""),
                    NoOfInstallment = x.NoOfInstallment != "0" ? x.NoOfInstallment : "",
                   LoanAmount= x.LoanAmount,
                   StarDate = x.StartDate.HasValue? x.StartDate.Value.ToString("dd/MM/yyyy"):"" ,
                   EndDate = x.EndDate.HasValue? x.EndDate.Value.ToString("dd/MM/yyyy"):"" ,
                   MonthlyDeduction = x.MonthlyDeduction,
                   PayHeadNameName = x.PayHeadNameId != null ? payHeadRepo.All().Where(x=> x.PayHeadNameId == x.PayHeadNameId).Select(x=>x.Name).FirstOrDefault():"",
                   PayHeadNameId = x.PayHeadNameId
                }).FirstOrDefault();
                return loan;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Helper: Month name → number
        private static int MonthNameToNumber(string monthName) => monthName switch
        {
            "January" => 1,
            "February" => 2,
            "March" => 3,
            "April" => 4,
            "May" => 5,
            "June" => 6,
            "July" => 7,
            "August" => 8,
            "September" => 9,
            "October" => 10,
            "November" => 11,
            "December" => 12,
            _ => 0
        };

        // Helper: number → Month name
        private static string MonthNumberToName(int month) => month switch
        {
            1 => "January",
            2 => "February",
            3 => "March",
            4 => "April",
            5 => "May",
            6 => "June",
            7 => "July",
            8 => "August",
            9 => "September",
            10 => "October",
            11 => "November",
            12 => "December",
            _ => ""
        };

        // Create loan adjustment installments
        public async Task<(bool isSuccess, string message, object data)> SaveUpdateLoanAdjustmentAsync(AdvanceLoanAdjustmentSetupViewModel modelData)
        {
            if (modelData == null|| modelData.AdjustmentType == "" || modelData.AdjustmentType == null || modelData.EmployeeID == "" || modelData.EmployeeID == null || modelData.AdvanceAmount < 0 || modelData.AdvanceAmount == null)
                return (false, CreateFailed, null);

            bool isExist;
          
            if(modelData.AdjustmentType == "Advance" && modelData.AdvancePayCode == 0)
            {
                isExist = advancePayRepo.All().Any(x =>  x.EmployeeId == modelData.EmployeeID && x.SalaryMonth == modelData.SalaryMonth && x.SalaryYear == modelData.SalaryYear);
                if (isExist)
                {
                    return (false, DataExists, null);
                }
            }
            

            if(modelData.AdvancePayCode != 0)
            {


                //try
                //{
                //    var existing = advancePayRepo.All()
                //        .FirstOrDefault(x => x.AdvancePayId == modelData.AdvancePayId);
                //    if (existing == null)
                //        return (false, "Data not found", null);

                //    var allRows = advancePayRepo.All()
                //        .Where(x => x.LoanId == modelData.LoanID
                //                 && x.EmployeeId == modelData.EmployeeID)
                //        .OrderBy(x => x.AdvancePayId)
                //        .ToList();

                //    int currentIndex = allRows.FindIndex(x => x.AdvancePayId == modelData.AdvancePayId);
                //    if (currentIndex == -1)
                //        return (false, "Row not found", null);

                //    decimal totalLoan = modelData.AdvanceAmount;
                //    decimal perMonth = modelData.MonthlyDeduction;

                //    decimal prevSum = allRows
                //        .Take(currentIndex)
                //        .Sum(x => x.MonthlyDeduction);

                //    if (prevSum >= totalLoan)
                //        return (false, "Loan already completed before this row", null);

                //    decimal remainingAfterPrev = totalLoan - prevSum;
                //    var currentRow = allRows[currentIndex];
                //    currentRow.MonthlyDeduction = Math.Min(remainingAfterPrev, perMonth);
                //    currentRow.AdvanceAmount = modelData.AdvanceAmount;
                //    currentRow.ModifyDate = DateTime.Now;

                //    decimal runningTotal = prevSum + currentRow.MonthlyDeduction;
                //    await advancePayRepo.UpdateAsync(currentRow);

                //    var rowsToDelete = new List<HrmPayAdvancePay>();
                //    for (int i = currentIndex + 1; i < allRows.Count; i++)
                //    {
                //        var row = allRows[i];

                //        if (runningTotal >= totalLoan)
                //        {
                //            // loan complete, বাকি rows delete
                //            rowsToDelete.Add(row);
                //            continue;
                //        }

                //        decimal remaining = totalLoan - runningTotal;

                //        if (remaining >= row.MonthlyDeduction)
                //        {
            
           
                //            row.AdvanceAmount = modelData.AdvanceAmount;
                //            row.ModifyDate = DateTime.Now;
                //            runningTotal += row.MonthlyDeduction; 
                //        }
                //        else
                //        {
                //            row.MonthlyDeduction = remaining;
                //            row.AdvanceAmount = modelData.AdvanceAmount;
                //            row.ModifyDate = DateTime.Now;
                //            runningTotal += remaining;
                //        }

                //        await advancePayRepo.UpdateAsync(row);
                //    }

                //    if (rowsToDelete.Any())
                //        await advancePayRepo.DeleteRangeAsync(rowsToDelete);

                //    return (true, "Update successful", currentRow);
                //}
                //catch (Exception)
                //{
                //    throw;
                //}

                try
                {
                    var existing = advancePayRepo.All()
                        .FirstOrDefault(x => x.AdvancePayId == modelData.AdvancePayId);
                    if (existing == null)
                        return (false, "Data not found", null);

                    var allRows = advancePayRepo.All()
                        .Where(x => x.LoanId == modelData.LoanID
                                 && x.EmployeeId == modelData.EmployeeID)
                        .OrderBy(x => x.AdvancePayId)
                        .ToList();

                    int currentIndex = allRows.FindIndex(x => x.AdvancePayId == modelData.AdvancePayId);
                    if (currentIndex == -1)
                        return (false, "Row not found", null);

                    decimal totalLoan = modelData.AdvanceAmount;
                    decimal perMonth = modelData.MonthlyDeduction;

                    decimal prevSum = allRows
                        .Take(currentIndex)
                        .Sum(x => x.MonthlyDeduction);

                    if (prevSum >= totalLoan)
                        return (false, "Loan already completed before this row", null);

                    decimal remainingAfterPrev = totalLoan - prevSum;
                    var currentRow = allRows[currentIndex];
                    currentRow.MonthlyDeduction = Math.Min(remainingAfterPrev, perMonth);
                    currentRow.AdvanceAmount = modelData.AdvanceAmount;
                    currentRow.ModifyDate = DateTime.Now;

                    decimal runningTotal = prevSum + currentRow.MonthlyDeduction;
                    await advancePayRepo.UpdateAsync(currentRow);

                    var rowsToDelete = new List<HrmPayAdvancePay>();
                    for (int i = currentIndex + 1; i < allRows.Count; i++)
                    {
                        var row = allRows[i];

                        if (runningTotal >= totalLoan)
                        {
                            rowsToDelete.Add(row);
                            continue;
                        }

                        decimal remaining = totalLoan - runningTotal;

                        if (remaining >= row.MonthlyDeduction)
                        {
                            row.AdvanceAmount = modelData.AdvanceAmount;
                            row.ModifyDate = DateTime.Now;
                            runningTotal += row.MonthlyDeduction;
                        }
                        else
                        {
                            row.MonthlyDeduction = remaining;
                            row.AdvanceAmount = modelData.AdvanceAmount;
                            row.ModifyDate = DateTime.Now;
                            runningTotal += remaining;
                        }

                        await advancePayRepo.UpdateAsync(row);
                    }

                    if (rowsToDelete.Any())
                        await advancePayRepo.DeleteRangeAsync(rowsToDelete);

                    if (runningTotal < totalLoan)
                    {
                        var lastRow = allRows
                            .Where(x => !rowsToDelete.Contains(x))
                            .LastOrDefault();

                        int nextMonth = MonthNameToNumber(lastRow?.SalaryMonth ?? modelData.SalaryMonth);
                        int nextYear = Convert.ToInt32( lastRow?.SalaryYear ?? modelData.SalaryYear);

                        nextMonth++;
                        if (nextMonth > 12)
                        {
                            nextMonth = 1;
                            nextYear++;
                        }

                        var lastDbRow = advancePayRepo.All()
        .OrderByDescending(x => x.AdvancePayId)
        .FirstOrDefault();

                        int nextId = 1;
                        if (lastDbRow != null && !string.IsNullOrEmpty(lastDbRow.AdvancePayId))
                        {
                            nextId = int.Parse(lastDbRow.AdvancePayId) + 1;
                        }

                        while (runningTotal < totalLoan)
                        {
                            decimal remaining = totalLoan - runningTotal;
                            decimal thisMonthDeduction = Math.Min(remaining, perMonth);

                            var newRow = new HrmPayAdvancePay
                            {
                               
                                
                                
                                MonthlyDeduction = thisMonthDeduction,
                                SalaryMonth = MonthNumberToName(nextMonth), 
                                SalaryYear = nextYear.ToString(),
                               
                                ModifyDate = DateTime.Now,




                                AdvancePayId = nextId.ToString("D8"),
                                EmployeeId = modelData.EmployeeID,
                                AdvanceAdjustStatus = modelData.AdvanceAdjustStatus,
                                AdvanceAmount = modelData.AdvanceAmount,
                               

                               

                                NoOfPaymentInstallment = modelData.NoOfPaymentInstallment,
                                PayHeadNameId = modelData.PayHeadNameId,
                                Remarks = modelData.Remarks,
                                Luser = modelData.Luser,
                                Ldate = DateTime.Now,
                                Lip = modelData.Lip,
                                Lmac = modelData.Lmac,
                                AdjustmentType = modelData.AdjustmentType,
                                LoanId = modelData.LoanID,
                                CompanyCode = "001"
                            };

                            await advancePayRepo.AddAsync(newRow);
                            runningTotal += thisMonthDeduction;
                            nextId++;
                            nextMonth++;
                            if (nextMonth > 12)
                            {
                                nextMonth = 1;
                                nextYear++;
                            }
                        }
                    }

                    return (true, "Update successful", currentRow);
                }
                catch (Exception)
                {
                    throw;
                }

            }
            int advancePayId =int.Parse(modelData.AdvancePayId);
            List<HrmPayAdvancePay> installments = new List<HrmPayAdvancePay>();
            if(modelData.AdvanceAdjustStatus == "By Month")
            {
               

                var installment = new HrmPayAdvancePay
                {
                    AdvancePayId = advancePayId.ToString("D8"),
                    EmployeeId = modelData.EmployeeID,
                    AdvanceAdjustStatus = modelData.AdvanceAdjustStatus,
                    AdvanceAmount = modelData.AdvanceAmount,
                    MonthlyDeduction = modelData.MonthlyDeduction,

                    SalaryMonth = modelData.SalaryMonth, 
                    SalaryYear = modelData.SalaryYear,

                    NoOfPaymentInstallment = modelData.NoOfPaymentInstallment,
                    PayHeadNameId = modelData.PayHeadNameId,
                    Remarks = modelData.Remarks,
                    Luser = modelData.Luser,
                    Ldate = DateTime.Now,
                    Lip = modelData.Lip,
                    Lmac = modelData.Lmac,
                    AdjustmentType = modelData.AdjustmentType,
                    LoanId = modelData.LoanID,
                    CompanyCode = "001"
                };

                installments.Add(installment);
                advancePayId++;
            }

            if (modelData.AdvanceAdjustStatus == "By Date")
            {
                if (modelData.FromDate == null || modelData.Todate == null)
                    return (false, CreateFailed, null);

                DateTime fromDate = modelData.FromDate.Value;
                DateTime toDate = modelData.Todate.Value;
                while (fromDate <= toDate)
                {


                   
                        isExist = advancePayRepo.All().Any(x => x.LoanId == modelData.LoanID && x.EmployeeId == modelData.EmployeeID && x.SalaryMonth == fromDate.ToString("MMMM") && x.SalaryYear == fromDate.Year.ToString());
                        if (!isExist)
                        {
                            var installment = new HrmPayAdvancePay
                            {
                                AdvancePayId = advancePayId.ToString("D8"),
                                EmployeeId = modelData.EmployeeID,
                                AdvanceAdjustStatus = modelData.AdvanceAdjustStatus,
                                AdvanceAmount = modelData.AdvanceAmount,
                                MonthlyDeduction = modelData.MonthlyDeduction,

                                SalaryMonth = fromDate.ToString("MMMM"), 
                                SalaryYear = fromDate.Year.ToString(),

                                NoOfPaymentInstallment = modelData.NoOfPaymentInstallment,
                                PayHeadNameId = modelData.PayHeadNameId,
                                Remarks = modelData.Remarks,
                                Luser = modelData.Luser,
                                Ldate = DateTime.Now,
                                Lip = modelData.Lip,
                                Lmac = modelData.Lmac,
                                AdjustmentType = modelData.AdjustmentType,
                                LoanId = modelData.LoanID,
                                CompanyCode = "001"
                            };

                            installments.Add(installment);
                            fromDate = fromDate.AddMonths(1);
                            advancePayId++;
                    }
                    else
                    {
                        fromDate = fromDate.AddMonths(1);
                    }
                    

                    
                }
            }

          

            try
            {
                foreach (var item in installments)
                {
                    await advancePayRepo.AddAsync(item);
                }

                return (true,CreateSuccess, null);
            }
            catch (Exception ex)
            {
                return (false, $"Failed to save installments: {ex.Message}", null);
            }
        }

        //auto id 
        public async Task<string> AdjustmentAutoGanarateIdAsync()
        {
            var lastItem = advancePayRepo.All().OrderByDescending(x => x.AdvancePayId).FirstOrDefault();

            string newId;
            if (lastItem != null && !string.IsNullOrEmpty(lastItem.AdvancePayId))
            {
                string numericPart = lastItem.AdvancePayId.Substring(1); 
                int number = int.Parse(numericPart) + 1;
                newId = number.ToString("D8");
            }
            else
            {
                newId = "00000001";
            }

            return newId;
        }

        //get month
      public async  Task<List<MonthDto>> GetMonthAsync()
        {
            var monthAll = monthRepo.All().OrderBy(x => x.MonthId).ToList();
            var months = monthAll.Select(x => new MonthDto
            {
                MonthId = x.MonthId,
                MonthName = x.MonthName,
            }).ToList();
            return months;
        }
        //get Deduction Heads
        public async  Task<List<PayHeadNameDto>> GetHeadDeductionAsync()
        {
            var deductionHeadAll = headRepo.All().OrderBy(x => x.PayHeadNameId).ToList();
            var head = deductionHeadAll.Select(x => new PayHeadNameDto
            {
                 PayHeadNameId= x.PayHeadNameId,
                Name = x.Name,
            }).ToList();
            return head;
        }

        // Helper method for safe conversion
        private static int SafeConvertToInt32(object value, int defaultValue = 0)
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;

            if (int.TryParse(value.ToString(), out int result))
                return result;

            return defaultValue;
        }

        private static decimal SafeConvertToDecimal(object value, decimal defaultValue = 0)
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;

            if (decimal.TryParse(value.ToString(), out decimal result))
                return result;

            return defaultValue;
        }

        public async Task<DataTableResponse<AdvancePayViewModel>> GetAdvancePayPaged(DataTableRequest request)
        {
            var response = new DataTableResponse<AdvancePayViewModel>
            {
                Data = new List<AdvancePayViewModel>(),
                TotalRecords = 0,
                FilteredRecords = 0
            };

            try
            {
                using (SqlConnection con = new SqlConnection(configuration.GetConnectionString("ApplicationDbConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand("GetAdvancePayPagedWithFilter", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PageNumber", request.Page);
                        cmd.Parameters.AddWithValue("@PageSize", request.PageSize);
                        cmd.Parameters.AddWithValue("@SearchValue", request.SearchValue ?? "");
                        cmd.Parameters.AddWithValue("@Department", request.Department ?? "");
                        cmd.Parameters.AddWithValue("@Month", request.Month ?? "");
                        cmd.Parameters.AddWithValue("@Year", request.Year ?? "");

                        await con.OpenAsync();

                        using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                        {
                            // First result set: Data
                            while (await rdr.ReadAsync())
                            {
                                response.Data.Add(new AdvancePayViewModel
                                {
                                    AdvancePayId = rdr["AdvancePayId"]?.ToString() ?? "",
                                    EmployeeID = rdr["EmployeeID"]?.ToString() ?? "",
                                    LoanID = rdr["LoanID"]?.ToString() ?? "",
                                    NoOfPaymentInstallment = rdr["NoOfPaymentInstallment"]?.ToString() ?? "",
                                    PayHeadNameId = rdr["PayHeadNameId"]?.ToString() ?? "",
                                    AdvancePayCode = SafeConvertToInt32(rdr["AdvancePayCode"]),
                                    FullName = rdr["FullName"]?.ToString() ?? "",
                                    JoiningDate = rdr["JoiningDate"] != DBNull.Value? ((DateTime)rdr["JoiningDate"]).ToString("dd/MM/yyyy") : "",
                                    LoanDate = rdr["LoanDate"] != DBNull.Value? ((DateTime)rdr["LoanDate"]).ToString("dd/MM/yyyy") : "",
                                    CreateDate = rdr["LDate"] != DBNull.Value? ((DateTime)rdr["LDate"]).ToString("dd/MM/yyyy") : "",
                                    ModifyDate = rdr["ModifyDate"] != DBNull.Value? ((DateTime)rdr["ModifyDate"]).ToString("dd/MM/yyyy") : "",
                                    LoanTypeId = rdr["LoanTypeId"]?.ToString() ?? "",
                                    LoanTypeName = loanTypeRepo.All().Where(x=> x.LoanTypeId == rdr["LoanTypeId"].ToString()).Select(x=> x.LoanType).FirstOrDefault() ?? "",
                                    LoanStartDate = rdr["StartDate"] != DBNull.Value? ((DateTime)rdr["StartDate"]).ToString("dd/MM/yyyy"):"",
                                    LoanEndDate = rdr["EndDate"] != DBNull.Value ? ((DateTime)rdr["EndDate"]).ToString("dd/MM/yyyy") : "",
                                    DepartmentName = rdr["DepartmentName"]?.ToString() ?? "",
                                    AdjustmentType = rdr["AdjustmentType"]?.ToString() ?? "",
                                    AdvanceAdjustStatus = rdr["AdvanceAdjustStatus"]?.ToString() ?? "",
                                    DesignationName = rdr["DesignationName"]?.ToString() ?? "",
                                    AdvanceAmount = SafeConvertToDecimal(rdr["AdvanceAmount"]),
                                    MonthlyDeduction = rdr["MonthlyDeduction"] == DBNull.Value
                                        ? (decimal?)null
                                        : SafeConvertToDecimal(rdr["MonthlyDeduction"]),
                                    SalaryMonth = rdr["SalaryMonth"]?.ToString() ?? "",
                                    SalaryYear = rdr["SalaryYear"]?.ToString() ?? "",
                                   
                                    Remarks = rdr["Remarks"]?.ToString() ?? "",
                                });
                            }

                            // Second result set: Total count
                            if (await rdr.NextResultAsync())
                            {
                                if (await rdr.ReadAsync())
                                {
                                    response.TotalRecords = SafeConvertToInt32(rdr["TotalRecords"]);
                                    response.FilteredRecords = SafeConvertToInt32(rdr["FilteredRecords"]);
                                }
                            }
                        }


                    }
                }

                if (!string.IsNullOrWhiteSpace(request.sortColumn))
                {
                    response.Data = request.sortColumn.ToLower() switch
                    {
                        "advancepayid" => request.sortDirection.ToLower() == "asc" ?
                            response.Data.OrderBy(x => x.AdvancePayId).ToList() :
                            response.Data.OrderByDescending(x => x.AdvancePayId).ToList(),
                        "employeeid" => request.sortDirection.ToLower() == "asc" ?
                            response.Data.OrderBy(x => x.EmployeeID).ToList() :
                            response.Data.OrderByDescending(x => x.EmployeeID).ToList(),
                        "fullname" => request.sortDirection.ToLower() == "asc" ?
                            response.Data.OrderBy(x => x.FullName).ToList() :
                            response.Data.OrderByDescending(x => x.FullName).ToList(),
                        "designationname" => request.sortDirection.ToLower() == "asc" ?
                            response.Data.OrderBy(x => x.DesignationName).ToList() :
                            response.Data.OrderByDescending(x => x.DesignationName).ToList(),
                        "loanid" => request.sortDirection.ToLower() == "asc" ?
                            response.Data.OrderBy(x => x.LoanID).ToList() :
                            response.Data.OrderByDescending(x => x.LoanID).ToList(),
                        "advanceamount" => request.sortDirection.ToLower() == "asc" ?
                            response.Data.OrderBy(x => x.AdvanceAmount).ToList() :
                            response.Data.OrderByDescending(x => x.AdvanceAmount).ToList(),
                        "noofpaymentinstallment" => request.sortDirection.ToLower() == "asc" ?
                            response.Data.OrderBy(x => x.NoOfPaymentInstallment).ToList() :
                            response.Data.OrderByDescending(x => x.NoOfPaymentInstallment).ToList(),
                        "monthlydeduction" => request.sortDirection.ToLower() == "asc" ?
                            response.Data.OrderBy(x => x.MonthlyDeduction).ToList() :
                            response.Data.OrderByDescending(x => x.MonthlyDeduction).ToList(),
                        "salarymonth" => request.sortDirection.ToLower() == "asc" ?
                            response.Data.OrderBy(x => x.SalaryMonth).ToList() :
                            response.Data.OrderByDescending(x => x.SalaryMonth).ToList(),
                        "salaryyear" => request.sortDirection.ToLower() == "asc" ?
                            response.Data.OrderBy(x => x.SalaryYear).ToList() :
                            response.Data.OrderByDescending(x => x.SalaryYear).ToList(),
                        _ => response.Data.OrderBy(x => x.LoanID).ToList()
                    };
                }
            }
            catch (Exception ex)
            {
                response.Data = new List<AdvancePayViewModel>();
                response.TotalRecords = 0;
                response.FilteredRecords = 0;
            }

            return response;
        }

        //delete
        public async Task<(bool isSuccess, string message)> DeleteAdvancePayAsync(List<decimal> ids, DeleteHistoryViewModel dModel)
        {
            await advancePayRepo.BeginTransactionAsync();
            try
            {
                if (ids == null || ids.Count == 0)
                    return (false, DeleteFailed);

                var itemsToDelete = new List<HrmPayAdvancePay>();

                foreach (decimal id in ids)
                {
                    var item = await advancePayRepo.GetByIdAsync(id);
                    if (item != null)
                    {
                        itemsToDelete.Add(item);
                    }
                }

                if (itemsToDelete.Count == 0)
                    return (false, DeleteFailed);

                await advancePayRepo.DeleteRangeAsync(itemsToDelete);

                dModel.tableName = advancePayRepo.GetTableName();
                await deleteHistoryService.LogDeletedRecordsAsync(itemsToDelete, dModel);

                await advancePayRepo.CommitTransactionAsync();

                return (true, DeleteSuccess);
            }
            catch (Exception)
            {
                await advancePayRepo.RollbackTransactionAsync();
                throw;
                
            }
        }

    }

}
