using IMMIWeb.Service.Service.CMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class UserSettingViewModel
    {
        public string? currentMobileCountryCode { get; set; }
       // public string? newMobileCountryCode { get; set; }
        public string? currentMobile { get; set; }
       // public string? newMobile { get; set; } 
        public string? OTP { get; set; }
        public string lstMobileVal { get; set; }
        public List<CommonListViewModel> lstMobile { get; set; }

        public List<CommonListViewModel> lstCountry { get; set; }
        public int lstCountryVal { get; set; }

        public List<CommonListViewModel> lstCountryByTypeOfService { get; set; }
        public int lstCountryByTypeOfServiceVal { get; set; }

        public List<CommonListViewModel> lstIsimmigrationCountry { get; set; }
        public string lstIsimmigrationCountryVal { get; set; }

        public List<CommonListViewModel> lstTypeOfService { get; set; }
        public int lstTypeOfServiceVal { get; set; }

        public List<CommonListViewModel> lstLanguage { get; set; }
        public int lstLanguageVal { get; set; }

        public List<CommonListViewModel> lstHelp { get; set; }
        public int lstHelpVal { get; set; }


        public string? Email { get; set; }
        public string? Subject { get; set; }
        public string? description { get; set; }

        public string ErrorMsg { get; set; }
        public string EmailErrorMsg { get; set; }
        public string MobileErrorMsg { get; set; }

        public string RandomNumber { get; set; }
        public string NewRandomNumber { get; set; }

        public string NewMobileCountryCode { get; set; }
        public string NewMobile { get; set; }

        public string Message { get; set; }

        public string GUID { get; set; }

        public string UserEmail { get; set; }

        public string OTPRequestTime { get; set; }
        public string otptimer { get; set; }



        //public int Id { get; set; }
        //public string Description { get; set; } = null!;
        //public int Module { get; set; }
        //public bool IsActive { get; set; }
        //public int UserRole { get; set; }
        //public IEnumerable<Module> lstModules { get; set; }

    }
}
