using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.BuyerDepartment
{
    public class InvBuyerDepartmentViewModel : BaseViewModel
    {
        public int Tc { get; set; }
        public string BuyerDepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string ShortName { get; set; }
    }
}
