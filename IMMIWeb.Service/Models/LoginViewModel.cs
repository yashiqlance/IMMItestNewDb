using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class LoginViewModel
    {
        public List<CommonListViewModel> lstMobile { get; set; }

        public string MobileCountryCode { get; set; }
        public string Mobile { get; set; }
        public string lstMobileVal { get; set; }
        public string Message { get; set; }
        public string MessageFail { get; set; }

    }
}
