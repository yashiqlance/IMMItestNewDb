using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class UserRetainConsultantViewModel
    {
        public int Id {  get; set; }
        public string ProfilePic { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StarRatingCount { get; set; }
        public string ReviewCount { get; set; }
        public string CommunicationLanguage { get; set; }
        public string TypeofService { get; set; }
    }
}
