using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? MobileCountryCode { get; set; }
        public string? Mobile { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? IsAgreement { get; set; }
        public int Country { get; set; }
        public int CommunicationLanguage { get; set; }
        public int TypeOfServiceName { get; set; }
        public int ImmigrationCountry { get; set; }
        public int ApplicationLanguage { get; set; }
        public int UserTypeVal { get; set; }
        public string Message { get; set; }
        public string MobileErrMsg { get; set; }
        public string EmailErrMsg { get; set; }
        public string ReturnCase { get; set; }
        public string OTP { get; set; }
        public IFormFile imageUploadAWS { get; set; }
        public string ProfilePicUpload { get; set; }
        public string UniqueId { get; set; }
        public string ImmigrationCountryName { get; set; }
        public string BelongCountryName { get; set; }


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
        public List<CommonListViewModel> lstMobile { get; set; }
        public string lstMobileVal { get; set; }
        public string? ProfilePic { get; set; }

        public GetUserDetail GetUserDetails { get; set; }

    }

    public class CreateCometChatUserparam
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        // Add more properties as needed
    }

    public class DeleteCometChatUserparam
    {
        public int UserId { get; set; }
    }

    public class DeleteCometChatConsultantparam
    {
        public int ConsultantId { get; set; }

        public string? CometChatConsultantUid { get; set; }
    }

    public class TimeZoneData
    {
        public string Timezone { get; set; }
    }

}
