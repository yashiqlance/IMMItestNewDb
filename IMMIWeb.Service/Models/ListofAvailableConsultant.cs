using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class AvailableConsultantParam
    {
        public int UserId { get; set; } = 0!;

        public int CommunicationLanguage { get; set; } = 0!;

        public int TypeofService { get; set; } = 0!;

        public int ImmigrationCountry { get; set; } = 0!;

        public int page { get; set; }

    }

    public class GetAvailableConsultantData
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
        public int reviewCount { get; set; }
        public bool IsFavConsultantornot { get; set; }

        public decimal AppointmentFees { get; set; }
                  
    }

    public class UpcomingConsultantParam
    {
        public int UserId { get; set; } = 0!;
    }


    public class GetUpcomingConsulantantAppointment
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string SessionTitle { get; set; }

        public int StartHour { get; set; }

        public int EndHour { get; set; }      
    }

    public class ConsultantDetailsParam
    {
        public int ConsultantId { get; set; } = 0!;

        public DateTime Date { get; set; }
    }

    public class GetConsultantDetail
    {
        public int ConsultantId { get; set; }
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

        public int reviewCount { get; set; }

        public int Rating { get; set; }

        public string Review { get; set; }
        public bool IsFavConsultantornot { get; set; }

        public decimal AppointmentFees { get; set; }
        public string ProfilePic { get; set; }

        public decimal RetainAmount { get; set; }

        public decimal ApplicationCharge { get; set; }

        public decimal TaxCharge { get; set; }
        public decimal RetainProcessCharges { get; set; }

        public decimal WithdrawAmount { get; set; }

        public decimal exchangerate { get; set; }

        public string UniqueId { get; set; }
    }

    public class ListofbusinessHoursConsultant
    {

        //public DateTime Date { get; set; }

        public int StartHour { get; set; }

        public int EndHour { get; set; }        

    }

    public class listofbookretcanceclparam
    {
        public int ConsultantId { get; set; }

        public int UserId { get; set; }

        public string Type { get; set; }
    }

    public class GetBookedRetainHistoryConsultant
    {
        public int ConsultantId { get; set; }
       
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string LicenceNumber { get; set; }

        public string Mobile { get; set; }

        public string MobileCountryCode { get; set; }

        public string DeviceToken { get; set; }

        public string DeviceType { get; set; }

        public string CommunicationMode { get; set; }

        public int StartHour { get; set; }

        public int EndHour { get; set; }

        public double averageRating { get; set; }
        public int reviewCount { get; set; }
        public bool IsFavConsultantornot { get; set; }

        public DateTime AppointmentDate { get; set; }

        public decimal Amount { get; set; }
        public decimal SessionStartTime { get; set; }
        public decimal SessionEndTime { get; set; }

        public int CancelledByUserTypeName { get; set; }
        public DateTime CancellationDate { get; set; }
        public int CancelledById { get; set; }
       
        public string LanguageName { get; set; }
        public string CountryName { get; set; }
        public string ServiceName { get; set; }
        public string Filename { get; set; }

        public int UserDocumentId { get;set; }
        public byte Size { get; set; }
        public string Extensions { get; set; }


    }

}
