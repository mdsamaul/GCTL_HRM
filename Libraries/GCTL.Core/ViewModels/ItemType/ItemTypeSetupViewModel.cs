using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.ItemType
{
    public class ItemTypeSetupViewModel : BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string ItemTypeId { get; set; }
        [Required(ErrorMessage = "Item Name is required")]
        public string ItemName { get; set; }
        public string CompanyId { get; set; }
        public string EmployeeId { get; set; }
    }
}
