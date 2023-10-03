using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class AppointmentRequestViewModel
    {
        public string Name { get; set; }
        public string AppointmentDateAndTime { get; set; }
        public string TypeOfService { get; set; }
        public string ForCountryService { get; set; }
    }
}
