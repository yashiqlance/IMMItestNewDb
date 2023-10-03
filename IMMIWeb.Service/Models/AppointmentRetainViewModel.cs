using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class AppointmentRetainViewModel
    {        
        public int RetainId { get; set; }
        public int ConsultantId { get; set; }
        public string? FirstName { get; set; }        
        public string? LastName { get; set; }        
        public string? ProfilePic { get; set; }                        
        public string? Language { get; set; }
        public string? TypeofService { get; set; }
        public int AverageRating { get; set; }
        public int ReviewCount { get; set; }
      
    }
}
