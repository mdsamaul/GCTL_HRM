using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.RMG_Prod_Def_UnitType
{
    public class RMG_Prod_Def_UnitTypeSetupViewModel : BaseViewModel
    {
        public int TC { get; set; }
        public string UnitTypId { get; set; }
        public string UnitTypeName { get; set; }
        public string ShortName { get; set; }
        public int? DecimalPlaces { get; set; }
        public string ShowCreateDate { get; set; }
        public string ShowModifyDate { get; set; }
    }
}
