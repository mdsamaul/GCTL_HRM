using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.ProductIssueReport
{
    public class ProductIssueDropdownDto
    {
        public List<DropdownItem> CategoryList { get; set; } = new List<DropdownItem>();
        public List<DropdownItem> ProductList { get; set; } = new List<DropdownItem>();
        public List<DropdownItem> BrandList { get; set; } = new List<DropdownItem>();
        public List<DropdownItem> ModelList { get; set; } = new List<DropdownItem>();
        public List<DropdownItem> DepartmentList { get; set; } = new List<DropdownItem>();
        public List<DropdownItem> EmployeeList { get; set; } = new List<DropdownItem>();
        public List<DropdownItem> FloorList { get; set; } = new List<DropdownItem>();
    }
    public class DropdownItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
