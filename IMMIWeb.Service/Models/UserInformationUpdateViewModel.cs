using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class UserInformationUpdateViewModel
    {
        public int UserId { get; set; }
        public int CommunicationLanguage { get; set; }
        public int TypeOfServiceName { get; set; }
        public int ImmigrationCountry { get; set; }
        public int ApplicationLanguage { get; set; }
        public string ApplicationLanguageName { get; set; }
        public List<CommonListViewModel> lstCountryByTypeOfService { get; set; }
        public int lstCountryByTypeOfServiceVal { get; set; }

        public List<CommonListViewModel> lstCountry { get; set; }
        public int lstCountryVal { get; set; }

        public List<CommonListViewModel> lstIsimmigrationCountry { get; set; }
        public string lstIsimmigrationCountryVal { get; set; }

        public List<CommonListViewModel> lstTypeOfService { get; set; }
        public int lstTypeOfServiceVal { get; set; }

        public List<CommonListViewModel> lstLanguage { get; set; }
        public int lstLanguageVal { get; set; }

        public List<CommonListViewModel> lstAppLanguage { get; set; }
        public int lstAppLanguageVal { get; set; }
    }
}
