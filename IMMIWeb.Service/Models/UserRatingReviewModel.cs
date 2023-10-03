using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class UserRatingReviewModel
    {
        public int? ConsultantId { get; set; }
        public int? UserId { get; set; }       
        public string? Review { get; set; }

        public int? Rating { get; set; }
    }
   

    

}
