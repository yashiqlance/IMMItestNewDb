using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class UserRequestViewModel
    {
        public string TOS { get; set; }
        public string AFC { get; set; }
        public int AppointmentId { get; set; }
        public int ConsultantId { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string AppointmentTime { get; set; }
        public int Status { get; set; }
        public List<UserRequestViewModel> lstUserRequestViewModels { get; set; }
    }
}
