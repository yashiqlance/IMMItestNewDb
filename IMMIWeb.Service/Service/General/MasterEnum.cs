using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Service.General
{
    public static class MasterEnum
    {
        public enum EUserType
        {
            Admin = 1,
            User = 2,
            Consultant = 3
        }
        public enum EDeviceType
        {
            Apple = 1,
            Android = 2,
            Web = 3
        }
        public enum EUserRequestType
        {
            Pending = 1,
            Approved = 2,
            Denied = 3
        }
        public  enum EUserRequestSessionType
        {
            Video = 1,
            Audio = 2,
            Chat = 3
        }
        public enum EAppointmentStatus
        {
            User_Request_For_Appointment = 1,
            Consultant_Appointment_Accepted = 2,
            Consultant_Appointment_Rejected = 3,
            Active = 4,
            Working = 5,
            Completed = 6,
            Canceled_By_User = 7,
            Canceled_By_Consultant = 8,
            User_Request_For_Money = 9,
            Technical_Issue = 10
        }
        public enum EPaymentStatus
        {
            Hold = 1,
            Success = 2,
            Refunded = 3,
            Cancelled = 4,
            Completed = 5,
            Pending = 6
        }
        public enum ESuspendedByUserType
        {
            Admin = 1,
            Systematic = 2
        }
        public enum EPaymentMode
        {
            FullSwipe = 1,
            EMI = 2
        }
    }
}
