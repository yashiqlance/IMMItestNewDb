using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class RetainDetailsByUserViewModel
    {                
        public int ConsultantId { get; set; }
        public string? FirstName { get; set; }        
        public string? LastName { get; set; }        
        public string? ProfilePic { get; set; }
        public string? UniqueId { get; set; }
        public int AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public bool IsFavouriteConsultant { get; set; }
        public string? CommunicationMode { get; set; }
        public DateTime RetainDate { get; set; }
        public string? TypeofService { get; set; }
        public string? Filename { get; set; }
        public string? URL { get; set; }
        public string? Extension { get; set; }
        //public string? TotalReview { get; set; }


    }
}
