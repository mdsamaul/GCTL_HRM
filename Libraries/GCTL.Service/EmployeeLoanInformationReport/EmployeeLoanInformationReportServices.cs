using GCTL.Core.Data;
using GCTL.Core.ViewModels.EmployeeLoanInformationReport;
using GCTL.Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace GCTL.Service.EmployeeLoanInformationReport
{
    public class EmployeeLoanInformationReportServices : AppService<HrmPayrollLoan>, IEmployeeLoanInformationReportServices
    {
        private readonly IRepository<HrmPayrollLoan> ploanRepo;
        private readonly IRepository<HrmPayrollLoan> payrollLoanRepo;
        private readonly IRepository<CoreCompany> comRepo;
        private readonly IRepository<HrmEmployee> empRepo;
        private readonly IRepository<HrmEmployeeOfficialInfo> empOffRepo;
        private readonly IRepository<HrmPayrollPaymentReceive> paymentRepo;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly IConfiguration configuration;

        public EmployeeLoanInformationReportServices(
            IRepository<HrmPayrollLoan> ploanRepo,
            IRepository<HrmPayrollLoan> payrollLoanRepo,
            IRepository<CoreCompany> comRepo,
            IRepository<HrmEmployee> empRepo,
            IRepository<HrmEmployeeOfficialInfo> empOffRepo,
            IRepository<HrmPayrollPaymentReceive> paymentRepo,
            IRepository<CoreAccessCode> accessCodeRepository,
            IConfiguration configuration
            ) : base(ploanRepo)
        {
            this.ploanRepo = ploanRepo;
            this.payrollLoanRepo = payrollLoanRepo;
            this.comRepo = comRepo;
            this.empRepo = empRepo;
            this.empOffRepo = empOffRepo;
            this.paymentRepo = paymentRepo;
            this.accessCodeRepository = accessCodeRepository;
            this.configuration = configuration;
        }

        public async Task<bool> PagePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Loan" && x.TitleCheck);

        }

        public async Task<EmployeeLoanReportResponseVM> GetLoanDetailsAsync(LoanFilterVM filter)
        {
            try
            {
                var response = new EmployeeLoanReportResponseVM
                {
                    LoanReports = new List<EmployeeLoanInformationReportVM>(),
                    Companies = new List<CompanyBasicInfoVM>(),
                    Employees = new List<EmployeeBasicInfoVM>(),
                    LoanIDs = new List<LoanBasicInfoVm>(),
                    LoanTypes = new List<LoanTypeInfoVm>()
                };

                using var conn = new SqlConnection(configuration.GetConnectionString("ApplicationDbConnection"));
                await conn.OpenAsync();

                using var cmd = new SqlCommand("EmployeeLoanInformationReport", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@CompanyID", (object)filter.CompanyID ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EmployeeID", (object)filter.EmployeeID ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@LoanID", (object)filter.LoanID ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@LoanTypeID", (object)filter.LoanTypeID ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DateFrom", (object)filter.DateFrom ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DateTo", (object)filter.DateTo ?? DBNull.Value);
                using var reader = await cmd.ExecuteReaderAsync();
                var loanDict = new Dictionary<string, EmployeeLoanInformationReportVM>();
                var companySet = new Dictionary<string, CompanyBasicInfoVM>();
                var employeeSet = new HashSet<string>();
                var loanIdSet = new Dictionary<string, LoanBasicInfoVm>();
                var loanTypeSet = new Dictionary<string, LoanTypeInfoVm>();

                while (await reader.ReadAsync())
                {
                    string loanId = reader["LoanID"].ToString();
                    string empId = reader["EmployeeID"].ToString();
                    string fullName = reader["FullName"].ToString();
                    string CompanyCode = reader["CompanyCode"].ToString();
                    string companyName = reader["CompanyName"].ToString();
                    string loanTypeId = reader["LoanTypeID"]?.ToString()?.Trim();
                    string loanTypeName = reader["LoanType"]?.ToString()?.Trim();

                    if (!companySet.ContainsKey(companyName))
                    {
                        companySet.Add(companyName, new CompanyBasicInfoVM
                        {
                            CompanyCode = CompanyCode,
                            CompanyName = companyName
                        });
                    }

                    if (!employeeSet.Contains(empId))
                    {
                        employeeSet.Add(empId);
                        response.Employees.Add(new EmployeeBasicInfoVM
                        {
                            EmployeeID = empId,
                            FullName = fullName,
                            DepartmentName = reader["DepartmentName"].ToString(),
                            DesignationName = reader["DesignationName"].ToString()
                        });
                    }


                    if (!string.IsNullOrEmpty(loanTypeId) && !string.IsNullOrEmpty(loanTypeName))
                    {
                        if (!loanTypeSet.ContainsKey(loanTypeId))
                        {
                            var loanTypeVm = new LoanTypeInfoVm
                            {
                                LoanTypeId = loanTypeId,
                                LoanType = loanTypeName
                            };

                            loanTypeSet.Add(loanTypeId, loanTypeVm);
                            response.LoanTypes.Add(loanTypeVm);
                        }
                    }


                    if (!loanIdSet.ContainsKey(loanId))
                    {
                        loanIdSet.Add(loanId, new LoanBasicInfoVm
                        {
                            LoanDate = Convert.ToDateTime(reader["LoanDate"]).ToString("dd/MM/yyy"),
                            LoanType = reader["LoanType"].ToString(),
                            LoanAmount = Convert.ToDecimal(reader["LoanAmount"]),
                            LoanIDs = reader["LoanID"].ToString(),
                            InstStartEndDate = Convert.ToDateTime(reader["StartDate"]).ToString("dd/MM/yyyy")
                       + " - " +
                       Convert.ToDateTime(reader["EndDate"]).ToString("dd/MM/yyyy"),
                            NoOfInstallment = reader["NoOfInstallment"].ToString(),
                        });
                    }
                    if (!loanDict.ContainsKey(loanId))
                    {
                        var loanVm = new EmployeeLoanInformationReportVM
                        {
                            LoanID = loanId,
                            EmployeeID = empId,
                            FullName = fullName,
                            DepartmentName = reader["DepartmentName"].ToString(),
                            DesignationName = reader["DesignationName"].ToString(),
                            Reason = reader["Reason"]?.ToString() ?? "",
                            Remarks = reader["Remarks"]?.ToString() ?? "",
                            TotalLoans = Convert.ToDecimal(reader["TotalLoans"]),
                            LoanAmount = Convert.ToDecimal(reader["LoanAmount"]),
                            //PaymentMode = reader["LoanRepaymentMethod"].ToString(),
                            CompanyName = companyName,
                            StartDate = Convert.ToDateTime(reader["StartDate"]?.ToString() ?? ""),
                            EndDate = Convert.ToDateTime(reader["EndDate"]?.ToString() ?? ""),
                            InstallmentDetails = reader["Installment Details"]?.ToString(),
                            LoanRepaymentMethod = reader["LoanRepaymentMethod"].ToString() ?? "",
                            MonthlyDeduction = Convert.ToDecimal(reader["MonthlyDeduction"]),
                            Installments = new List<InstallmentVM>()
                        };
                        loanDict.Add(loanId, loanVm);
                    }
                    var installment = new InstallmentVM
                    {
                        InstallmentNo = Convert.ToInt32(reader["InstallmentNo"]),
                        InstallmentDate = reader.IsDBNull(reader.GetOrdinal("InstallmentDate")) ? "" : reader.GetString(reader.GetOrdinal("InstallmentDate")).ToString(),
                        PaymentMode = reader["PaymentMode"].ToString() ?? "",
                        Deposit = Convert.ToDecimal(reader["Deposit"]),
                        OutstandingBalance = Convert.ToDecimal(reader["Outstanding Balance"])
                    };
                    loanDict[loanId].Installments.Add(installment);

                }

                response.LoanReports = loanDict.Values.ToList();
                response.Companies = companySet.Values.ToList();
                response.LoanIDs = loanIdSet.Values.ToList();
                response.LoanTypes = loanTypeSet.Values.ToList();


                // --- Fallback master data load if lists are empty ---
                if (response.Companies == null || response.Companies.Count == 0)
                {
                    using var cmdCompany = new SqlCommand("SELECT DISTINCT CompanyCode, CompanyName FROM Core_Company", conn);
                    using var readerCompany = await cmdCompany.ExecuteReaderAsync();
                    var companies = new List<CompanyBasicInfoVM>();
                    while (await readerCompany.ReadAsync())
                    {
                        companies.Add(new CompanyBasicInfoVM
                        {
                            CompanyCode = readerCompany["CompanyCode"].ToString(),
                            CompanyName = readerCompany["CompanyName"].ToString()
                        });
                    }
                    response.Companies = companies;
                }

                if (response.Employees == null || response.Employees.Count == 0)
                {
                    using var cmdEmployee = new SqlCommand("SELECT DISTINCT EmployeeID, FirstName + ' ' + LastName AS FullName FROM HRM_Employee", conn);
                    using var readerEmployee = await cmdEmployee.ExecuteReaderAsync();
                    var employees = new List<EmployeeBasicInfoVM>();
                    while (await readerEmployee.ReadAsync())
                    {
                        employees.Add(new EmployeeBasicInfoVM
                        {
                            EmployeeID = readerEmployee["EmployeeID"].ToString(),
                            FullName = readerEmployee["FullName"].ToString()
                        });
                    }
                    response.Employees = employees;
                }

                if (response.LoanTypes == null || response.LoanTypes.Count == 0)
                {
                    using var cmdLoanType = new SqlCommand("SELECT DISTINCT LoanTypeID, LoanType FROM HRM_PAY_LoanTypeEntry", conn);
                    using var readerLoanType = await cmdLoanType.ExecuteReaderAsync();
                    var loanTypes = new List<LoanTypeInfoVm>();
                    while (await readerLoanType.ReadAsync())
                    {
                        loanTypes.Add(new LoanTypeInfoVm
                        {
                            LoanTypeId = readerLoanType["LoanTypeID"].ToString(),
                            LoanType = readerLoanType["LoanType"].ToString()
                        });
                    }
                    response.LoanTypes = loanTypes;
                }
                return response;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //public async Task<EmployeeLoanReportResponseVM> GetLoanDetailsAsync(LoanFilterVM filter)
        //{
        //    try
        //    {
        //        var response = new EmployeeLoanReportResponseVM
        //        {
        //            LoanReports = new List<EmployeeLoanInformationReportVM>(),
        //            Companies = new List<CompanyBasicInfoVM>(),
        //            Employees = new List<EmployeeBasicInfoVM>(),
        //            LoanIDs = new List<LoanBasicInfoVm>(),
        //            LoanTypes = new List<LoanTypeInfoVm>()
        //        };

        //        using var conn = new SqlConnection(configuration.GetConnectionString("ApplicationDbConnection"));
        //        await conn.OpenAsync();

        //        using var cmd = new SqlCommand("EmployeeLoanInformationReport", conn)
        //        {
        //            CommandType = CommandType.StoredProcedure
        //        };

        //        cmd.Parameters.AddWithValue("@CompanyID", (object)filter.CompanyID ?? DBNull.Value);
        //        cmd.Parameters.AddWithValue("@EmployeeID", (object)filter.EmployeeID ?? DBNull.Value);
        //        cmd.Parameters.AddWithValue("@LoanID", (object)filter.LoanID ?? DBNull.Value);
        //        cmd.Parameters.AddWithValue("@LoanTypeID", (object)filter.LoanTypeID ?? DBNull.Value);
        //        cmd.Parameters.AddWithValue("@DateFrom", (object)filter.DateFrom ?? DBNull.Value);
        //        cmd.Parameters.AddWithValue("@DateTo", (object)filter.DateTo ?? DBNull.Value);

        //        using var reader = await cmd.ExecuteReaderAsync();

        //        // --- First ResultSet: Loan Report ---
        //        var loanDict = new Dictionary<string, EmployeeLoanInformationReportVM>();
        //        var companySet = new Dictionary<string, CompanyBasicInfoVM>();
        //        var employeeSet = new HashSet<string>();
        //        var loanIdSet = new Dictionary<string, LoanBasicInfoVm>();
        //        var loanTypeSet = new Dictionary<string, LoanTypeInfoVm>();

        //        while (await reader.ReadAsync())
        //        {
        //            string loanId = reader["LoanID"].ToString();
        //            string empId = reader["EmployeeID"].ToString();
        //            string fullName = reader["FullName"].ToString();
        //            string companyCode = reader["CompanyCode"].ToString();
        //            string companyName = reader["CompanyName"].ToString();
        //            string loanTypeId = reader["LoanTypeID"]?.ToString()?.Trim();
        //            string loanTypeName = reader["LoanType"]?.ToString()?.Trim();

        //            if (!companySet.ContainsKey(companyCode))
        //            {
        //                companySet.Add(companyCode, new CompanyBasicInfoVM
        //                {
        //                    CompanyCode = companyCode,
        //                    CompanyName = companyName
        //                });
        //            }

        //            if (!employeeSet.Contains(empId))
        //            {
        //                employeeSet.Add(empId);
        //                response.Employees.Add(new EmployeeBasicInfoVM
        //                {
        //                    EmployeeID = empId,
        //                    FullName = fullName,
        //                    DepartmentName = reader["DepartmentName"].ToString(),
        //                    DesignationName = reader["DesignationName"].ToString()
        //                });
        //            }

        //            if (!string.IsNullOrEmpty(loanTypeId) && !string.IsNullOrEmpty(loanTypeName))
        //            {
        //                if (!loanTypeSet.ContainsKey(loanTypeId))
        //                {
        //                    var loanTypeVm = new LoanTypeInfoVm
        //                    {
        //                        LoanTypeId = loanTypeId,
        //                        LoanType = loanTypeName
        //                    };
        //                    loanTypeSet.Add(loanTypeId, loanTypeVm);
        //                    response.LoanTypes.Add(loanTypeVm);
        //                }
        //            }

        //            if (!loanIdSet.ContainsKey(loanId))
        //            {
        //                loanIdSet.Add(loanId, new LoanBasicInfoVm
        //                {
        //                    LoanDate = Convert.ToDateTime(reader["LoanDate"]).ToString("dd/MM/yyyy"),
        //                    LoanType = reader["LoanType"].ToString(),
        //                    LoanAmount = Convert.ToDecimal(reader["LoanAmount"]),
        //                    LoanIDs = loanId,
        //                    InstStartEndDate = Convert.ToDateTime(reader["StartDate"]).ToString("dd/MM/yyyy") +
        //                                       " - " +
        //                                       Convert.ToDateTime(reader["EndDate"]).ToString("dd/MM/yyyy"),
        //                    NoOfInstallment = reader["NoOfInstallment"].ToString(),
        //                });
        //            }

        //            if (!loanDict.ContainsKey(loanId))
        //            {
        //                var loanVm = new EmployeeLoanInformationReportVM
        //                {
        //                    LoanID = loanId,
        //                    EmployeeID = empId,
        //                    FullName = fullName,
        //                    DepartmentName = reader["DepartmentName"].ToString(),
        //                    DesignationName = reader["DesignationName"].ToString(),
        //                    Reason = reader["Reason"]?.ToString() ?? "",
        //                    Remarks = reader["Remarks"]?.ToString() ?? "",
        //                    TotalLoans = Convert.ToDecimal(reader["TotalLoans"]),
        //                    LoanAmount = Convert.ToDecimal(reader["LoanAmount"]),
        //                    CompanyName = companyName,
        //                    StartDate = Convert.ToDateTime(reader["StartDate"]),
        //                    EndDate = Convert.ToDateTime(reader["EndDate"]),
        //                    InstallmentDetails = reader["Installment Details"]?.ToString(),
        //                    LoanRepaymentMethod = reader["LoanRepaymentMethod"].ToString() ?? "",
        //                    MonthlyDeduction = Convert.ToDecimal(reader["MonthlyDeduction"]),
        //                    Installments = new List<InstallmentVM>()
        //                };
        //                loanDict.Add(loanId, loanVm);
        //            }

        //            var installment = new InstallmentVM
        //            {
        //                InstallmentNo = Convert.ToInt32(reader["InstallmentNo"]),
        //                InstallmentDate = reader.IsDBNull(reader.GetOrdinal("InstallmentDate")) ? "" : reader["InstallmentDate"].ToString(),
        //                PaymentMode = reader["PaymentMode"].ToString() ?? "",
        //                Deposit = Convert.ToDecimal(reader["Deposit"]),
        //                OutstandingBalance = Convert.ToDecimal(reader["OutstandingBalance"])
        //            };
        //            loanDict[loanId].Installments.Add(installment);
        //        }

        //        response.LoanReports = loanDict.Values.ToList();
        //        response.Companies = companySet.Values.ToList();
        //        response.LoanIDs = loanIdSet.Values.ToList();
        //        response.LoanTypes = loanTypeSet.Values.ToList();

        //        // --- Next ResultSets: Master Dropdowns ---
        //        if (await reader.NextResultAsync())
        //        {
        //            while (await reader.ReadAsync())
        //            {
        //                response.Companies.Add(new CompanyBasicInfoVM
        //                {
        //                    CompanyCode = reader["CompanyCode"].ToString(),
        //                    CompanyName = reader["CompanyName"].ToString()
        //                });
        //            }
        //        }

        //        if (await reader.NextResultAsync())
        //        {
        //            while (await reader.ReadAsync())
        //            {
        //                response.Employees.Add(new EmployeeBasicInfoVM
        //                {
        //                    EmployeeID = reader["EmployeeID"].ToString(),
        //                    FullName = reader["FullName"].ToString()
        //                });
        //            }
        //        }

        //        if (await reader.NextResultAsync())
        //        {
        //            while (await reader.ReadAsync())
        //            {
        //                response.LoanTypes.Add(new LoanTypeInfoVm
        //                {
        //                    LoanTypeId = reader["LoanTypeID"].ToString(),
        //                    LoanType = reader["LoanType"].ToString()
        //                });
        //            }
        //        }

        //        return response;
        //    }
        //    catch (Exception e)
        //    {
        //        throw;
        //    }
        //}
    }
}
