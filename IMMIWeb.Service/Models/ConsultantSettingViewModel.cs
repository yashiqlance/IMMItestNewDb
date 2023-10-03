using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class ConsultantSettingViewModel
    {
        public string? currentMobileCountryCode { get; set; }
        //public string? newMobileCountryCode { get; set; }
        public string? currentMobile { get; set; }
        //public string? newMobile { get; set; }
        public string? OTP { get; set; }
        public string lstMobileVal { get; set; }

        public List<CommonListViewModel> lstMobile { get; set; }

        public string? Email { get; set; }
        public string? Subject { get; set; }
        public string? description { get; set; }

        public string ErrorMsg { get; set; }
        public string EmailErrorMsg { get; set; }
        public string MobileErrorMsg { get; set; }

        public string GUID { get; set; }

        public string UserEmail { get; set; }

        public string OTPRequestTime { get; set; }
        public string otptimer { get; set; }

        public string ConsultantOTPVerification { get; set; }

        public string NewMobileCountryCode { get; set; }
        public string NewMobile { get; set; }

        public string RandomNumber { get; set; }
    }
}
