using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class AppointmentHistoryViewModel
    {        
        public int AppointmentId { get; set; }
        public int ConsultantId { get; set; }
        public string? FirstName { get; set; }        
        public string? LastName { get; set; }        
        public string? ProfilePic { get; set; }
        
        public DateTime AppointmentDate { get; set; }
        public int  BookingTime { get; set; }
        public string? CommunicationMode { get; set; }
        public int UserRequestTypeName { get; set; }
        public decimal Payment { get; set; }
        public int Rating { get; set; }
        public string Review { get; set; }
        public int CallExtendedCount { get; set; }

        public int BookingMinutes { get; set; }
    }
}
