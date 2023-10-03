using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class UserHomeViewModel
    {
        public List<AppointmentPendingViewModel> lstModelAppoitment { get; set; }
        public List<GetConsultantByTosClangCountryViewModel> lstModelConsultant { get; set; }
    }
}
