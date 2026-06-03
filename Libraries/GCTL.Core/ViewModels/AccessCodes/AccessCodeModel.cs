namespace GCTL.Core.ViewModels.AccessCodes
{
    public class AccessCodeModel
    {
        public int AccessId { get; set; }
        public string AccessCodeId { get; set; }
        public string AccessCodeName { get; set; }
        public bool TitleCheck { get; set; }
        public string Title { get; set; }
        public string PageUrl { get; set; }
        public bool CheckAdd { get; set; }
        public bool CheckEdit { get; set; }
        public bool CheckDelete { get; set; }
        public bool CheckPrint { get; set; }
        public string ParentId { get; set; }
        public string MenuId { get; set; }
        public int OrderBy { get; set; }
        public string MenuText { get; set; }
        public string ControllerName { get; set; }
        public string ViewName { get; set; }
        public string Icon { get; set; }
        public bool IsActive { get; set; }

        public List<AccessCodeModel> Children { get; set; } = new List<AccessCodeModel>();
        public bool IsGreatGrandChild { get; set; }
    }
}
