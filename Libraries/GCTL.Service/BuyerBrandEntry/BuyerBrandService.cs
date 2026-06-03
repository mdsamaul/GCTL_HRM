//using DocumentFormat.OpenXml.Drawing.Charts;
//using GCTL.Core.Data;
//using GCTL.Core.ViewModels.BuyerBrands;
//using GCTL.Core.ViewModels.BuyerInfos;
//using GCTL.Data.Models;
//using GCTL.Service.Common;
//using Microsoft.AspNetCore.Http;
//using Microsoft.EntityFrameworkCore;
//using SixLabors.ImageSharp;
//using SixLabors.ImageSharp.Formats.Jpeg;
//using SixLabors.ImageSharp.Processing;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace GCTL.Service.BuyerBrandEntry
//{
//    public class BuyerBrandService : AppService<RmgProdDefBrand>, IBuyerBrandService
//    {
//        #region Private Fields

//        private readonly IRepository<RmgProdDefBrand> brandRepo;
//        private readonly IRepository<RmgProdDefBuyer> buyerRepo;
//        private readonly ICommonService comService;
//        private readonly IRepository<InvItemBrand> photoRepo;
//        private readonly IRepository<CoreAccessCode> accessCodeRepository;
//        #endregion Private Fields

//        #region Public Constructors

//        public BuyerBrandService(ICommonService comService, IRepository<InvItemBrand> photoRepo, IRepository<RmgProdDefBrand> brandRepo, IRepository<RmgProdDefBuyer> buyerRepo, IRepository<CoreAccessCode> accessCodeRepository) : base(brandRepo)
//        {
//            this.comService = comService;
//            this.brandRepo = brandRepo;
//            this.buyerRepo = buyerRepo;
//            this.photoRepo = photoRepo;
//            this.accessCodeRepository = accessCodeRepository;
//        }

//        #endregion Public Constructors

//        #region Public Methods

//        public async Task<(bool isSuccess, string message)> BulkDeleteAsync(List<decimal> tcs, bool useTransaction = true)
//        {

//            if (tcs == null || !tcs.Any())
//                return (false, "Validation Failed");

//            const int batchSize = 500;

//            if (useTransaction)
//                await brandRepo.BeginTransactionAsync();

//            try
//            {

//                for (int i = 0; i < tcs.Count; i += batchSize)
//                {
//                    var batch = tcs.Skip(i).Take(batchSize).ToList();
//                    var entries = await brandRepo.All()
//                        .Where(e => batch.Contains(e.Tc))
//                        .AsNoTracking()
//                        .ToListAsync();

//                    var batchBrandIds = entries.Select(e => e.BrandId).ToList();
//                    var photoToDelete = await photoRepo.All()
//                        .Where(p=> batchBrandIds.Contains(p.BrandId))
//                        .AsNoTracking()
//                        .ToListAsync();

//                    if (!entries.Any()) continue;

//                    await brandRepo.DeleteRangeAsync(entries);

//                    if (photoToDelete.Any())
//                    {
//                        await photoRepo.DeleteRangeAsync(photoToDelete);
//                    }
//                }

//                if (useTransaction)
//                    await brandRepo.CommitTransactionAsync();
//                return (true, "Deleted Successfully");
//            }
//            catch (Exception ex)
//            {
//                if (useTransaction)
//                {
//                    await brandRepo.RollbackTransactionAsync();
//                    return (false, "Internal Server Error!");
//                }

//                throw;
//            }
//        }

//        public async Task<RMGProdBrandViewModel> GetByIdAsync(decimal id)
//        {
//            try
//            {
//                var brand = await brandRepo.GetByIdAsync(id);

//                var logoData = await photoRepo.All().Where(x => x.BrandId == brand.BrandId).FirstOrDefaultAsync();

//                if (brand == null)
//                {
//                    return null;
//                }

//                var record = new RMGProdBrandViewModel
//                {
//                    Tc = brand.Tc,
//                    BuyerId = brand.BuyerId,
//                    BrandId = brand.BrandId,
//                    Name = brand.Name,
//                    Detail = brand.Detail,
//                    Ldate = brand.Ldate,
//                    ModifyDate = brand.ModifyDate,
//                    LogoMonogram = logoData?.BrandLogo != null ? Convert.ToBase64String(logoData.BrandLogo) : null,
//                };

//                return record;

//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }

//        public async Task<(List<RMGProdBrandViewModel> Data, int totalRecord, int curentRecord)> GetPaginatedDataAsync(string searchValue, int page, int pageSize, string sortColumn, string sortDirection, string id, string buyerId)
//        {
//            var query = await (from b in brandRepo.All()
//                              join buy in buyerRepo.All() on b.BuyerId equals buy.BuyerId into bbuy
//                              from buy in bbuy.DefaultIfEmpty()   
//                              join logo in photoRepo.All() on b.BrandId equals logo.BrandId into logoGroup // left join
//                              from logo in logoGroup.DefaultIfEmpty() // left join

//                               select new RMGProdBrandViewModel
//                              {
//                                  Tc = b.Tc,
//                                  BrandId = b.BrandId,
//                                  BuyerId = b.BuyerId,
//                                  BuyerName = buy.BuyerName,
//                                  Name = b.Name,
//                                  LogoMonogram = logo.BrandLogo != null ? Convert.ToBase64String(logo.BrandLogo) : null,
//                                  Detail = b.Detail
//                              })
//                              .ToListAsync();

//            var totalRecord = query.Count();

//            var materializedQuery = query.Where(x =>
//                (id == null || x.BrandId == id) &&
//                (buyerId == null || x.BuyerId == buyerId)
//            );


//            IEnumerable<RMGProdBrandViewModel> filterQuery = materializedQuery;

//            if (!string.IsNullOrWhiteSpace(searchValue))
//            {
//                filterQuery = filterQuery.Where(d =>
//                    (d.BrandId?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ?? false) ||
//                    (d.BuyerName?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ?? false) ||
//                    (d.Name?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ?? false) ||
//                    (d.Detail?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ?? false) 
//                );
//            }

//            var currentRecord = filterQuery.Count();

//            if (!string.IsNullOrWhiteSpace(sortColumn) && !string.IsNullOrWhiteSpace(sortDirection))
//            {
//                filterQuery = sortColumn.ToLower() switch
//                {
//                    "brandid" => sortDirection.ToLower() == "asc" ? filterQuery.OrderBy(x => x.BrandId) : filterQuery.OrderByDescending(x => x.BrandId),
//                    "buyername" => sortDirection.ToLower() == "asc" ? filterQuery.OrderBy(x => x.BuyerName) : filterQuery.OrderByDescending(x => x.BuyerName),
//                    "name" => sortDirection.ToLower() == "asc" ? filterQuery.OrderBy(x => x.Name) : filterQuery.OrderByDescending(x => x.Name),
//                    "detail" => sortDirection.ToLower() == "asc" ? filterQuery.OrderBy(x => x.Detail) : filterQuery.OrderByDescending(x => x.Detail),
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

//        public async Task<(bool isSuccess, string message)> DeleteImageAsync(decimal tc)
//        {
//            var brand = await brandRepo.GetByIdAsync(tc);

//            if (brand == null) return (false, "Not Found");

//            var brandPhoto = await photoRepo.All().Where(x => x.BrandId == brand.BrandId).FirstOrDefaultAsync();

//            if (brandPhoto != null)
//            {
//                await photoRepo.DeleteAsync(brandPhoto);
//                return (true, "Image deleted Successfully!");
//            }
//            else
//            {
//                return (false, "Image not found");
//            }
//        }

//        public async Task<bool> IsDuplicate(string brandName, string buyerId, string id = null)
//        {
//            if(string.IsNullOrWhiteSpace(brandName) || string.IsNullOrWhiteSpace(buyerId))
//            {
//                return false;
//            }

//            return await brandRepo.All()
//                .AnyAsync(b => b.Name.ToLower() == brandName.ToLower() 
//                            && b.BuyerId == buyerId
//                            && (id == null || b.BrandId != id));

//        }

//        public async Task<(bool isSuccess, string message, object data)> SaveAsync(RMGProdBrandViewModel model)
//        {
//            if (model == null || string.IsNullOrWhiteSpace(model.Name))
//            {
//                return (false, "Internal server error!", null);
//            }
//            await brandRepo.BeginTransactionAsync();

//            try
//            {
//                if (model.Tc == 0)
//                {
//                    if(string.IsNullOrWhiteSpace(model.BuyerId))
//                    {
//                        return (false, "Buyer is required!", null);
//                    }

//                    if(await IsDuplicate(model.Name, model.BuyerId))
//                    {
//                        return (false, "Duplicate Brand Name found!", null);
//                    }

//                    var newId = comService.GenerateNextCode("BrandId", "RMG_Prod_Def_Brand", 3);

//                    RmgProdDefBrand record = new RmgProdDefBrand
//                    {
//                        BrandId = newId,
//                        BuyerId = model.BuyerId,
//                        Name = model.Name,
//                        LogoMonogram = string.Empty,
//                        Detail = model.Detail ?? string.Empty,
//                        Ldate = model.Ldate,
//                        Luser = model.Luser,
//                        Lip = model.Lip,
//                        Lmac = model.Lmac,
//                    };
//                    await brandRepo.AddAsync(record);

//                    if(model.logoPhoto != null && model.logoPhoto.Length > 0)
//                    {
//                        await HandleBrandPhoto(record.BrandId, model.CompanyCode, model.UserInfoEmployeeId, model.logoPhoto, true);
//                    }

//                    await brandRepo.CommitTransactionAsync();
//                    return (true, "Saved Successfully!", record);
//                }
//                else
//                {
//                    var exData = await brandRepo.GetByIdAsync(model.Tc);
//                    if (exData == null)
//                        return (false, "Data does not exists!", null);

//                    if(await IsDuplicate(model.Name, model.BuyerId, exData.BrandId))
//                    {
//                        return (false, "Duplicate Brand Name found!", null);
//                    }

//                    exData.Name = model.Name;
//                    exData.Detail = model.Detail ?? string.Empty;

//                    exData.ModifyDate = model.ModifyDate;
//                    exData.Luser = model.Luser;
//                    exData.Lip = model.Lip;
//                    exData.Lmac = model.Lmac;

//                    await brandRepo.UpdateAsync(exData);

//                    if (model.logoPhoto != null && model.logoPhoto.Length > 0)
//                    {
//                        await HandleBrandPhoto(exData.BrandId, model.CompanyCode, model.UserInfoEmployeeId, model.logoPhoto, false);
//                    }

//                    await brandRepo.CommitTransactionAsync();
//                    return (true, "Update Successfully", exData);
//                }
//            }
//            catch (Exception ex)
//            {
//                await brandRepo?.RollbackTransactionAsync();

//                return (false, "Internal Server Error!", null);
//            }
//        }


//        #region Permission all type
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

//        public async Task<bool> DeletePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Buyer Info" && x.CheckDelete);
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

//        private async Task HandleBrandPhoto(string brandId, string companyCode, string userId, IFormFile photo, bool isNew)
//        {
//            try
//            {
//                var compressedPhotoBytes = await CompressImage(photo);

//                if (isNew)
//                {
//                    var buyerPhoto = new InvItemBrand
//                    {
//                        BrandId = brandId,
//                        BrandLogo = compressedPhotoBytes,
//                        ImgType = "image/jpeg",
//                        ImgSize = compressedPhotoBytes.Length,
//                        CompanyCode = companyCode,
//                        EmployeeId = userId,
//                    };
//                    await photoRepo.AddAsync(buyerPhoto);
//                }
//                else
//                {
//                    var existingPhoto = photoRepo.All().FirstOrDefault(x => x.BrandId == brandId);

//                    if (existingPhoto != null)
//                    {
//                        existingPhoto.BrandLogo = compressedPhotoBytes;
//                        existingPhoto.ImgType = "image/jpeg";
//                        existingPhoto.ImgSize = compressedPhotoBytes.Length;
//                        existingPhoto.CompanyCode = companyCode;
//                        existingPhoto.EmployeeId = userId;
//                        await photoRepo.UpdateAsync(existingPhoto);
//                    }
//                    else
//                    {
//                        var buyerPhoto = new InvItemBrand
//                        {
//                            BrandId = brandId,
//                            BrandLogo = compressedPhotoBytes,
//                            ImgType = "image/jpeg",
//                            ImgSize = compressedPhotoBytes.Length,
//                            CompanyCode = companyCode,
//                            EmployeeId = userId,
//                        };
//                        await photoRepo.AddAsync(buyerPhoto);
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
