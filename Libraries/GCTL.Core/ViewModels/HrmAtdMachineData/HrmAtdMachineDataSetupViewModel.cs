using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HrmAtdMachineData
{
    public class HrmAtdMachineDataSetupViewModel : BaseViewModel
    {
        public string FingerPrintId { get; set; }
        public string MachineId { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
    }
}
