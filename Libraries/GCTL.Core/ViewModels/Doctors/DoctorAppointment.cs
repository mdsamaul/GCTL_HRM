using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.Doctors
{
    public class DoctorAppointment : BaseViewModel
    {
        public string AppointmentCode { get; set; }
        public string AppointmentDays { get; set; }
        public string DoctorCode { get; set; }
        public string VisitingTimeFrom { get; set; }
        public string VisitingTimeTo { get; set; }
        public decimal VisitingFee { get; set; }
        public decimal ReportShowingFee { get; set; }

    }
}
