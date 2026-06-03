namespace GCTL.Core.ViewModels.MenuTab
{
    public class MenuItemDto
    {
        public string MenuId { get; set; }
        public string? ParentId { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanPrint { get; set; }
    }
}