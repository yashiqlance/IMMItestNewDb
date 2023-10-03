using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class UserAppointmentBooking
    {
        public int ConsultantId { get; set; }
        public int UserId { get; set; }
        public long Amount { get; set; }

        public string? StripeCustomerId { get; set; }

        public string? StripeCardId { get; set; }

        public string Currency { get; set; }
        public int? ConsultantSlotId { get; set; }
        public string CommunicationMode { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public int? ServiceType { get; set; }

        public int? ApplyForCountry { get; set; }
    }
    public class CommonInsertNotificationandSendNotificationparam
    {
        public int UserId { get; set; }

        public int ConsultantId { get; set; }
        public int NotificationTypeName { get; set; }

        public string Header { get; set; }

        public string Body { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }


    }
    public class InsertNotificationUserparam
    {
        public int NotificationTypeName { get; set; }

        public string Header { get; set; } = null!;

        public string Body { get; set; } = null!;

        public DateTime CreatedOn { get; set; }

        public int SenderId { get; set; }

        public int ReceiverId { get; set; }

        public int? SenderUserType { get; set; }

        public int? ReceiverUserType { get; set; }

        public bool? IsAct { get; set; }

    }

    public class CommonSendNotificationtoConsultantparam
    {
        public int UserId { get; set; }

        public int ConsultantId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
    }

    public class NotificationModel
    {
        [JsonProperty("deviceId")]
        public string DeviceId { get; set; }
        [JsonProperty("isAndroiodDevice")]
        public bool IsAndroiodDevice { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("body")]
        public string Body { get; set; }
    }

    public class GoogleNotification
    {
        public class DataPayload
        {
            [JsonProperty("title")]
            public string Title { get; set; }
            [JsonProperty("body")]
            public string Body { get; set; }
        }
        [JsonProperty("priority")]
        public string Priority { get; set; } = "high";
        [JsonProperty("data")]
        public DataPayload Data { get; set; }
        [JsonProperty("notification")]
        public DataPayload Notification { get; set; }
    }

    public class ResponseModel
    {
        [JsonProperty("isSuccess")]
        public bool IsSuccess { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }

}
