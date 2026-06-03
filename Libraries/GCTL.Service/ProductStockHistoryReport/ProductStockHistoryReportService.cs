using GCTL.Core.Data;
using GCTL.Core.ViewModels.ProductStockHistoryReport;
using GCTL.Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace GCTL.Service.ProductStockHistoryReport
{
    public class ProductStockHistoryReportService : AppService<HrmItemMasterInformation>
        , IProductStockHistoryReportService
    {
        private readonly IRepository<HrmItemMasterInformation> ItemMasterInformationRepo;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly string _connectionString;

        public ProductStockHistoryReportService(
            IRepository<HrmItemMasterInformation> ItemMasterInformationRepo,
            IRepository<CoreAccessCode> accessCodeRepository,
            IConfiguration configuration
            ) : base(ItemMasterInformationRepo)
        {
            this.ItemMasterInformationRepo = ItemMasterInformationRepo;
            this.accessCodeRepository = accessCodeRepository;
            _connectionString = configuration.GetConnectionString("ApplicationDbConnection");
        }
        public async Task<bool> PagePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Product Stock History Report" && x.TitleCheck);

        }
        public async Task<ProductStockHistoryReportDropdownDto> GetFilteredDropdownAsync(StockReportFilterViewModel model)
        {
            try
            {
                var dropdownDto = new ProductStockHistoryReportDropdownDto
                {
                    CategoryIds = new List<DropdownItemDto>(),
                    ProductIds = new List<DropdownItemDto>(),
                    BrandIds = new List<DropdownItemDto>(),
                    ModelIds = new List<DropdownItemDto>()
                };
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {

                    using (SqlCommand cmd = new SqlCommand("INV_StockReport_Filter", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CategoryId", model.CategoryIds != null ? string.Join(",", model.CategoryIds) : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ProductCode", model.ProductCodes != null ? string.Join(",", model.ProductCodes) : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@BrandId", model.BrandIds != null ? string.Join(",", model.BrandIds) : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ModelId", model.ModelIds != null ? string.Join(",", model.ModelIds) : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@FromDate", model.FromDate ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ToDate", model.ToDate ?? (object)DBNull.Value);


                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            var categoryDict = new Dictionary<string, string>();
                            var productDict = new Dictionary<string, string>();
                            var brandDict = new Dictionary<string, string>();
                            var modelDict = new Dictionary<string, string>();
                            while (await reader.ReadAsync())
                            {
                                var catId = reader["CatagoryID"]?.ToString();
                                var catName = reader["CatagoryName"]?.ToString();
                                var prodId = reader["ProductCode"]?.ToString();
                                var prodName = reader["ProductName"]?.ToString();
                                var brandId = reader["BrandID"]?.ToString();
                                var brandName = reader["BrandName"]?.ToString();
                                var modelId = reader["ModelID"]?.ToString();
                                var modelName = reader["ModelName"]?.ToString();
                                if (!string.IsNullOrWhiteSpace(catId) && !categoryDict.ContainsKey(catId))
                                {
                                    categoryDict[catId] = catName;
                                }
                                if (!string.IsNullOrWhiteSpace(prodId) && !productDict.ContainsKey(prodId))
                                {
                                    productDict[prodId] = prodName;
                                }
                                if (!string.IsNullOrWhiteSpace(brandId) && !brandDict.ContainsKey(brandId))
                                {
                                    brandDict[brandId] = brandName;
                                }
                                if (!string.IsNullOrWhiteSpace(modelId) && !modelDict.ContainsKey(modelId))
                                {
                                    modelDict[modelId] = modelName;
                                }
                            }
                            dropdownDto.CategoryIds = categoryDict.Select(x => new DropdownItemDto
                            {
                                Id = x.Key,
                                Name = x.Value
                            }).ToList();
                            dropdownDto.ProductIds = productDict.Select(x => new DropdownItemDto
                            {
                                Id = x.Key,
                                Name = x.Value
                            }).ToList();
                            dropdownDto.BrandIds = brandDict.Select(x => new DropdownItemDto
                            {
                                Id = x.Key,
                                Name = x.Value
                            }).ToList();
                            dropdownDto.ModelIds = modelDict.Select(x => new DropdownItemDto
                            {
                                Id = x.Key,
                                Name = x.Value
                            }).ToList();
                        }
                    }
                }
                return dropdownDto;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<ProductStockHistoryReportSetupViewModel>> GetStockReportAsync(StockReportFilterViewModel filter)
        {
            try
            {
                var result = new List<ProductStockHistoryReportSetupViewModel>();

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INV_StockReport_Filter", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@FromDate", (object?)filter.FromDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ToDate", (object?)filter.ToDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ProductCode", string.Join(",", filter.ProductCodes ?? new List<string>()));
                        cmd.Parameters.AddWithValue("@CategoryId", string.Join(",", filter.CategoryIds ?? new List<string>()));
                        cmd.Parameters.AddWithValue("@BrandId", string.Join(",", filter.BrandIds ?? new List<string>()));
                        cmd.Parameters.AddWithValue("@ModelId", string.Join(",", filter.ModelIds ?? new List<string>()));

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {

                            while (await reader.ReadAsync())
                            {


                                result.Add(new ProductStockHistoryReportSetupViewModel
                                {
                                    ProductCode = reader["ProductCode"].ToString() ?? "",
                                    ProductName = reader["ProductName"].ToString() ?? "",
                                    Description = reader["Description"].ToString() ?? "",
                                    BrandID = reader["BrandID"].ToString(),
                                    BrandName = reader["BrandName"].ToString(),
                                    CatagoryID = reader["CatagoryID"].ToString(),
                                    CatagoryName = reader["CatagoryName"].ToString(),
                                    ModelID = reader["ModelID"].ToString(),
                                    ModelName = reader["ModelName"].ToString(),
                                    SizeID = reader["SizeID"].ToString(),
                                    SizeName = reader["SizeName"].ToString(),
                                    UnitId = reader["UnitId"].ToString(),
                                    UnitName = reader["UnitName"].ToString(),
                                    UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                                    OpeningQty = reader["OpeningQty"].ToString(),
                                    ReceivedQty = reader["ReceivedQty"].ToString(),
                                    IssuedQty = reader["IssuedQty"].ToString(),
                                    StockQty = reader["StockQty"].ToString(),
                                    BalanceQty = reader["BalanceQty"].ToString(),
                                    StockValue = Convert.ToDecimal(reader["StockValue"])
                                });

                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
