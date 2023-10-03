using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class ConsultantApponintmentDetailByUserIdViewModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePic { get; set; }
        public string BelongCountry { get; set; }
        public string LanguageName { get; set; }
        public int AppointmentId { get; set; }
        public int ConsultantId { get; set;}
        public string UserRequestTypeName { get; set; }
        public DateOnly CreatedOn { get; set; }
        public string TypeOfService { get; set; }
        public string ApplyForCountry { get; set; }
        public DateTime BookingDate { get; set; }
        public int BookingTime { get; set; }
        public string SessionTitle { get; set; }
        public int BookingMinutes { get; set; }
        public List<ConsultantApponintmentDetailByUserIdViewModel> lstAppointmentDetails { get; set; }
    }
}
