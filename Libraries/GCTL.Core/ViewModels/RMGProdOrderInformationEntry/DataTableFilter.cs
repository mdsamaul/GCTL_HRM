namespace GCTL.Core.ViewModels.RMGProdOrderInformationEntry
{
    public class DataTableFilter
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public string? SearchValue { get; set; }
        public string IntegraJobNo { get; set; }
        public string buyerId { get; set; }
    }

    public class PagedResult<T>
    {
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public List<T> Data { get; set; } = new();
    }
    public class OrderJobDto
    {
        public string PoId { get; set; }      // poId
        public string IJobNo { get; set; } // ijobno
    }

}
