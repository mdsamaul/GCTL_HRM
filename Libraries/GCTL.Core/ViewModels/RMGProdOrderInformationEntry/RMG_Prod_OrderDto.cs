namespace GCTL.Core.ViewModels.RMGProdOrderInformationEntry
{
    public class RMG_Prod_OrderDto : BaseViewModel
    {
        public decimal? TC { get; set; }
        public string? OrderId { get; set; } = string.Empty;
        public DateTime? Date { get; set; } = null;
        public string? BuyerId { get; set; }
        public string? BuyerName { get; set; }
        public string? BuyerOrderNo { get; set; }
        public DateTime? BuyerOrderDate { get; set; } = null;
        public string? MasterPurchaseOrder { get; set; }
        public DateTime? MPO_Date { get; set; } = null;
        public string? SeasonId { get; set; }
        public string? SeasonName { get; set; }
        public string? SeasonYear { get; set; }
        public string? SupplierId { get; set; }
        public decimal? TotalOrderQuantity { get; set; }
        public string? TotalOrderQuantityDis { get; set; }
        public string? UnitTypID { get; set; }
        public string? UnitTyp { get; set; }
        public decimal? TotalPrice { get; set; }
        public string CurrencyId { get; set; } = string.Empty;
        public string? PaymentTerm { get; set; }
        public string? BuyerBankId { get; set; }
        public string? BuyerBranchId { get; set; }
        public string? CompanyOwnBankId { get; set; }
        public string? CompanyOwnBranchId { get; set; }
        public List<string>? BuContatPerson { get; set; }
        public string? BuDesignation1 { get; set; }
        public string? Buphone { get; set; }
        public string? BuEmail { get; set; }
        public string? MerContatPerson { get; set; }
        public string? MerDesignation1 { get; set; }
        public string? Merphone { get; set; }
        public string? MerEmail { get; set; }
        public string? BuyerDeclaration { get; set; }
        public string? InspectionInfo { get; set; }
        public string? Remarks { get; set; }
        //public string? LUser { get; set; }
        //public DateTime? LDate { get; set; }
        //public string? LIP { get; set; }
        //public string? LMAC { get; set; }
        //public DateTime? ModifyDate { get; set; }
        //public string? CompanyCode { get; set; }
        public string EmployeeID { get; set; } = string.Empty;
        public string? IntegraJOBNo { get; set; }
        public string? POStatusId { get; set; }
        public string? BuyerBrand { get; set; }
        public string? BuyerBrandName { get; set; }
        public string? StyleId { get; set; }
        public string? StyleName { get; set; }
        public DateTime? OrderDate { get; set; } = null;
        public string? BuyerSwiftCode { get; set; }
        public string? CompanySwiftCode { get; set; }
        public List<string>? MerchandiserContactId { get; set; }
        public string? StylePOWise { get; set; }
        public decimal? FOBAmount { get; set; }
        public string? FOBAmountDis { get; set; }
        public string? CurrencyId_FOB { get; set; }
        public string? CurrencyId_FOBDis { get; set; }
        public string? ShowCreateDate { get; set; }
        public string? ShowModifyDate { get; set; }
    }
}
