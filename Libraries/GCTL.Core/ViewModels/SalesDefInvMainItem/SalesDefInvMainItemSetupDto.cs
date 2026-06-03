namespace GCTL.Core.ViewModels.SalesDefInvMainItem
{
    public class SalesDefInvMainItemSetupDto : BaseViewModel
    {
        public decimal? TC { get; set; }
        public string? MainItemID { get; set; }
        public string? MainItemName { get; set; }
        public string? Description { get; set; }
        public string? CompanyCode { get; set; }
        public string? EmployeeID { get; set; }
        public string? showCreateDate { get; set; }
        public string? showModifyDate { get; set; }
    }
}
