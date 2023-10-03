using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class ConsultantAppointmentByStatusViewModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePic { get; set; }
        public string CometChatUserUID { get; set; }
        public string LanguageName { get; set; }
        public int AppointmentId { get; set; }
        public int ConsultantId { get; set; }
        public string UserRequestTypeName { get; set; }
        public string TypeOfService { get; set; }
        public string CountryName { get; set; }
        public DateTime BookingDate { get; set; }
        public int BookingTime { get; set; }
        public string SessionTitle { get; set; }
        public int BookingMinutes { get; set; }
        public List<ConsultantAppointmentByStatusViewModel> lstconsultantAppointmentByStatus { get; set; }
    }
}
