using Dapper;
using GCTL.Core.Data;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.HRLettersReportViewModel;
using GCTL.Core.ViewModels.HRM_NOCEntry;
using GCTL.Data.Models;
using GCTL.Service.DeleteHistories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HRM_NOCEntry
{
    public class HRM_NOCEntryService: AppService<HrmNocinfo>, IHRM_NOCEntryService
    {
        private readonly string _connectionString;
        private readonly IRepository<HrmNocinfo> nocRepo;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly IRepository<HrmEmployee> empRepo;
        private readonly IDeleteHistoryService deleteHistoryService;

        public HRM_NOCEntryService(
            IConfiguration configuration,
            IRepository<HrmNocinfo> nocRepo,
            IRepository<CoreAccessCode> accessCodeRepository,
            IRepository<HrmEmployee> empRepo,
            IDeleteHistoryService deleteHistoryService
            ):base(nocRepo)
        {
            _connectionString = configuration.GetConnectionString("ApplicationDbConnection");
            this.nocRepo = nocRepo;
            this.accessCodeRepository = accessCodeRepository;
            this.empRepo = empRepo;
            this.deleteHistoryService = deleteHistoryService;
        }


        #region Permission all type

        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AsNoTracking().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "NOC Entry" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AsNoTracking().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "NOC Entry" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AsNoTracking().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "NOC Entry" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AsNoTracking().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "NOC Entry" && x.CheckDelete);
        }

        #endregion



        // ── Employee Details ─────────────────────────────────────────────
        public async Task<FullEmployeeDetailsGetByIdNocViewModel> GetByEmployeeCodeAsync(string employeeCode)
        {
            using var conn = new SqlConnection(_connectionString);
            return await conn.QueryFirstOrDefaultAsync<FullEmployeeDetailsGetByIdNocViewModel>(
                "FullEmployeeDetailsGetByid",
                new { EmployeeCode = employeeCode },
                commandType: CommandType.StoredProcedure
            );
        }

        // ── Generate New NOC ID ──────────────────────────────────────────
        // Format: 00000001, 00000002 …  (8-digit zero-padded)
        public async Task<string> GenerateNewNocIdAsync()
        {
            const string sql = @"
                SELECT ISNULL(MAX(CAST(NOCID AS BIGINT)), 0) + 1
                FROM   dbo.HRM_NOCInfo";

            using var conn = new SqlConnection(_connectionString);
            var next = await conn.ExecuteScalarAsync<long>(sql);
            return next.ToString("D8");           // e.g. "00000005"
        }

        // ── Get By AutoId ────────────────────────────────────────────────
        public async Task<HRM_NOCEntrySetupViewModel> GetNocByAutoIdAsync(long autoId)
        {
            const string sql = @"
                SELECT AutoId, NOCID AS NocId, NOCTypeId, EmployeeID, CompanyCode,
                       PlaceofVisit, FromDate, ToDate,
                       UniversityName, CourseName,
                       Remarks, LDate, ModifyDate
                FROM   dbo.HRM_NOCInfo
                WHERE  AutoId = @AutoId";

            using var conn = new SqlConnection(_connectionString);
            return await conn.QueryFirstOrDefaultAsync<HRM_NOCEntrySetupViewModel>(sql, new { AutoId = autoId });
        }

        // ── Save (Insert) ────────────────────────────────────────────────
        public async Task<NocOperationResult> SaveNocAsync(HRM_NOCEntrySetupViewModel model, string companyCode)
        {
            try
            {
                model.NOCID = await GenerateNewNocIdAsync();
                model.CompanyCode = companyCode ?? "";

                // NULL → EMPTY
                model.NOCTypeId = model.NOCTypeId ?? "";
                model.EmployeeID = model.EmployeeID ?? "";
                model.PlaceofVisit = model.PlaceofVisit ?? "";
                model.UniversityName = model.UniversityName ?? "";
                model.CourseName = model.CourseName ?? "";
                model.Remarks = model.Remarks ?? "";
                model.UserEmployeeID = model.UserInfoEmployeeId ?? "";
                const string sql = @"
            INSERT INTO dbo.HRM_NOCInfo
                   (NOCID, NOCTypeId, EmployeeID, CompanyCode,
                    PlaceofVisit, FromDate, ToDate,
                    UniversityName, CourseName, LMAC, UserEmployeeID,
                    Remarks, LUser, LDate, LIP)
            VALUES (@NOCID, @NOCTypeId, @EmployeeID, @CompanyCode,
                    @PlaceofVisit, @FromDate, @ToDate,
                    @UniversityName, @CourseName, @LMAC, @UserEmployeeID,
                    @Remarks, @LUser, @LDate, @LIP);

            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

                using var conn = new SqlConnection(_connectionString);
                var autoId = await conn.ExecuteScalarAsync<long>(sql, model);

                return new NocOperationResult
                {
                    Success = true,
                    AutoId = autoId,
                    NocId = model.NOCID,
                    LDate = model.Ldate
                };
            }
            catch (Exception ex)
            {
                return new NocOperationResult
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        // ── Update ───────────────────────────────────────────────────────
        public async Task<NocOperationResult> UpdateNocAsync(HRM_NOCEntrySetupViewModel model)
        {
            try
            {
                // NULL → EMPTY
                model.NOCTypeId = model.NOCTypeId ?? "";
                model.EmployeeID = model.EmployeeID ?? "";
                model.PlaceofVisit = model.PlaceofVisit ?? "";
                model.UniversityName = model.UniversityName ?? "";
                model.CourseName = model.CourseName ?? "";
                model.Remarks = model.Remarks ?? "";
                model.UserEmployeeID = model.UserInfoEmployeeId ?? "";
                model.Lmac = model.Lmac ?? "";
                model.ModifyDate = model.ModifyDate ?? DateTime.Now;

                const string sql = @"
        UPDATE dbo.HRM_NOCInfo SET
            NOCTypeId      = @NOCTypeId,
            EmployeeID     = @EmployeeID,
            CompanyCode    = @CompanyCode,
            PlaceofVisit   = @PlaceofVisit,
            FromDate       = @FromDate,
            ToDate         = @ToDate,
            UniversityName = @UniversityName,
            CourseName     = @CourseName,
            UserEmployeeID = @UserEmployeeID,
            LMAC           = @LMAC,
            Remarks        = @Remarks,
            LUser          = @LUser,
            ModifyDate     = @ModifyDate,
            LIP            = @LIP
        WHERE AutoId = @AutoId";

                using var conn = new SqlConnection(_connectionString);
                var rows = await conn.ExecuteAsync(sql, model);

                if (rows == 0)
                {
                    return new NocOperationResult
                    {
                        Success = false,
                        Message = "Record not found."
                    };
                }

                return new NocOperationResult
                {
                    Success = true,
                    ModifyDate = model.ModifyDate
                };
            }
            catch (Exception ex)
            {
                return new NocOperationResult
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        // ── Delete ───────────────────────────────────────────────────────
        public async Task<NocOperationResult> DeleteNocAsync(List<decimal> autoIds, DeleteHistoryViewModel dModel)
        {
            try
            {
                var dList = nocRepo.All().Where(x => autoIds.Contains(x.AutoId)).ToList();

                const string sql = "DELETE FROM dbo.HRM_NOCInfo WHERE AutoId IN @AutoIds";

                using var conn = new SqlConnection(_connectionString);
                var rows = await conn.ExecuteAsync(sql, new { AutoIds = autoIds });


                if (rows == 0)
                    return new NocOperationResult { Success = false, Message = "No records found to delete." };

                dModel.tableName = nocRepo.GetTableName();
                await deleteHistoryService.LogDeletedRecordsAsync(dList, dModel);

                return new NocOperationResult { Success = true };
            }
            catch (Exception ex)
            {
                return new NocOperationResult { Success = false, Message = ex.Message };
            }
        }

        public async Task<List<NocListItemDto>> GetListAsync(string nocType)
        {
            var query = from noc in nocRepo.All()
                        join emp in empRepo.All()
                            on noc.EmployeeId equals emp.EmployeeId into empGroup
                        from emp in empGroup.DefaultIfEmpty()   // LEFT JOIN
                        where noc.NoctypeId == nocType
                        select new NocListItemDto
                        {
                            AutoId = noc.AutoId,
                            NocId = noc.Nocid,
                            EmployeeID = noc.EmployeeId,
                            EmployeeName = emp != null
                                            ? (emp.FirstName + " " + emp.LastName).Trim()
                                            : string.Empty,
                            PlaceofVisit = noc.PlaceofVisit,
                            FromDate = noc.FromDate,
                            ToDate = noc.ToDate,
                            UniversityName = noc.UniversityName,
                            CourseName = noc.CourseName,
                            Remarks = noc.Remarks
                        };

            return await query.ToListAsync();
        }
       
    }
}
