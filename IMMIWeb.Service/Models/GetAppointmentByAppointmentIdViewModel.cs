using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class GetAppointmentByAppointmentIdViewModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set;}
        public string LastName { get; set;}
        public string ProfilePic { get; set;}
        public string CometChatUserUID { get; set;}
        public string ConsultantName { get; set;}
    }
}
