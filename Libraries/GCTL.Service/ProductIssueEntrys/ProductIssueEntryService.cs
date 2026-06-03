using GCTL.Core.Data;
using GCTL.Core.ViewModels.ProductIssueEntry;
using GCTL.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GCTL.Service.ProductIssueEntrys
{
    public class ProductIssueEntryService : AppService<InvProductIssueInformation>, IProductIssueEntryService
    {
        private readonly IRepository<InvProductIssueInformation> ProductIssueInformation;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly IRepository<SalesSupplier> salesSupRepo;
        private readonly IRepository<HrmItemMasterInformation> productRepo;
        private readonly IRepository<HrmModel> modelRepo;
        private readonly IRepository<HrmBrand> brandRepo;
        private readonly IRepository<RmgPurchaseOrderReceiveDetails> purchaseOrderReceiveDetailsRepo;
        private readonly IRepository<InvProductIssueInformationDetails> productIssueInformationDetailRepo;
        private readonly IRepository<InvProductIssueInformationDetailsTemp> productIssueInformationDetailTempRepo;
        private readonly IRepository<HrmDefDepartment> depRepo;
        private readonly IRepository<HrmEmployee> empRepo;
        private readonly IRepository<CoreCompany> comRepo;
        private readonly IRepository<HrmSize> sizeRepo;
        private readonly IRepository<HmsLtrvPeriod> periodRepo;
        private readonly IRepository<RmgProdDefUnitType> unitRepo;
        private readonly IRepository<HrmDefFloor> floorRepo;

        public ProductIssueEntryService(
        IRepository<InvProductIssueInformation> ProductIssueInformation,
        IRepository<CoreAccessCode> accessCodeRepository,
        IRepository<SalesSupplier> salesSupRepo,
        IRepository<HrmItemMasterInformation> productRepo,
        IRepository<HrmModel> modelRepo,
        IRepository<HrmBrand> brandRepo,
        IRepository<RmgPurchaseOrderReceiveDetails> PurchaseOrderReceiveDetailsRepo,
        IRepository<InvProductIssueInformationDetails> ProductIssueInformationDetailRepo,
        IRepository<InvProductIssueInformationDetailsTemp> ProductIssueInformationDetailTempRepo,
        IRepository<HrmDefDepartment> depRepo,
        IRepository<HrmEmployee> empRepo,
        IRepository<CoreCompany> comRepo,
        IRepository<HrmSize> sizeRepo,
        IRepository<HmsLtrvPeriod> periodRepo,
        IRepository<RmgProdDefUnitType> unitRepo,
        IRepository<HrmDefFloor> floorRepo
        ) : base(ProductIssueInformation)
        {
            this.ProductIssueInformation = ProductIssueInformation;
            this.accessCodeRepository = accessCodeRepository;
            this.salesSupRepo = salesSupRepo;
            this.productRepo = productRepo;
            this.modelRepo = modelRepo;
            this.brandRepo = brandRepo;
            this.purchaseOrderReceiveDetailsRepo = PurchaseOrderReceiveDetailsRepo;
            this.productIssueInformationDetailRepo = ProductIssueInformationDetailRepo;
            this.productIssueInformationDetailTempRepo = ProductIssueInformationDetailTempRepo;
            this.depRepo = depRepo;
            this.empRepo = empRepo;
            this.comRepo = comRepo;
            this.sizeRepo = sizeRepo;
            this.periodRepo = periodRepo;
            this.unitRepo = unitRepo;
            this.floorRepo = floorRepo;
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

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Product Issue" && x.TitleCheck);

        }

        public async Task<bool> SavePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Product Issue" && x.CheckAdd);

        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Product Issue" && x.CheckEdit);

        }

        public async Task<bool> DeletePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Product Issue" && x.CheckDelete);

        }

        #endregion


        public async Task<List<ProductIssueEntrySetupViewModel>> GetAllAsync()
        {
            try
            {
                return ProductIssueInformation.All().Select(c => new ProductIssueEntrySetupViewModel
                {
                    TC = c.Tc,
                    IssueNo = c.IssueNo,
                    IssueDate = c.IssueDate,
                    ShowIssueDate = c.IssueDate.HasValue ? c.IssueDate.Value.ToString("dd/MM/yyyy hh:mm:ss tt") : "",
                    DepartmentCode = depRepo.All().Where(x => c.DepartmentCode == x.DepartmentCode).Select(w => w.DepartmentName).FirstOrDefault(),
                    EmployeeID = empRepo.All().Where(w => w.EmployeeId == c.EmployeeId).Select(w => w.FirstName + " " + w.LastName).FirstOrDefault(),
                    IssuedBy = empRepo.All().Where(w => w.EmployeeId == c.IssuedBy).Select(w => w.FirstName + " " + w.LastName).FirstOrDefault(),
                    Remarks = c.Remarks,
                    Luser = c.Luser,
                    CompanyCode = comRepo.All().Where(x => x.CompanyCode == c.CompanyCode).Select(x => x.CompanyName).FirstOrDefault(),


                }).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<ProductIssueEntrySetupViewModel> GetByIdAsync(string id)
        {
            try
            {
                var entity = ProductIssueInformation.All().FirstOrDefault(x => x.IssueNo == id);
                if (entity == null) return null;

                // Caching all lookup data to avoid repeated DB calls
                var allProducts = productRepo.All().ToList();
                var allBrands = brandRepo.All().ToList();
                var allModels = modelRepo.All().ToList();
                var allSizes = sizeRepo.All().ToList();
                var allPeriods = periodRepo.All().ToList();
                var allUnits = unitRepo.All().ToList();

                var detailsList = ProductIssueInformation.All()
                    .Where(x => x.IssueNo == entity.IssueNo)
                    .ToList() // Important: materialize query first, then project
                    .Select(x =>
                    {


                        return new ProductIssueEntrySetupViewModel
                        {
                            TC = x.Tc,

                        };
                    }).ToList();


                return new ProductIssueEntrySetupViewModel
                {
                    TC = entity.Tc,

                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<(bool isSuccess, string message, object data)> CreateUpdateAsync(ProductIssueEntrySetupViewModel model, string companyCode)
        {
            try
            {
                var tempDetails = productIssueInformationDetailTempRepo.All()
                       .Where(x => x.IssueNo == model.IssueNo)
                       .ToList();

                if (model.TC == 0) // Create
                {

                    if (tempDetails == null || tempDetails.Count == 0)
                    {
                        return (false, CreateFailed, null);
                    }

                    var mEntity = new InvProductIssueInformation
                    {
                        IssueNo = model.IssueNo,
                        IssueDate = model.IssueDate,
                        DepartmentCode = model.DepartmentCode,
                        EmployeeId = model.EmployeeID,
                        IssuedBy = model.IssuedBy,
                        Remarks = model.Remarks,
                        Luser = model.Luser,
                        Ldate = model.Ldate,
                        Lip = model.Lip,
                        Lmac = model.Lmac,
                        CompanyCode = companyCode,
                        UserInfoEmployeeId = model.UserInfoEmployeeId,
                        FloorCode = model.FloorCode
                    };
                    await ProductIssueInformation.AddAsync(mEntity);

                    // Convert Temp data to actual Detail data
                    var convertedDetails = tempDetails.Select(temp => new InvProductIssueInformationDetails
                    {
                        Pidid = temp.Pidid,
                        IssueNo = temp.IssueNo,
                        ProductCode = temp.ProductCode,
                        BrandId = temp.BrandId,
                        ModelId = temp.ModelId,
                        SizeId = temp.SizeId,
                        UnitTypId = temp.UnitTypId,
                        StockQty = temp.StockQty,
                        IssueQty = temp.IssueQty,
                        FloorCode = temp.FloorCode,
                        Luser = temp.Luser,

                    }).ToList();

                    await productIssueInformationDetailRepo.AddRangeAsync(convertedDetails);
                    await productIssueInformationDetailTempRepo.DeleteRangeAsync(tempDetails);



                    return (true, CreateSuccess, mEntity);
                }
                else // Update
                {
                    var exData = await ProductIssueInformation.GetByIdAsync(model.TC);

                    if (exData == null)
                    {
                        return (false, UpdateFailed, null);
                    }

                    exData.IssueDate = model.IssueDate;
                    exData.DepartmentCode = model.DepartmentCode;
                    exData.EmployeeId = model.EmployeeID;
                    exData.IssuedBy = model.IssuedBy;
                    exData.Remarks = model.Remarks;
                    exData.Luser = model.Luser;
                    exData.Ldate = model.Ldate;
                    exData.Lip = model.Lip;
                    exData.Lmac = model.Lmac;
                    exData.CompanyCode = companyCode;
                    exData.UserInfoEmployeeId = model.UserInfoEmployeeId;
                    exData.FloorCode = model.FloorCode;
                    exData.ModifyDate = DateTime.Now;

                    ProductIssueInformation.Update(exData);

                    var detailsList = productIssueInformationDetailRepo.All().Where(x => x.IssueNo == model.IssueNo).ToList();
                    if (detailsList != null)
                    {
                        await productIssueInformationDetailRepo.DeleteRangeAsync(detailsList);
                    }
                    foreach (var temp in tempDetails)
                    {
                        var newDetail = new InvProductIssueInformationDetails
                        {
                            Pidid = temp.Pidid,
                            IssueNo = temp.IssueNo,
                            ProductCode = temp.ProductCode,
                            BrandId = temp.BrandId,
                            ModelId = temp.ModelId,
                            SizeId = temp.SizeId,
                            UnitTypId = temp.UnitTypId,
                            StockQty = temp.StockQty,
                            IssueQty = temp.IssueQty,
                            FloorCode = temp.FloorCode,
                            Luser = temp.Luser,
                        };
                        await productIssueInformationDetailRepo.AddAsync(newDetail);
                    }

                    await productIssueInformationDetailTempRepo.DeleteRangeAsync(tempDetails);
                    return (true, UpdateSuccess, exData);
                }
            }
            catch (Exception ex)
            {
                // Optional: Log the exception message here
                return (false, "An error occurred: " + ex.Message, null);
            }
        }



        public async Task<(bool isSuccess, string message, object data)> DeleteAsync(List<string> ids)
        {
            foreach (var id in ids)
            {
                try
                {

                    var entity = await ProductIssueInformation.GetByIdAsync(decimal.Parse(id));
                    var detailsEntity = productIssueInformationDetailRepo.All().Where(x => x.IssueNo == entity.IssueNo).ToList();
                    if (detailsEntity != null)
                    {
                        await productIssueInformationDetailRepo.DeleteRangeAsync(detailsEntity);
                    }
                    if (entity == null)
                    {
                        continue;
                    }

                    await ProductIssueInformation.DeleteAsync(entity);
                }
                catch (Exception)
                {
                    return (true, DeleteFailed, null);
                }
            }

            return (true, DeleteSuccess, null);
        }

        public async Task<string> AutoProdutIssueIdAsync()
        {
            var currentYearShort = DateTime.Now.Year.ToString();
            var prefix = $"PI_{currentYearShort}_";

            var productIssueList = (await ProductIssueInformation.GetAllAsync()).ToList();

            int newIdNumber = 1;

            if (productIssueList != null && productIssueList.Count > 0)
            {
                var lastId = productIssueList
                    .Select(x => x.IssueNo)
                    .Where(id => id != null && id.StartsWith(prefix))
                    .OrderByDescending(id => id)
                    .FirstOrDefault();


                if (!string.IsNullOrEmpty(lastId))
                {
                    var numericPart = lastId.Substring(prefix.Length);
                    if (int.TryParse(numericPart, out int lastNumber))
                    {
                        newIdNumber = lastNumber + 1;
                    }
                }
            }
            var exIssueData = productIssueInformationDetailTempRepo.All().ToList();
            if (exIssueData != null)
            {
                await productIssueInformationDetailTempRepo.DeleteRangeAsync(exIssueData);
            }
            return $"{prefix}{newIdNumber.ToString("D6")}";
        }

        public async Task<(bool isSuccess, string message, object data)> AlreadyExistAsync(string productIssueValue)
        {
            bool Exists = ProductIssueInformation.All().Any(x => x.DepartmentCode == productIssueValue);
            return (Exists, DataExists, null);
        }

        public async Task<(bool isSuccess, string message, object data)> PurchaseIssueAddmoreCreateEditDetailsAsync(ProductIssueInformationDetailViewModel model)
        {
            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.ProductCode) || string.IsNullOrWhiteSpace(model.IssueNo))
                {
                    return (false, CreateFailed, null);
                }

                if (productIssueInformationDetailRepo == null || productIssueInformationDetailTempRepo == null)
                {
                    return (false, "Required repository is null.", null);
                }

                if (model.TC == 0)
                {
                    // Create New                  
                    var lastRecord = productIssueInformationDetailRepo.All()
                                          .OrderByDescending(x => x.Tc)
                                          .FirstOrDefault();
                    int nextId = 1;
                    string nextDetailsId = lastRecord?.Pidid;
                    if (!string.IsNullOrWhiteSpace(nextDetailsId) && int.TryParse(nextDetailsId, out int dLastId))
                    {
                        nextId = dLastId + 1;
                    }
                    var lastTempRecord = productIssueInformationDetailTempRepo.All()
                                          .OrderByDescending(x => x.Tc)
                                          .FirstOrDefault();

                    string lastPid = lastTempRecord?.Pidid;

                    if (!string.IsNullOrWhiteSpace(lastPid) && int.TryParse(lastPid, out int lastId))
                    {
                        nextId = lastId + 1;
                    }

                    var entity = new InvProductIssueInformationDetailsTemp
                    {
                        Pidid = nextId.ToString("D3"),
                        IssueNo = model.IssueNo,
                        ProductCode = model.ProductCode,
                        BrandId = model.BrandID,
                        ModelId = model.ModelID,
                        SizeId = model.SizeID,
                        UnitTypId = model.UnitTypID,
                        StockQty = model.StockQty,
                        IssueQty = model.IssueQty,
                        FloorCode = model.FloorCode,
                        Luser = model.Luser
                    };

                    await productIssueInformationDetailTempRepo.AddAsync(entity);
                    return (true, CreateSuccess, entity);
                }
                else
                {
                    // Edit Existing
                    var exEntity = await productIssueInformationDetailTempRepo.GetByIdAsync(model.TC);
                    if (exEntity == null)
                    {
                        return (false, UpdateFailed, null);
                    }

                    exEntity.ProductCode = model.ProductCode;
                    exEntity.BrandId = model.BrandID;
                    exEntity.ModelId = model.ModelID;
                    exEntity.SizeId = model.SizeID;
                    exEntity.UnitTypId = model.UnitTypID;
                    exEntity.StockQty = model.StockQty;
                    exEntity.IssueQty = model.IssueQty;
                    exEntity.FloorCode = model.FloorCode;
                    exEntity.Luser = model.Luser;

                    await productIssueInformationDetailTempRepo.UpdateAsync(exEntity);
                    return (true, UpdateSuccess, exEntity);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Exception occurred: {ex.Message}", null);
            }
        }

        public Task<List<ProductIssueInformationDetailViewModel>> LoadTempDataAsync()
        {
            try
            {
                return productIssueInformationDetailTempRepo.All()
                    .Select(x => new ProductIssueInformationDetailViewModel
                    {
                        TC = x.Tc,
                        PIDID = x.Pidid,
                        IssueNo = x.IssueNo,
                        ProductCode = x.ProductCode,
                        ProductName = productRepo.All().Where(w => w.ProductCode == x.ProductCode).Select(p => p.ProductName).FirstOrDefault(),
                        BrandID = x.BrandId,
                        BrandName = brandRepo.All().Where(w => w.BrandId == x.BrandId).Select(b => b.BrandName).FirstOrDefault(),
                        ModelID = x.ModelId,
                        ModelName = modelRepo.All().Where(w => w.ModelId == x.ModelId).Select(m => m.ModelName).FirstOrDefault(),
                        SizeID = x.SizeId,
                        SizeName = sizeRepo.All().Where(w => w.SizeId == x.SizeId).Select(s => s.SizeName).FirstOrDefault(),
                        UnitTypID = x.UnitTypId,
                        UnitTypName = unitRepo.All().Where(w => w.UnitTypId == x.UnitTypId).Select(u => u.UnitTypeName).FirstOrDefault(),
                        StockQty = x.StockQty,
                        IssueQty = x.IssueQty,
                        FloorCode = x.FloorCode,
                        FloorName = floorRepo.All().Where(w => w.FloorCode == x.FloorCode).Select(f => f.FloorName).FirstOrDefault(),
                        Description = productRepo.All().Where(w => w.ProductCode == x.ProductCode).Select(p => p.Description).FirstOrDefault(),
                        Luser = x.Luser
                    }).ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<(bool isSuccess, object data)> detailsEditByIdAsync(decimal id)
        {
            try
            {
                if (id == 0)
                    return (false, UpdateFailed);

                var detailsItem = await productIssueInformationDetailTempRepo.GetByIdAsync(id);

                if (detailsItem == null)
                    return (false, UpdateFailed);


                return (true, detailsItem);
            }
            catch (Exception ex)
            {
                // Optional: log the error
                return (false, $"Error: {ex.Message}");
            }
        }

        public async Task<(bool isSuccess, object data)> detailsDeleteByIdAsync(decimal id)
        {
            try
            {
                if (id == 0)
                    return (false, DeleteFailed);

                var detailsItem = await productIssueInformationDetailTempRepo.GetByIdAsync(id);

                if (detailsItem == null)
                    return (false, DeleteFailed);

                await productIssueInformationDetailTempRepo.DeleteAsync(detailsItem);

                return (true, DeleteSuccess);
            }
            catch (Exception ex)
            {
                // Optional: log the error
                return (false, $"Error: {ex.Message}");
            }
        }

        public async Task<ProductIssueEntrySetupViewModel> EditPopulateIssueidAsync(decimal issueId)
        {
            try
            {
                var exIssueData = productIssueInformationDetailTempRepo.All();
                if (exIssueData != null)
                {
                    await productIssueInformationDetailTempRepo.DeleteRangeAsync(exIssueData);
                }
                var issueData = await ProductIssueInformation.GetByIdAsync(issueId);
                if (issueData == null)
                    return null;

                var detailsList = productIssueInformationDetailRepo
                    .All()
                    .Where(x => x.IssueNo == issueData.IssueNo)
                    .ToList();

                List<InvProductIssueInformationDetailsTemp> detailsTempList = new();

                if (detailsList?.Any() == true)
                {
                    detailsTempList = detailsList.Select(temp => new InvProductIssueInformationDetailsTemp
                    {
                        Pidid = temp.Pidid,
                        IssueNo = temp.IssueNo,
                        ProductCode = temp.ProductCode,
                        BrandId = temp.BrandId,
                        ModelId = temp.ModelId,
                        SizeId = temp.SizeId,
                        UnitTypId = temp.UnitTypId,
                        StockQty = temp.StockQty,
                        IssueQty = temp.IssueQty,
                        FloorCode = temp.FloorCode,
                        Luser = temp.Luser,
                    }).ToList();

                    await productIssueInformationDetailTempRepo.AddRangeAsync(detailsTempList);

                    // Only delete from original table if intended
                    //await productIssueInformationDetailRepo.DeleteRangeAsync(detailsList);
                }

                var issueDetailsData = new ProductIssueEntrySetupViewModel
                {

                    IssueNo = issueData.IssueNo,
                    IssueDate = issueData.IssueDate,
                    FloorCode = issueData.FloorCode,
                    IssuedBy = issueData.IssuedBy,
                    Remarks = issueData.Remarks,
                    TC = issueData.Tc,
                    DepartmentCode = issueData.DepartmentCode,
                    EmployeeID = issueData.EmployeeId,
                    ShowCreateDate = issueData.Ldate.HasValue ? issueData.Ldate.Value.ToString("dd/MM/yyyy") : "",
                    ShowModifyDate = issueData.ModifyDate.HasValue ? issueData.ModifyDate.Value.ToString("dd/MM/yyyy") : "",
                    Details = detailsTempList.Select(x => new ProductIssueInformationDetailViewModel
                    {
                        PIDID = x.Pidid,
                        IssueNo = x.IssueNo,
                        ProductCode = x.ProductCode,
                        BrandID = x.BrandId,
                        ModelID = x.ModelId,
                        SizeID = x.SizeId,
                        UnitTypID = x.UnitTypId,
                        StockQty = x.StockQty,
                        IssueQty = x.IssueQty,
                        FloorCode = x.FloorCode,
                        Luser = x.Luser
                    }).ToList()
                };

                return issueDetailsData;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
