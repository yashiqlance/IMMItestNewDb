using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class RetainConsultantDetails
    {
        public int Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string LicenceNumber { get; set; }

        public string Mobile { get; set; }

        public string MobileCountryCode { get; set; }

        public string DeviceToken { get; set; }

        public string DeviceType { get; set; }

        public string LanguageName { get; set; }

        public string CountryName { get; set; }

        public string ServiceName { get; set; }

        public double averageRating { get; set; }
      
        public decimal RetainAmount { get; set; }

    }

    public class consultantretainparam
    {
        public int UserId { get; set; }

        public int ConsultantId { get; set; }

        public int RetainTypeOfService { get; set; }

        public int RetainCountryForService { get; set; }

        public int RetainCommunicationLanguage { get; set; }

    }


    public class GetDocumentandPaymentDetails
    {
        public int ConsultantId { get; set; }
        public string ConsultantFirstName { get; set; }

        public string ConsultantLastName { get; set; }

        public string ConsultantEmail { get; set; }

        public string ConsultantLicenceNumber { get; set; }

        public string ConsultantMobile { get; set; }

        public string ConsultantMobileCountryCode { get; set; }

        public string ConsultantDeviceToken { get; set; }

        public string ConsultantDeviceType { get; set; }
        public int UserId { get; set; }
        public string UserFirstName { get; set; }

        public string UserLastName { get; set; }

        public string UserEmail { get; set; }


        public string UserMobile { get; set; }

        public string UserMobileCountryCode { get; set; }

        public string UserDeviceToken { get; set; }

        public string UserDeviceType { get; set; }


        public string LanguageName { get; set; }

        public string CountryName { get; set; }

        public string ServiceName { get; set; }
        public string CommunicationLanguage { get; set; }

        public string ImmigrationCountry { get; set; }

        public string TypeofServiceName { get; set; }

        public double averageRating { get; set; }

        public decimal RetainAmount { get; set; }
        public string PaymentTitle { get; set; }
       
        public string Filename { get; set; }
        public int UserDocumentId { get; set; }
         public byte Size { get; set; }
        public string Extensions { get; set; }


    }


    public class payretentionamountparam
    {
        public int UserId { get; set; }

        public int RetainId { get; set; }

        public int ConsultantId { get; set; }

        public int PaymentModeName { get; set; }

        public decimal RetainAmount { get; set; }
        
        public string Currency { get; set; }

    }

    public class GetRetainConsultnatListForUserViewModel
    {
        public int ConsultantId { get; set; }
        public int RetainID { get; set; }
        public string ConsultantName { get; set; }
        public string ProfilePic { get; set; }
        public string CometChatConsultantUID { get; set; }

        public string UserName { get; set; }
        public string CometChatUserUID { get; set; }
        public int UserId { get; set; }
    }

    public class RetainCList
    {
        public List<GetRetainConsultnatListForUserViewModel> lstGetRetainConsultnatListForUserViewModel { get; set; }
    }

    //public class payretentionamountemiparam
    //{


    //    public int RetainId { get; set; }



    //    public string PaymentModeName { get; set; }

    //    public long RetainAmount { get; set; }

    //    public string StripeCustomerId { get; set; }

    //    public string StripeCardId { get; set; }

    //    public int ForNumberOfEmi { get; set; }

    //    public long TotalAmount { get; set; }

    //    public long PaidAmount { get; set; }
    //    public int EMICount { get; set; }

    //    public decimal Emiamount { get; set; }
    //    public bool? IsActive { get; set; }


    //}
}
