using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class UserProfileDetail
    {
        public int UserId { get; set; }
        public string? UserFirstName { get; set; }

        public string? UserLastName { get; set; }

        public string? UserEmail { get; set; }
      
        public string? UserMobile { get; set; }

        public string? UserMobileCountryCode { get; set; }

        public string? UserDeviceToken { get; set; }

        public string? UserDeviceType { get; set; }

        public string? CommunicationLanguage { get; set; }

        public string? ImmigrationCountry { get; set; }

        public string? TypeofServiceName { get; set; }

        public string? Country { get; set; }

        public string? CardName { get; set; }
        public int CardId { get; set; }
        public string? CardNumber { get; set; }
        public string? StripeCustomerId { get; set; }
        public string? StripeCardId { get; set; }
        public string? ExpMonth { get; set; }  
        public string? ExpYear { get; set; }

        public string ProfilePic { get; set; }

        public bool IsGuest { get; set; }
        public string UniqueId { get; set; }
    }

    public class GetUserDetail
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public string MobileCountryCode { get; set; }

        public string DeviceToken { get; set; }

        public string DeviceType { get; set; }

        public string ImmigrationCountry { get; set; }

        public string BelongCountryName { get; set; }
        public string Country { get; set; }

        public string TypeOfService { get; set; }

        public string LanguageName { get; set; }

        public string ProfilePic { get; set; }
        public string UniqueId { get; set; }

    }

    public class UpdateCountryLanguageparam
    {
        public int UserId { get; set; }

        public int ImmigrationCountryId { get; set; }

        public int CommunicationLanguageId { get; set; }

        public int TypeofService { get; set; }

        //public int BelongCountryId { get; set; }

    }

    public class HelpFAQ
    {
        public int Id { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }


        

    }

    public class ContactUsDetails
    {

        public string EmailId { get; set; } = null!;

        public string Subject { get; set; } = null!;

        public string? Description { get; set; }
    }


}
