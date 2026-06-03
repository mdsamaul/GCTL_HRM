using GCTL.Core.Data;
using GCTL.Core.ViewModels.ProductIssueReport;
using GCTL.Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace GCTL.Service.ProductIssueReports
{
    public class ProductIssueReportService : AppService<InvProductIssueInformation>, IProductIssueReportService
    {
        private readonly IRepository<InvProductIssueInformation> invProductIssueInformationRepo;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly string _connectionString;

        public ProductIssueReportService(
                IRepository<InvProductIssueInformation> InvProductIssueInformationRepo,
                IRepository<CoreAccessCode> accessCodeRepository,
                IConfiguration configuration
            ) : base(InvProductIssueInformationRepo)
        {
            this.invProductIssueInformationRepo = InvProductIssueInformationRepo;
            this.accessCodeRepository = accessCodeRepository;
            _connectionString = configuration.GetConnectionString("ApplicationDbConnection");
        }
        public async Task<bool> PagePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Product Issue Report" && x.TitleCheck);

        }

        public async Task<List<ProductIssueReportSetupViewModel>> GetProductIssueReportAsync(ProductIssueReportFilterViewModel filter)
        {
            try
            {

                var result = new List<ProductIssueReportSetupViewModel>();
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("INV_ProductIssueReport_Filter", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@FromDate", (object?)filter.FromDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ToDate", (object?)filter.ToDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProductCodes", string.Join(",", filter.ProductCodes));
                    cmd.Parameters.AddWithValue("@CategoryIds", string.Join(",", filter.CategoryIds));
                    cmd.Parameters.AddWithValue("@BrandIds", string.Join(",", filter.BrandIds));
                    cmd.Parameters.AddWithValue("@ModelIds", string.Join(",", filter.ModelIds));
                    cmd.Parameters.AddWithValue("@DepartmentCodes", string.Join(",", filter.DepartmentCodes));
                    cmd.Parameters.AddWithValue("@EmployeeIds", string.Join(",", filter.EmployeeIds));
                    cmd.Parameters.AddWithValue("@FloorIds", string.Join(",", filter.FloorIds));

                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new ProductIssueReportSetupViewModel
                            {
                                IssueNo = reader["IssueNo"].ToString(),
                                IssueQty = Convert.ToDecimal(reader["IssueQty"].ToString()),
                                CatagoryName = reader["CatagoryName"].ToString(),
                                ProductName = reader["ProductName"].ToString(),
                                BrandName = reader["BrandName"].ToString(),
                                ModelName = reader["ModelName"].ToString(),
                                SizeName = reader["SizeName"].ToString(),
                                UnitTypeName = reader["UnitTypeName"].ToString(),
                                DepartmentName = reader["DepartmentName"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                FloorName = reader["FloorName"].ToString(),
                            });
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

        public async Task<ProductIssueDropdownDto> GetProductIssueDropdownAsync(ProductIssueReportFilterViewModel model)
        {
            try
            {
                var dropdownDto = new ProductIssueDropdownDto
                {
                    CategoryList = new List<DropdownItem>(),
                    ProductList = new List<DropdownItem>(),
                    BrandList = new List<DropdownItem>(),
                    ModelList = new List<DropdownItem>(),
                    DepartmentList = new List<DropdownItem>(),
                    EmployeeList = new List<DropdownItem>(),
                    FloorList = new List<DropdownItem>()
                };
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INV_ProductIssueReport_Filter", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FromDate", model.FromDate != null ? string.Join(',', model.FromDate) : DBNull.Value);
                        cmd.Parameters.AddWithValue("@ToDate", model.ToDate != null ? string.Join(',', model.ToDate) : DBNull.Value);
                        cmd.Parameters.AddWithValue("@ProductCodes", model.ProductCodes != null ? string.Join(',', model.ProductCodes) : DBNull.Value);
                        cmd.Parameters.AddWithValue("@CategoryIds", model.CategoryIds != null ? string.Join(',', model.CategoryIds) : DBNull.Value);
                        cmd.Parameters.AddWithValue("@BrandIds", model.BrandIds != null ? string.Join(',', model.BrandIds) : DBNull.Value);
                        cmd.Parameters.AddWithValue("@ModelIds", model.ModelIds != null ? string.Join(',', model.ModelIds) : DBNull.Value);
                        cmd.Parameters.AddWithValue("@DepartmentCodes", model.DepartmentCodes != null ? string.Join(',', model.DepartmentCodes) : DBNull.Value);
                        cmd.Parameters.AddWithValue("@EmployeeIds", model.EmployeeIds != null ? string.Join(',', model.EmployeeIds) : DBNull.Value);
                        cmd.Parameters.AddWithValue("@FloorIds", model.FloorIds != null ? string.Join(',', model.FloorIds) : DBNull.Value);
                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            var categoryList = new Dictionary<string, string>();
                            var productList = new Dictionary<string, string>();
                            var brandList = new Dictionary<string, string>();
                            var modelList = new Dictionary<string, string>();
                            var departmentList = new Dictionary<string, string>();
                            var employeeList = new Dictionary<string, string>();
                            var floorList = new Dictionary<string, string>();
                            while (await reader.ReadAsync())
                            {
                                if (reader["CatagoryID"] != DBNull.Value)
                                {
                                    var categoryId = reader["CatagoryID"].ToString();
                                    if (!categoryList.ContainsKey(categoryId))
                                    {
                                        categoryList[categoryId] = reader["CatagoryName"].ToString();
                                    }
                                }
                                if (reader["ProductCode"] != DBNull.Value)
                                {
                                    var productId = reader["ProductCode"].ToString();
                                    if (!productList.ContainsKey(productId))
                                    {
                                        productList[productId] = reader["ProductName"].ToString();
                                    }
                                }
                                if (reader["BrandID"] != DBNull.Value)
                                {
                                    var brandId = reader["BrandID"].ToString();
                                    if (!brandList.ContainsKey(brandId))
                                    {
                                        brandList[brandId] = reader["BrandName"].ToString();
                                    }
                                }
                                if (reader["ModelID"] != DBNull.Value)
                                {
                                    var modelId = reader["ModelID"].ToString();
                                    if (!modelList.ContainsKey(modelId))
                                    {
                                        modelList[modelId] = reader["ModelName"].ToString();
                                    }
                                }
                                if (reader["DepartmentCode"] != DBNull.Value)
                                {
                                    var departmentCode = reader["DepartmentCode"].ToString();
                                    if (!departmentList.ContainsKey(departmentCode))
                                    {
                                        departmentList[departmentCode] = reader["DepartmentName"].ToString();
                                    }
                                }
                                if (reader["EmployeeID"] != DBNull.Value)
                                {
                                    var employeeId = reader["EmployeeID"].ToString();
                                    if (!employeeList.ContainsKey(employeeId))
                                    {
                                        employeeList[employeeId] = reader["FullName"].ToString();
                                    }
                                }
                                if (reader["FloorCode"] != DBNull.Value)
                                {
                                    var floorId = reader["FloorCode"].ToString();
                                    if (!floorList.ContainsKey(floorId))
                                    {
                                        floorList[floorId] = reader["FloorName"].ToString();
                                    }
                                }
                            }
                            dropdownDto.CategoryList = categoryList.Select(c => new DropdownItem
                            {
                                Id = c.Key,
                                Name = c.Value
                            }).ToList();
                            dropdownDto.ProductList = productList.Select(p => new DropdownItem
                            {
                                Id = p.Key,
                                Name = p.Value
                            }).ToList();
                            dropdownDto.BrandList = brandList.Select(b => new DropdownItem
                            {
                                Id = b.Key,
                                Name = b.Value
                            }).ToList();
                            dropdownDto.ModelList = modelList.Select(m => new DropdownItem
                            {
                                Id = m.Key,
                                Name = m.Value
                            }).ToList();
                            dropdownDto.DepartmentList = departmentList.Select(d => new DropdownItem
                            {
                                Id = d.Key,
                                Name = d.Value
                            }).ToList();
                            dropdownDto.EmployeeList = employeeList.Select(e => new DropdownItem
                            {
                                Id = e.Key,
                                Name = e.Value
                            }).ToList();
                            dropdownDto.FloorList = floorList.Select(f => new DropdownItem
                            {
                                Id = f.Key,
                                Name = f.Value
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
    }
}
