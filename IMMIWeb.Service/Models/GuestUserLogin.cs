using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class GuestUserLogin
    {
        public List<CommonListViewModel> lstMobile { get; set; }  
        public string? FirstName { get; set; } 
        public string? LastName { get; set; }
        public string MobileCountryCode { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
       

    }
}
