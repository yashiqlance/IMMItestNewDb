using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class GetConsultantByTosClangCountryViewModel
    {
        public int CurrentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TypeOfServiceName { get; set; }
        public string LanguageName { get; set; }
        public string CountryName { get; set; }
        public string ProfilePic { get; set; }
        public double AvgRating { get; set; }
        public int CountRating { get; set; }
        public int ConsultantRank { get; set; }
        public bool IsFavConsultantornot { get;set; }
    }
}
