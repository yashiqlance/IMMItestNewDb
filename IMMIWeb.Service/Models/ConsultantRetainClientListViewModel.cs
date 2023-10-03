using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class ConsultantRetainClientListViewModel
    {
        public int Id { get; set; }
        public int ConsultantId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsAct { get; set; }
        public int RetainTypeOfService { get; set; }
        public int RetainCountryForService { get; set; }
        public int RetainCommunicationLanguage { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePic { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string CountryCode { get; set; }
        public int BelongCountry { get; set; }

        public string ImmigrationCountryName { get; set; }
        public string BelongCountryName { get; set; }
        public string Language { get; set; }
        public string TypeofService { get; set; }

        public decimal ReatinAmount { get; set; }
        public decimal TaxCharge { get; set; }
        public decimal AppCharge { get; set; }
        public int PaymentMode { get; set; }

        public IEnumerable<UserDocumentViewModel> lstUserDocmument { get; set; }
    }


}
