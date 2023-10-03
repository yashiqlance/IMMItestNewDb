using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class OTPVerificationViewModel
    {
        public string Message { get; set; }
        public string OTP { get; set; }
        public string GUID { get; set; }
        public string MobileCountryCode { get; set; }
        public string Mobile { get; set; }
        public string OTPRequestTime { get; set; }
        public System.Timers.Timer systemGlobalTime { get; set; }
    }
}
