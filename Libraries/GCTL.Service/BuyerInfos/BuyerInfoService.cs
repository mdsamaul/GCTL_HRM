//using GCTL.Core.Data;
//using GCTL.Core.ViewModels.BuyerInfos;
//using GCTL.Data.Models;
//using GCTL.Service.BuyerBrandEntry;
//using GCTL.Service.BuyerDLAddressEntry;
//using GCTL.Service.Common;
//using Microsoft.AspNetCore.Http;
//using Microsoft.EntityFrameworkCore;
//using SixLabors.ImageSharp;
//using SixLabors.ImageSharp.Formats.Jpeg;
//using SixLabors.ImageSharp.Processing;

//namespace GCTL.Service.BuyerInfos
//{
//    public class BuyerInfoService : AppService<RmgProdDefBuyer>, IBuyerInfoService
//    {
//        #region Private Fields

//        private readonly IRepository<CoreAccessCode> accessCodeRepository;
//        //private readonly IRepository<InvDefBuyerCompany> bComRepo;
//        private readonly IRepository<InvDefBuyerDepartment> bDepRepo;
//        private readonly IRepository<RmgProdDefBuyerPhoto> buyerPhotoRepo;
//        private readonly IRepository<RmgProdDefBuyer> buyerRepo;

//        private readonly IRepository<RmgProdDefBrand> brandRepo;
//        private readonly IRepository<RmgProdDefDeliveryAddress> dlRepo;

//        private readonly IBuyerBrandService brandService;
//        private readonly IBuyerDLAddressService dlService;
//        private readonly ICommonService comService;
//        private readonly IRepository<CaDefCountry> countryRepo;
//        private readonly IRepository<SalesContactPerson> cpRepo;
//        #endregion Private Fields

//        #region Public Constructors

//        public BuyerInfoService(ICommonService comService, IRepository<RmgProdDefBuyer> buyerRepo,
//            //IRepository<InvDefBuyerCompany> 
//            // bComRepo, 
//            IRepository<InvDefBuyerDepartment> bDepRepo, IRepository<CaDefCountry> countryRepo, IRepository<SalesContactPerson> cpRepo, IRepository<RmgProdDefBuyerPhoto> buyerPhotoRepo, IBuyerBrandService brandService, IBuyerDLAddressService dlService, IRepository<CoreAccessCode> accessCodeRepository, IRepository<RmgProdDefBrand> brandRepo, IRepository<RmgProdDefDeliveryAddress> dlRepo) : base(buyerRepo)
//        {
//            this.comService = comService;
//            this.buyerRepo = buyerRepo;
//            //this.bComRepo = bComRepo;
//            this.bDepRepo = bDepRepo;
//            this.countryRepo = countryRepo;
//            this.cpRepo = cpRepo;
//            this.buyerPhotoRepo = buyerPhotoRepo;
//            this.brandService = brandService;
//            this.dlService = dlService;
//            this.brandRepo = brandRepo;
//            this.dlRepo = dlRepo;

//            this.accessCodeRepository = accessCodeRepository;
//        }

//        #endregion Public Constructors

//        #region Public Methods

//        public async Task<(bool isSuccess, string message)> BulkDeleteAsync(List<decimal> tcs)
//        {
//            if (tcs == null || !tcs.Any())
//                return (false, "Validation Failed");

//            const int batchSize = 500;

//            await buyerRepo.BeginTransactionAsync();
//            try
//            {
//                for (int i = 0; i < tcs.Count; i += batchSize)
//                {
//                    var batch = tcs.Skip(i).Take(batchSize).ToList();
//                    var entries = await buyerRepo.All()
//                        .Where(e => batch.Contains(e.Tc))
//                        .AsNoTracking()
//                        .ToListAsync();

//                    var batchBuyerIds = entries.Select(e => e.BuyerId).ToList();
//                    var photoEntries = await buyerPhotoRepo.All()
//                        .Where(p => batchBuyerIds.Contains(p.BuyerId))
//                        .AsNoTracking()
//                        .ToListAsync();

//                    var brandEntries = await brandRepo.All()
//                        .Where(b => batchBuyerIds.Contains(b.BuyerId)).Select(b => b.Tc)
//                        .ToListAsync();

//                    var dlEntries = await dlRepo.All()
//                        .Where(d => batchBuyerIds.Contains(d.BuyerId)).Select(d => d.Tc)
//                        .ToListAsync();

//                    if (!entries.Any()) continue;

//                    await buyerRepo.DeleteRangeAsync(entries);

//                    if (photoEntries.Any()) await buyerPhotoRepo.DeleteRangeAsync(photoEntries);
//                    if (brandEntries.Any()) await brandService.BulkDeleteAsync(brandEntries, false);
//                    if (dlEntries.Any()) await dlService.BulkDeleteAsync(dlEntries, false);
//                }
//                await buyerRepo.CommitTransactionAsync();
//                return (true, "Deleted Successfully");
//            }
//            catch (Exception ex)
//            {
//                await buyerRepo.RollbackTransactionAsync();
//                return (false, "Internal Server Error!");
//            }
//        }

//        public async Task<BuyerInfoSetupViewModel> GetByIdAsync(decimal id)
//        {
//            try
//            {
//                var buyer = await buyerRepo.All().FirstOrDefaultAsync(x => x.Tc == id);

//                if (buyer == null)
//                {
//                    return null;
//                }

//                // Get buyer photo if exists
//                var buyerPhoto = buyerPhotoRepo.All().FirstOrDefault(x => x.BuyerId == buyer.BuyerId);

//                string photoBase64 = null;

//                if (buyerPhoto != null && buyerPhoto.Photo != null)
//                {
//                    photoBase64 = Convert.ToBase64String(buyerPhoto.Photo);
//                }

//                var record = new BuyerInfoSetupViewModel
//                {
//                    Tc = buyer.Tc,
//                    BuyerId = buyer.BuyerId,
//                    BuyerName = buyer.BuyerName,
//                    CompanyId = buyer.CompanyId,
//                    Address = buyer.Address,
//                    LocalOfficeAddress = buyer.LocalOfficeAddress,
//                    BuyerDepartmentId = buyer.BuyerDepartmentId,
//                    CountryId = buyer.CountryId,
//                    Phone = buyer.Phone,
//                    Fax = buyer.Fax,
//                    Email = buyer.Email,
//                    Url = buyer.Url,
//                    ContatPerson1 = buyer.ContatPerson1,
//                    BuyerTypeId = buyer.BuyerTypeId,
//                    SalesPersonId = buyer.SalesPersonId,
//                    Remarks = buyer.Remarks,
//                    Active = buyer.Active,
//                    CompanyCode = buyer.CompanyCode,
//                    Photo = photoBase64,
//                    PhotoType = buyerPhoto?.ImgType,
//                    Ldate = buyer.Ldate,
//                    ModifyDate = buyer.ModifyDate,
//                };

//                return record;

//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }
//        public async Task<(List<BuyerInfoGridViewModel> Data, int totalRecord, int curentRecord)> GetPaginatedDataAsync(string searchValue, int page, int pageSize, string sortColumn, string sortDirection, string id)
//        {
//            var query = await (from b in buyerRepo.All()
//                               join c in countryRepo.All() on b.CountryId equals c.CountryId into bc
//                               from c in bc.DefaultIfEmpty()   // left join
//                               where id == null || b.BuyerId == id
//                               select new
//                               {
//                                   b.Tc,
//                                   b.BuyerId,
//                                   b.BuyerName,
//                                   b.Address,
//                                   CountryName = c != null ? c.CountryName : "",
//                                   b.Phone,
//                                   b.Email,
//                                   b.ContatPerson1
//                               })
//                              .ToListAsync();

//            // Get all contact person IDs from the buyers
//            var allContactPersonIds = query
//                .Where(b => !string.IsNullOrWhiteSpace(b.ContatPerson1))
//                .SelectMany(b => b.ContatPerson1.Split(',', StringSplitOptions.RemoveEmptyEntries))
//                .Select(cp => cp.Trim())
//                .Distinct()
//                .ToList();

//            // Fetch all relevant contact persons
//            var contactPersons = await cpRepo.All()
//                .Where(cp => allContactPersonIds.Contains(cp.Cpid))
//                .ToDictionaryAsync(cp => cp.Cpid, cp => cp.ContactPersonName);

//            // Map to view model with concatenated contact person names
//            var materializedQuery = query.Select(b => new BuyerInfoGridViewModel
//            {
//                Tc = b.Tc,
//                BuyerId = b.BuyerId,
//                BuyerName = b.BuyerName,
//                Address = b.Address,
//                CountryName = b.CountryName,
//                Phone = b.Phone,
//                Email = b.Email,
//                ContactPersonName = string.IsNullOrWhiteSpace(b.ContatPerson1)
//                    ? ""
//                    : string.Join(", ", b.ContatPerson1
//                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
//                        .Select(cp => cp.Trim())
//                        .Where(cp => contactPersons.ContainsKey(cp))
//                        .Select(cp => contactPersons[cp]))
//            }).ToList();

//            var totalRecord = materializedQuery.Count();

//            IEnumerable<BuyerInfoGridViewModel> filterQuery = materializedQuery;

//            if (!string.IsNullOrWhiteSpace(searchValue))
//            {
//                filterQuery = filterQuery.Where(d =>
//                    (d.BuyerId?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ?? false) ||
//                    (d.BuyerName?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ?? false) ||
//                    (d.Address?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ?? false) ||
//                    (d.CountryName?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ?? false) ||
//                    (d.Phone?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ?? false) ||
//                    (d.Email?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ?? false) ||
//                    (d.ContactPersonName?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ?? false)
//                );
//            }

//            var currentRecord = filterQuery.Count();

//            if (!string.IsNullOrWhiteSpace(sortColumn) && !string.IsNullOrWhiteSpace(sortDirection))
//            {
//                filterQuery = sortColumn.ToLower() switch
//                {
//                    "buyerid" => sortDirection.ToLower() == "asc" ? filterQuery.OrderBy(x => x.BuyerId) : filterQuery.OrderByDescending(x => x.BuyerId),
//                    "buyername" => sortDirection.ToLower() == "asc" ? filterQuery.OrderBy(x => x.BuyerName) : filterQuery.OrderByDescending(x => x.BuyerName),
//                    "address" => sortDirection.ToLower() == "asc" ? filterQuery.OrderBy(x => x.Address) : filterQuery.OrderByDescending(x => x.Address),
//                    "countryname" => sortDirection.ToLower() == "asc" ? filterQuery.OrderBy(x => x.CountryName) : filterQuery.OrderByDescending(x => x.CountryName),
//                    "phone" => sortDirection.ToLower() == "asc" ? filterQuery.OrderBy(x => x.Phone) : filterQuery.OrderByDescending(x => x.Phone),
//                    "email" => sortDirection.ToLower() == "asc" ? filterQuery.OrderBy(x => x.Email) : filterQuery.OrderByDescending(x => x.Email),
//                    "contactpersonname" => sortDirection.ToLower() == "asc" ? filterQuery.OrderBy(x => x.ContactPersonName) : filterQuery.OrderByDescending(x => x.ContactPersonName),
//                    _ => filterQuery.OrderBy(a => a.Tc)
//                };
//            }
//            else
//            {
//                filterQuery = filterQuery.OrderBy(a => a.Tc);
//            }

//            var data = pageSize < 0 ? filterQuery.ToList() : filterQuery.Skip((page - 1) * pageSize).Take(pageSize).ToList();

//            return (data, totalRecord, currentRecord);
//        }

//        public async Task<(bool isSuccess, string message, object data)> SaveAsync(BuyerInfoSetupViewModel model)
//        {
//            if (model == null || string.IsNullOrWhiteSpace(model.BuyerName))
//            {
//                return (false, "Internal server error!", null);
//            }

//            await buyerRepo.BeginTransactionAsync();

//            try
//            {
//                if (model.Tc == 0)
//                {
//                    var newId = comService.GenerateNextCode("BuyerId", "RMG_Prod_Def_Buyer", 6, "BUY");

//                    RmgProdDefBuyer record = new RmgProdDefBuyer
//                    {
//                        BuyerId = newId,
//                        BuyerName = model.BuyerName,
//                        CompanyId = model.CompanyId ?? string.Empty,
//                        Address = model.Address ?? string.Empty,
//                        LocalOfficeAddress = model.LocalOfficeAddress ?? string.Empty,
//                        BuyerDepartmentId = model.BuyerDepartmentId ?? string.Empty,
//                        CountryId = model.CountryId ?? string.Empty,
//                        Phone = model.Phone ?? string.Empty,
//                        Fax = model.Fax ?? string.Empty,
//                        Email = model.Email ?? string.Empty,
//                        Url = model.Url ?? string.Empty,
//                        ContatPerson1 = model.ContatPerson1 ?? string.Empty,
//                        BuyerTypeId = model.BuyerTypeId,
//                        SalesPersonId = model.SalesPersonId ?? string.Empty,
//                        Remarks = model.Remarks ?? string.Empty,
//                        Active = model.Active ?? string.Empty,
//                        Luser = model.Luser,
//                        Lip = model.Lip,
//                        Lmac = model.Lmac,
//                        Ldate = model.Ldate,
//                        CompanyCode = model.CompanyCode ?? string.Empty,

//                        DesignationId = string.Empty,
//                        DesignationId2 = string.Empty,
//                        DesignationId3 = string.Empty,
//                        ContatPerson2 = string.Empty,
//                        ContatPerson3 = string.Empty,
//                        Phone1 = string.Empty,
//                        Phone2 = string.Empty,
//                        Phone3 = string.Empty,
//                        Email1 = string.Empty,
//                        Email2 = string.Empty,
//                        Email3 = string.Empty,
//                    };

//                    await buyerRepo.AddAsync(record);

//                    if (model.BuyerPhoto != null && model.BuyerPhoto.Length > 0)
//                    {
//                        await HandleBuyerPhoto(model.BuyerId, model.BuyerPhoto, true);
//                    }

//                    await buyerRepo.CommitTransactionAsync();

//                    return (true, "Saved Successfully", record);
//                }
//                else
//                {
//                    var existingBuyer = await buyerRepo.GetByIdAsync(model.Tc);

//                    if (existingBuyer == null)
//                        return (false, "Data does not exists!", null);

//                    existingBuyer.BuyerName = model.BuyerName;
//                    existingBuyer.CompanyId = model.CompanyId ?? string.Empty;
//                    existingBuyer.Address = model.Address ?? string.Empty;
//                    existingBuyer.LocalOfficeAddress = model.LocalOfficeAddress ?? string.Empty;
//                    existingBuyer.BuyerDepartmentId = model.BuyerDepartmentId ?? string.Empty;
//                    existingBuyer.CountryId = model.CountryId ?? string.Empty;
//                    existingBuyer.Phone = model.Phone ?? string.Empty;
//                    existingBuyer.Fax = model.Fax ?? string.Empty;
//                    existingBuyer.Email = model.Email ?? string.Empty;
//                    existingBuyer.Url = model.Url ?? string.Empty;
//                    existingBuyer.ContatPerson1 = model.ContatPerson1 ?? string.Empty;
//                    existingBuyer.BuyerTypeId = model.BuyerTypeId ?? string.Empty;
//                    existingBuyer.SalesPersonId = model.SalesPersonId ?? string.Empty;
//                    existingBuyer.Remarks = model.Remarks ?? string.Empty;
//                    existingBuyer.Active = model.Active ?? string.Empty;

//                    existingBuyer.CompanyCode = model.CompanyCode ?? string.Empty;

//                    existingBuyer.Luser = model.Luser;
//                    existingBuyer.Lip = model.Lip;
//                    existingBuyer.Lmac = model.Lmac;
//                    existingBuyer.ModifyDate = model.ModifyDate;

//                    await buyerRepo.UpdateAsync(existingBuyer);

//                    if (model.BuyerPhoto != null && model.BuyerPhoto.Length > 0)
//                    {
//                        await HandleBuyerPhoto(existingBuyer.BuyerId, model.BuyerPhoto, false);
//                    }

//                    await buyerRepo.CommitTransactionAsync();

//                    return (true, "Update Successfully", existingBuyer);
//                }
//            }
//            catch (Exception ex)
//            {
//                await buyerRepo.RollbackTransactionAsync();
//                return (false, "Internal Server Error!", null);
//            }
//        }

//        public async Task<(bool isSuccess, string message)> DeleteImageAsync(decimal tc)
//        {
//            var buyer = await buyerRepo.GetByIdAsync(tc);

//            if (buyer == null) return (false, "Not Found");

//            var buyerPhoto = await buyerPhotoRepo.All().Where(x => x.BuyerId == buyer.BuyerId).FirstOrDefaultAsync();

//            if (buyerPhoto != null)
//            {
//                await buyerPhotoRepo.DeleteAsync(buyerPhoto);
//                return (true, "Image deleted Successfully!");
//            }
//            else
//            {
//                return (false, "Image not found");
//            }
//        }

//        #region Permission all type
//        public async Task<bool> DeletePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Buyer Info" && x.CheckDelete);
//        }

//        public async Task<bool> PagePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Buyer Info" && x.TitleCheck);
//        }

//        public async Task<bool> SavePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Buyer Info" && x.CheckAdd);
//        }

//        public async Task<bool> UpdatePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Buyer Info" && x.CheckEdit);
//        }
//        #endregion
//        #endregion Public Methods

//        #region Private Methods


//        private (int width, int height) CalculateNewDimensions(int originalWidth, int originalHeight, int maxWidth, int maxHeight)
//        {
//            double ratioX = (double)maxWidth / originalWidth;
//            double ratioY = (double)maxHeight / originalHeight;
//            double ratio = Math.Min(ratioX, ratioY);

//            int newWidth = (int)(originalWidth * ratio);
//            int newHeight = (int)(originalHeight * ratio);

//            return (newWidth, newHeight);
//        }

//        private async Task<byte[]> CompressImage(IFormFile imageFile, int maxWidth = 800, int maxHeight = 600, int quality = 75)
//        {
//            using var imageStream = imageFile.OpenReadStream();
//            using var image = Image.Load(imageStream);

//            var (newWidth, newHeight) = CalculateNewDimensions(image.Width, image.Height, maxWidth, maxHeight);

//            image.Mutate(x => x.Resize(newWidth, newHeight));

//            using var outputStream = new MemoryStream();

//            var encoder = new JpegEncoder()
//            {
//                Quality = quality
//            };

//            await image.SaveAsync(outputStream, encoder);
//            return outputStream.ToArray();
//        }

//        private async Task HandleBuyerPhoto(string buyerId, IFormFile photo, bool isNew)
//        {
//            try
//            {
//                var compressedPhotoBytes = await CompressImage(photo);

//                if (isNew)
//                {
//                    var buyerPhoto = new RmgProdDefBuyerPhoto
//                    {
//                        BuyerId = buyerId,
//                        Photo = compressedPhotoBytes,
//                        ImgType = "image/jpeg",
//                        ImgSize = compressedPhotoBytes.Length
//                    };
//                    await buyerPhotoRepo.AddAsync(buyerPhoto);
//                }
//                else
//                {
//                    var existingPhoto = buyerPhotoRepo.All().FirstOrDefault(x => x.BuyerId == buyerId);
//                    if (existingPhoto != null)
//                    {
//                        existingPhoto.Photo = compressedPhotoBytes;
//                        existingPhoto.ImgType = "image/jpeg";
//                        existingPhoto.ImgSize = compressedPhotoBytes.Length;
//                        await buyerPhotoRepo.UpdateAsync(existingPhoto);
//                    }
//                    else
//                    {
//                        var buyerPhoto = new RmgProdDefBuyerPhoto
//                        {
//                            BuyerId = buyerId,
//                            Photo = compressedPhotoBytes,
//                            ImgType = "image/jpeg",
//                            ImgSize = compressedPhotoBytes.Length
//                        };
//                        await buyerPhotoRepo.AddAsync(buyerPhoto);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                throw new Exception($"Error handling buyer photo: {ex.Message}");
//            }
//        }
//        #endregion Private Methods

//    }
//}
