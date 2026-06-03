//using Dapper;
//using GCTL.Core.Data;
//using GCTL.Core.ViewModels.MonthWiseOrderBookingReport;
//using GCTL.Data.Models;
//using Microsoft.Data.SqlClient;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using System.Data;
//using System.Globalization;

//namespace GCTL.Service.MonthWiseOrderBookingReport
//{
//    public class OrderReportService : AppService<OrderReportData>, IOrderReportService
//    {
//        private readonly string _connectionString;
//        private readonly IRepository<OrderReportData> orderReportDataRepo;
//        private readonly IRepository<RmgProdDefBuyer> buyerRepo;
//        private readonly IRepository<ProdDefStyle> styleRepo;
//        private readonly IRepository<InvDefItem> itemRepo;
//        private readonly IRepository<CoreCompany> comRepo;
//        private readonly IRepository<RmgProdDefColor> colorRepo;
//        private readonly IRepository<CoreAccessCode> accessCodeRepository;
//        private readonly IRepository<RmgProdDefSize> sizeRepo;

//        public OrderReportService(
//            IRepository<OrderReportData> orderReportDataRepo,
//            IRepository<RmgProdDefBuyer> buyerRepo,
//            IRepository<ProdDefStyle> styleRepo,
//            IRepository<InvDefItem> itemRepo,
//            IRepository<CoreCompany> comRepo,
//            IRepository<RmgProdDefColor> colorRepo,
//            IRepository<CoreAccessCode> accessCodeRepository,
//            IRepository<RmgProdDefSize> sizeRepo,
//            IConfiguration configuration

//            ) : base(orderReportDataRepo)
//        {

//            _connectionString = configuration.GetConnectionString("ApplicationDbConnection");
//            this.orderReportDataRepo = orderReportDataRepo;
//            this.buyerRepo = buyerRepo;
//            this.styleRepo = styleRepo;
//            this.itemRepo = itemRepo;
//            this.comRepo = comRepo;
//            this.colorRepo = colorRepo;
//            this.accessCodeRepository = accessCodeRepository;
//            this.sizeRepo = sizeRepo;
//        }

//        public async Task<bool> PagePermissionAsync(string accessCode)

//        {

//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Month Wise Order Booking Report" && x.TitleCheck);

//        }

//        public async Task<OrderReportAllStyleResponse> GetOrderReportAllStyleAsync(OrderReportRequest request, string companyCode)
//        {
//            using var connection = new SqlConnection(_connectionString);

//            var parameters = new DynamicParameters();
//            parameters.Add("@FromDate", request.FromDate, DbType.Date);
//            parameters.Add("@ToDate", request.ToDate, DbType.Date);
//            parameters.Add("@FromYear", request.FromYear, DbType.Int32);
//            parameters.Add("@ToYear", request.ToYear, DbType.Int32);
//            parameters.Add("@BuyerIds", request.BuyerIds?.Any() == true ? string.Join(",", request.BuyerIds) : null, DbType.String);
//            //parameters.Add("@StyleId", request.StyleIds?.Any() == true ? string.Join(",", request.StyleIds) : null, DbType.String);
//            //parameters.Add("@PurchaseOrder", request.PurchaseOrders?.Any() == true ? string.Join(",", request.PurchaseOrders) : null, DbType.String);
//            //parameters.Add("@ColorId", request.ColorIds?.Any() == true ? string.Join(",", request.ColorIds) : null, DbType.String);
//            //parameters.Add("@SizeId", request.SizeIds?.Any() == true ? string.Join(",", request.SizeIds) : null, DbType.String);

//            var result = await connection.QueryAsync<dynamic>("sp_GetOrderReport", parameters, commandType: CommandType.StoredProcedure);

//            var groupedData = new Dictionary<string, OrderReportDataAllStyle>();
//            var allMonthKeys = new HashSet<string>();
//            int slNo = 1;

//            foreach (var row in result)
//            {
//                string buyerId = row.BuyerId?.ToString() ?? "";
//                string buyerName = buyerRepo.All().Where(x => x.BuyerId == buyerId).Select(x => x.BuyerName).FirstOrDefault() ?? "";

//                if (!groupedData.ContainsKey(buyerName))
//                {
//                    groupedData[buyerName] = new OrderReportDataAllStyle
//                    {
//                        SlNo = slNo++.ToString(),
//                        BuyerName = buyerName,
//                        Style = "",
//                        Item = "",
//                        TotalOrderQuantity = row.TotalQuantity?.ToString() ?? ""
//                    };
//                }

//                var dto = groupedData[buyerName];

//                // Collect unique styles
//                string currentId = row.Style?.ToString() ?? "";
//                string currentStyle = styleRepo.All().Where(x => x.StyleId == currentId).Select(e => e.Style).FirstOrDefault() ?? "";
//                if (!string.IsNullOrEmpty(currentStyle) && !dto.Style.Contains(currentStyle))
//                {
//                    dto.Style = string.IsNullOrEmpty(dto.Style) ? currentStyle : dto.Style + ", " + currentStyle;
//                }

//                // Collect unique items
//                string currentItemId = row.ProductId?.ToString() ?? "";
//                string currentItem = itemRepo.All().Where(x => x.ItemId == currentItemId).Select(s => s.ItemName).FirstOrDefault() ?? "";
//                if (!string.IsNullOrEmpty(currentItem) && !dto.Item.Contains(currentItem))
//                {
//                    dto.Item = string.IsNullOrEmpty(dto.Item) ? currentItem : dto.Item + ", " + currentItem;
//                }

//                // Extract monthly quantities
//                var rowDict = (IDictionary<string, object>)row;
//                foreach (var kvp in rowDict)
//                {
//                    if (kvp.Key.Contains("-"))
//                    {
//                        allMonthKeys.Add(kvp.Key);

//                        decimal currentValue = 0;
//                        if (dto.MonthlyQuantities.TryGetValue(kvp.Key, out var existingValue) &&
//                            decimal.TryParse(existingValue, out var parsedExisting))
//                        {
//                            currentValue = parsedExisting;
//                        }

//                        if (kvp.Value != null && decimal.TryParse(kvp.Value.ToString(), out var incomingValue))
//                        {
//                            currentValue += incomingValue;
//                        }

//                        dto.MonthlyQuantities[kvp.Key] = currentValue == 0 ? "" : currentValue.ToString();
//                    }
//                }
//            }

//            // Determine report year
//            string reportYear = "";
//            if (request.FromYear.HasValue && request.ToYear.HasValue)
//            {
//                reportYear = request.FromYear == request.ToYear
//                    ? $"({request.FromYear})"
//                    : $"({request.FromYear} - {request.ToYear})";
//            }
//            else if (request.FromDate.HasValue && request.ToDate.HasValue)
//            {
//                int startYear = request.FromDate.Value.Year;
//                int endYear = request.ToDate.Value.Year;
//                reportYear = startYear == endYear
//                    ? $"({startYear})"
//                    : $"({startYear} - {endYear})";
//            }

//            return new OrderReportAllStyleResponse
//            {
//                CompanyName = comRepo.All().Where(x => x.CompanyCode == companyCode).Select(s => s.CompanyName).FirstOrDefault() ?? "",
//                ReportTitle = "Month Wise Order Booking Status",
//                ReportYear = reportYear,
//                Data = groupedData.Values.OrderBy(x => int.Parse(x.SlNo)).ToList()
//            };
//        }


//        //style
//        public async Task<OrderReportStyleResponse> GetOrderReportStyleAsync(OrderReportRequest request, string companyCode)
//        {
//            try
//            {
//                using var connection = new SqlConnection(_connectionString);

//                var parameters = new DynamicParameters();
//                parameters.Add("@FromDate", request.FromDate, DbType.Date);
//                parameters.Add("@ToDate", request.ToDate, DbType.Date);
//                parameters.Add("@FromYear", request.FromYear, DbType.Int32);
//                parameters.Add("@ToYear", request.ToYear, DbType.Int32);
//                parameters.Add("@BuyerIds", request.BuyerIds?.Any() == true ? string.Join(",", request.BuyerIds) : null, DbType.String);

//                var result = await connection.QueryAsync<dynamic>("sp_GetOrderReport", parameters, commandType: CommandType.StoredProcedure);

//                var groupedData = new Dictionary<string, OrderReportDataStyle>();
//                var allMonthKeys = new SortedSet<string>();
//                int slNo = 1;

//                foreach (var row in result)
//                {
//                    string buyerId = row.BuyerId?.ToString() ?? "";
//                    string styleId = row.Style?.ToString() ?? "";
//                    string key = $"{buyerId}_{styleId}";

//                    string buyerName = buyerRepo.All().Where(x => x.BuyerId == buyerId).Select(x => x.BuyerName).FirstOrDefault() ?? "";
//                    string styleName = styleRepo.All().Where(x => x.StyleId == styleId).Select(e => e.Style).FirstOrDefault() ?? "";

//                    if (!groupedData.ContainsKey(key))
//                    {
//                        groupedData[key] = new OrderReportDataStyle
//                        {
//                            SlNo = slNo++.ToString(),
//                            BuyerName = buyerName,
//                            Style = styleName,
//                            Item = "",
//                            TotalOrderQuantity = row.DetailsQuantity?.ToString() ?? "",
//                            MonthlyQuantities = new Dictionary<string, string>()
//                        };
//                    }

//                    var dto = groupedData[key];

//                    // Collect unique items for this style
//                    string currentItemId = row.ProductId?.ToString() ?? "";
//                    string currentItem = itemRepo.All().Where(x => x.ItemId == currentItemId).Select(s => s.ItemName).FirstOrDefault() ?? "";
//                    if (!string.IsNullOrEmpty(currentItem))
//                    {
//                        var items = dto.Item.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
//                        if (!items.Contains(currentItem))
//                        {
//                            items.Add(currentItem);
//                            dto.Item = string.Join(", ", items);
//                        }
//                    }

//                    // Extract and sum monthly quantities
//                    var rowDict = (IDictionary<string, object>)row;
//                    foreach (var kvp in rowDict)
//                    {
//                        // Check if it's a month column (format: Jan-19, Feb-19, etc.)
//                        if (kvp.Key.Contains("-") && kvp.Key.Length >= 6)
//                        {
//                            allMonthKeys.Add(kvp.Key);

//                            decimal currentValue = 0;
//                            if (dto.MonthlyQuantities.TryGetValue(kvp.Key, out var existingValue) &&
//                                !string.IsNullOrEmpty(existingValue) &&
//                                decimal.TryParse(existingValue, out var parsedExisting))
//                            {
//                                currentValue = parsedExisting;
//                            }

//                            if (kvp.Value != null &&
//                                !string.IsNullOrEmpty(kvp.Value.ToString()) &&
//                                decimal.TryParse(kvp.Value.ToString(), out var incomingValue))
//                            {
//                                currentValue += incomingValue;
//                            }

//                            dto.MonthlyQuantities[kvp.Key] = currentValue == 0 ? "" : currentValue.ToString();
//                        }
//                    }
//                }

//                // Determine report year
//                string reportYear = "";
//                if (request.FromYear.HasValue && request.ToYear.HasValue)
//                {
//                    reportYear = request.FromYear == request.ToYear
//                        ? $"({request.FromYear})"
//                        : $"({request.FromYear} - {request.ToYear})";
//                }
//                else if (request.FromDate.HasValue && request.ToDate.HasValue)
//                {
//                    int startYear = request.FromDate.Value.Year;
//                    int endYear = request.ToDate.Value.Year;
//                    reportYear = startYear == endYear
//                        ? $"({startYear})"
//                        : $"({startYear} - {endYear})";
//                }

//                var sortedData = groupedData.Values.OrderBy(x => x.BuyerName).ThenBy(x => x.Style).ToList();

//                // Reassign SlNo after sorting
//                for (int i = 0; i < sortedData.Count; i++)
//                {
//                    sortedData[i].SlNo = (i + 1).ToString();
//                }

//                return new OrderReportStyleResponse
//                {
//                    CompanyName = comRepo.All().Where(x => x.CompanyCode == companyCode).Select(s => s.CompanyName).FirstOrDefault() ?? "",
//                    ReportTitle = "Month Wise Order Booking Status",
//                    ReportYear = reportYear,
//                    Data = sortedData,
//                    //MonthColumns = allMonthKeys.ToList()
//                    MonthColumns = allMonthKeys.Select(m =>
//                    {
//                        var formats = new[] { "MMM-yy", "MMM-yyyy" };
//                        DateTime.TryParseExact(m, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsed);
//                        return new { Key = m, Date = parsed };
//                    })
//        .OrderBy(x => x.Date)
//        .Select(x => x.Key)
//        .ToList()
//                };
//            }
//            catch (Exception)
//            {

//                throw;
//            }
//        }



//        //style po

//        public async Task<OrderReportStylePoResponse> GetOrderReportStylePoAsync(OrderReportRequest request, string companyCode)
//        {
//            try
//            {
//                using var connection = new SqlConnection(_connectionString);

//                var parameters = new DynamicParameters();
//                parameters.Add("@FromDate", request.FromDate, DbType.Date);
//                parameters.Add("@ToDate", request.ToDate, DbType.Date);
//                parameters.Add("@FromYear", request.FromYear, DbType.Int32);
//                parameters.Add("@ToYear", request.ToYear, DbType.Int32);
//                parameters.Add("@BuyerIds", request.BuyerIds?.Any() == true ? string.Join(",", request.BuyerIds) : null, DbType.String);

//                var result = await connection.QueryAsync<dynamic>("sp_GetOrderReport", parameters, commandType: CommandType.StoredProcedure);

//                var groupedData = new Dictionary<string, OrderReportDataStylePo>();
//                var allMonthKeys = new SortedSet<string>();

//                foreach (var row in result)
//                {
//                    string buyerId = row.BuyerId?.ToString() ?? "";
//                    string styleId = row.Style?.ToString() ?? "";
//                    string purchaseOrder = row.PurchaseOrder?.ToString() ?? "";
//                    string detailOrderId = row.DetailOrderId?.ToString() ?? "";

//                    string key = $"{buyerId}_{styleId}_{purchaseOrder}_{detailOrderId}";

//                    string buyerName = buyerRepo.All().Where(x => x.BuyerId == buyerId).Select(x => x.BuyerName).FirstOrDefault() ?? "";
//                    string styleName = styleRepo.All().Where(x => x.StyleId == styleId).Select(e => e.Style).FirstOrDefault() ?? "";

//                    if (!groupedData.ContainsKey(key))
//                    {
//                        groupedData[key] = new OrderReportDataStylePo
//                        {
//                            SlNo = "",
//                            BuyerName = buyerName,
//                            Style = styleName,
//                            Item = "",
//                            PurchaseOrder = purchaseOrder,
//                            OrderQuantity = row.DetailsQuantity?.ToString() ?? "",
//                            MonthlyQuantities = new Dictionary<string, string>()
//                        };
//                    }

//                    var dto = groupedData[key];

//                    // Collect unique items
//                    string currentItemId = row.ProductId?.ToString() ?? "";
//                    string currentItem = itemRepo.All().Where(x => x.ItemId == currentItemId).Select(s => s.ItemName).FirstOrDefault() ?? "";
//                    if (!string.IsNullOrEmpty(currentItem))
//                    {
//                        var items = dto.Item.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
//                        if (!items.Contains(currentItem))
//                        {
//                            items.Add(currentItem);
//                            dto.Item = string.Join(", ", items);
//                        }
//                    }

//                    var rowDict = (IDictionary<string, object>)row;

//                    foreach (var kvp in rowDict)
//                    {
//                        if (kvp.Key.Contains("-") && kvp.Key.Length >= 6)
//                        {
//                            allMonthKeys.Add(kvp.Key);

//                            decimal currentValue = 0;
//                            if (dto.MonthlyQuantities.TryGetValue(kvp.Key, out var existingValue) &&
//                                !string.IsNullOrWhiteSpace(existingValue) &&
//                                decimal.TryParse(existingValue.Split(' ').First(), NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedExisting))
//                            {
//                                currentValue = parsedExisting;
//                            }

//                            if (kvp.Value != null &&
//                                decimal.TryParse(kvp.Value?.ToString()?.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var incomingValue))
//                            {
//                                currentValue += incomingValue;
//                            }

//                            // ✅ Extract PO Date
//                            string poDate = "";
//                            if (rowDict.ContainsKey("PODate") &&
//                                rowDict["PODate"] != null &&
//                                DateTime.TryParse(rowDict["PODate"].ToString(), out var parsedDate))
//                            {
//                                poDate = parsedDate.ToString("dd/MM/yyyy");
//                            }

//                            // ✅ Apply final format
//                            dto.MonthlyQuantities[kvp.Key] = currentValue == 0
//                                ? ""
//                                : poDate == ""
//                                    ? currentValue.ToString()
//                                    : $"{currentValue} (sh: {poDate})";
//                        }
//                    }


//                }

//                // Determine report year
//                string reportYear = "";
//                if (request.FromYear.HasValue && request.ToYear.HasValue)
//                {
//                    reportYear = request.FromYear == request.ToYear
//                        ? $"({request.FromYear})"
//                        : $"({request.FromYear} - {request.ToYear})";
//                }
//                else if (request.FromDate.HasValue && request.ToDate.HasValue)
//                {
//                    int startYear = request.FromDate.Value.Year;
//                    int endYear = request.ToDate.Value.Year;
//                    reportYear = startYear == endYear
//                        ? $"({startYear})"
//                        : $"({startYear} - {endYear})";
//                }

//                var sortedData = groupedData.Values
//                    .OrderBy(x => x.BuyerName)
//                    .ThenBy(x => x.Style)
//                    .ThenBy(x => x.PurchaseOrder)
//                    .ToList();

//                // Assign SlNo
//                int slNo = 1;
//                for (int i = 0; i < sortedData.Count; i++)
//                {
//                    sortedData[i].SlNo = slNo++.ToString();
//                }

//                return new OrderReportStylePoResponse
//                {
//                    CompanyName = comRepo.All().Where(x => x.CompanyCode == companyCode).Select(s => s.CompanyName).FirstOrDefault() ?? "",
//                    ReportTitle = "Month Wise Order Booking Status",
//                    ReportYear = reportYear,
//                    Data = sortedData,
//                    //MonthColumns = allMonthKeys.ToList()
//                    MonthColumns = allMonthKeys.Select(m =>
//                    {
//                        var formats = new[] { "MMM-yy", "MMM-yyyy" };
//                        DateTime.TryParseExact(m, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsed);
//                        return new { Key = m, Date = parsed };
//                    }).OrderBy(x => x.Date).Select(x => x.Key).ToList()
//                };
//            }
//            catch (Exception)
//            {

//                throw;
//            }
//        }



//        // ============ SERVICE METHOD ============
//        public async Task<OrderReportStylePoCSResponse> GetOrderReportStylePoCSAsync(OrderReportRequest request, string companyCode)
//        {
//            using var connection = new SqlConnection(_connectionString);

//            var parameters = new DynamicParameters();
//            parameters.Add("@FromDate", request.FromDate, DbType.Date);
//            parameters.Add("@ToDate", request.ToDate, DbType.Date);
//            parameters.Add("@FromYear", request.FromYear, DbType.Int32);
//            parameters.Add("@ToYear", request.ToYear, DbType.Int32);
//            parameters.Add("@BuyerIds", request.BuyerIds?.Any() == true ? string.Join(",", request.BuyerIds) : null, DbType.String);

//            var result = await connection.QueryAsync<dynamic>("sp_GetOrderReport", parameters, commandType: CommandType.StoredProcedure);

//            var groupedData = new Dictionary<string, OrderReportDataStylePoCS>();
//            var allMonthKeys = new SortedSet<string>();

//            foreach (var row in result)
//            {
//                string buyerId = row.BuyerId?.ToString() ?? "";
//                string styleId = row.Style?.ToString() ?? "";
//                string purchaseOrder = row.PurchaseOrder?.ToString() ?? "";
//                string detailOrderId = row.DetailOrderId?.ToString() ?? "";

//                string key = $"{buyerId}_{styleId}_{purchaseOrder}_{detailOrderId}";

//                string buyerName = buyerRepo.All().Where(x => x.BuyerId == buyerId).Select(x => x.BuyerName).FirstOrDefault() ?? "";
//                string styleName = styleRepo.All().Where(x => x.StyleId == styleId).Select(e => e.Style).FirstOrDefault() ?? "";

//                if (!groupedData.ContainsKey(key))
//                {
//                    groupedData[key] = new OrderReportDataStylePoCS
//                    {
//                        BuyerName = buyerName,
//                        Style = styleName,
//                        Item = "",
//                        PurchaseOrder = purchaseOrder,
//                        OrderQuantity = row.DetailsQuantity?.ToString() ?? "",
//                        MonthlyData = new Dictionary<string, List<ColorSizeDetail>>()
//                    };
//                }

//                var dto = groupedData[key];

//                // Collect items
//                string currentItemId = row.ProductId?.ToString() ?? "";
//                string currentItem = itemRepo.All().Where(x => x.ItemId == currentItemId).Select(s => s.ItemName).FirstOrDefault() ?? "";
//                if (!string.IsNullOrEmpty(currentItem) && !dto.Item.Contains(currentItem))
//                {
//                    dto.Item = string.IsNullOrEmpty(dto.Item) ? currentItem : dto.Item + ", " + currentItem;
//                }

//                // Extract color, size, quantity
//                string colorId = row.ColorId?.ToString() ?? "";
//                string sizeId = row.SizeId?.ToString() ?? "";
//                string colorSizeQty = row.ColorSizeBreakupQuantity?.ToString() ?? "";

//                string colorName = colorRepo.All().Where(x => x.ColorId == colorId).Select(c => c.Color).FirstOrDefault() ?? "";
//                string sizeName = sizeRepo.All().Where(x => x.SizeId == sizeId).Select(s => s.Size).FirstOrDefault() ?? "";

//                // Process monthly data
//                var rowDict = (IDictionary<string, object>)row;
//                foreach (var kvp in rowDict)
//                {
//                    if (kvp.Key.Contains("-") && kvp.Key.Length >= 6)
//                    {
//                        allMonthKeys.Add(kvp.Key);

//                        if (kvp.Value != null &&
//                            !string.IsNullOrEmpty(kvp.Value.ToString()) &&
//                            decimal.TryParse(kvp.Value.ToString(), out var monthValue) &&
//                            monthValue > 0 &&
//                            !string.IsNullOrEmpty(colorSizeQty) &&
//                            decimal.TryParse(colorSizeQty, out var csQty) &&
//                            csQty > 0)
//                        {
//                            if (!dto.MonthlyData.ContainsKey(kvp.Key))
//                            {
//                                dto.MonthlyData[kvp.Key] = new List<ColorSizeDetail>();
//                            }

//                            dto.MonthlyData[kvp.Key].Add(new ColorSizeDetail
//                            {
//                                Color = colorName,
//                                Size = sizeName,
//                                Quantity = csQty.ToString()
//                            });
//                        }
//                    }
//                }
//            }

//            // Determine report year
//            string reportYear = "";
//            if (request.FromYear.HasValue && request.ToYear.HasValue)
//            {
//                reportYear = request.FromYear == request.ToYear ? $"({request.FromYear})" : $"({request.FromYear} - {request.ToYear})";
//            }
//            else if (request.FromDate.HasValue && request.ToDate.HasValue)
//            {
//                int startYear = request.FromDate.Value.Year;
//                int endYear = request.ToDate.Value.Year;
//                reportYear = startYear == endYear ? $"({startYear})" : $"({startYear} - {endYear})";
//            }

//            var sortedData = groupedData.Values.OrderBy(x => x.BuyerName).ThenBy(x => x.Style).ThenBy(x => x.PurchaseOrder).ToList();

//            return new OrderReportStylePoCSResponse
//            {
//                CompanyName = comRepo.All().Where(x => x.CompanyCode == companyCode).Select(s => s.CompanyName).FirstOrDefault() ?? "",
//                ReportTitle = "Month Wise Order Booking Status",
//                ReportYear = reportYear,
//                Data = sortedData,
//                //MonthColumns = allMonthKeys.ToList()
//                MonthColumns = allMonthKeys.Select(m =>
//                {
//                    var formats = new[] { "MMM-yy", "MMM-yyyy" };
//                    DateTime.TryParseExact(m, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsed);
//                    return new { Key = m, Date = parsed };
//                }).OrderBy(x => x.Date).Select(x => x.Key).ToList()
//            };
//        }



//        public async Task<List<StyleMaster>> GetStylesAsync()
//        {
//            using (var connection = new SqlConnection(_connectionString))
//            {
//                var query = "SELECT StyleId, StyleName FROM RMG_Master_Style WHERE IsActive = 1 ORDER BY StyleName";
//                var result = await connection.QueryAsync<StyleMaster>(query);
//                return result.ToList();
//            }
//        }

//        public async Task<List<ColorMaster>> GetColorsAsync()
//        {
//            using (var connection = new SqlConnection(_connectionString))
//            {
//                var query = "SELECT ColorId, ColorName FROM RMG_Master_Color WHERE IsActive = 1 ORDER BY ColorName";
//                var result = await connection.QueryAsync<ColorMaster>(query);
//                return result.ToList();
//            }
//        }

//        public async Task<List<SizeMaster>> GetSizesAsync()
//        {
//            using (var connection = new SqlConnection(_connectionString))
//            {
//                var query = "SELECT SizeId, SizeName FROM RMG_Master_Size WHERE IsActive = 1 ORDER BY SizeName";
//                var result = await connection.QueryAsync<SizeMaster>(query);
//                return result.ToList();
//            }
//        }

//        //Task<OrderReportResponse> IOrderReportService.GetOrderReportAsync(OrderReportRequest request)
//        //{
//        //    throw new NotImplementedException();
//        //}
//    }
//}
