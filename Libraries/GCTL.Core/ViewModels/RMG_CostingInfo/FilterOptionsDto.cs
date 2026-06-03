namespace GCTL.Core.ViewModels.RMG_CostingInfo
{
    // DTOs/FilterOptionsDto.cs
    public class FilterOptionsDto
    {
        public List<BuyerDto> Buyers { get; set; }
        public List<JobNoDto> JobNos { get; set; }
        public List<StyleDto> Styles { get; set; }
        public List<MasterPODto> MasterPOs { get; set; }
        public List<PurchaseOrderDto> PurchaseOrders { get; set; }
    }


    // DTOs/BuyerDto.cs
    public class BuyerDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    // DTOs/JobNoDto.cs
    public class JobNoDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    // DTOs/StyleDto.cs
    public class StyleDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    // DTOs/MasterPODto.cs
    public class MasterPODto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    // DTOs/PurchaseOrderDto.cs
    public class PurchaseOrderDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }


}
