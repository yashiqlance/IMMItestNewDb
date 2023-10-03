using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class RetainUploadDocumentViewModel
    {

        public int UserId { get; set; }
        public string UserUniqueId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserProfilePic { get; set; }
        public string UserMobileCountryCode { get; set; }
        public string UserMobile { get; set; }
        public string UserTypeOfService { get; set; }
        public string UserImmigrationCountry { get; set; }
        public string UserResidenceCountry { get; set; }
        public string UserLanguage { get; set; }
        public string UniqueId { get; set; }

        public string ErrorMessage { get; set; }

    }
}
