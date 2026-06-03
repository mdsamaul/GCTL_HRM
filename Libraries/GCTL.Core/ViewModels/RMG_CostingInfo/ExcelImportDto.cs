namespace GCTL.Core.ViewModels.RMG_CostingInfo
{
    public class ExcelImportDto
    {
        public string Material { get; set; }
        public string Description { get; set; }
        public string Width { get; set; }
        public string Color { get; set; }
        public string Supplier { get; set; }
        public string PONo { get; set; }
        public decimal? OrdQty { get; set; }
        public decimal? QtyGmt { get; set; }
        public decimal? TtlQty { get; set; }
        public string Unit { get; set; }
        public decimal? UnitPrice { get; set; }
        public string ResponsibleBy { get; set; }
    }

}
