namespace GCTL.Core.ViewModels.SalesDefInvMainItem
{
    public class DefInvStockLevelManagementDto : BaseViewModel
    {
        public int? TC { get; set; }
        public string? SLMID { get; set; }
        public string? ItemID { get; set; }
        public string? ItemName { get; set; }
        public string? WarehouseID { get; set; }
        public string? WarehouseName { get; set; }
        public decimal? InStock { get; set; }
        public decimal? StockValue { get; set; }
        public decimal? ReorderLevel { get; set; }
        public decimal? MaxStock { get; set; }
        public decimal? MinStock { get; set; }
        public string? Description { get; set; }
        public string? CompanyCode { get; set; }
        public string? EmployeeID { get; set; }
        public string? showCreateDate { get; set; }
        public string? showModifyDate { get; set; }
    }
}
