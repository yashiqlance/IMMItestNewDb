using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class ChangeMobileViewModel
    {
        public string NewMobileCountryCode { get; set; }
        public string NewMobile { get; set; }
        public string RandomNumber { get; set; }
        public string NewRandomNumber { get; set; }
        public string OTP { get; set; }
        public string MobileCountryCode { get; set; }
        public string Mobile { get; set; }
        public string ErrorMsg { get; set; }
        public string EmailErrorMsg { get; set; }
        public string MobileErrorMsg { get; set; }
        public string Email { get; set; }
        public List<CommonListViewModel> lstMobile { get; set; }
        public string lstMobileVal { get; set; }
    }
}
