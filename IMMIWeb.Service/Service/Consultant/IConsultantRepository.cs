using IMMIWeb.Service.Models;
using IMMIWeb.Service.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Service.Consultant
{
    public interface IConsultantRepository : IGenericRepository<IMMIWeb.Consultant>
    {
        IMMIWeb.Consultant IsValidConsultant(string mobileCountryCode,string mobile);
        int AddConsultant(IMMIWeb.Consultant consultant);
        public void AddConsultantSlotAsync(List<string> SlotDate,int ConsultantId);
        IEnumerable<CommonListViewModel> GetConsultantTypeOfServices(int ConsultantId);
        IEnumerable<CommonListViewModel> GetConsultantCountryForService(int ConsultantId);
        IEnumerable<CommonListViewModel> GetConsultantCommunicationLanguage(int ConsultantId);

        public IEnumerable<ConsultantRetainClientListViewModel> GetConsultantClientList(int Consultantid);

        public IEnumerable<ConsultantPaymentHistoryViewModel> GetPaymentHistoryList(int Consultantid);

        IEnumerable<string> GetConsultatnSlotDate(int ConsultantId, string Timezone);
        IEnumerable<SlotDetail> GetConsultatnSlotTime(int ConsultantId,string Date, string Timezone);        
        IMMIWeb.Consultant ValidConsultant(int ConsultantId);
        public bool BookConsultantSlot(int ConsultantSlotId,bool status);
        public DateTime GetConsultantSlotDate(int ConsultantSlotId);
        public IEnumerable<GetConsultantByTosClangCountryViewModel> GetConsultantByTosClangCountry(int tosId, int langId, int cntId, int startCnt, int endCnt, int userid,bool isGuestUser);
        public IEnumerable<GetFavouriteConsultantByUserIdViewModel> GetFavouriteConsultantByUserId(int UserId);
        public IEnumerable<ConsultantAppointmentByStatusViewModel> ConsultantAppointmentByStatus(int Consultantid, int StatusType);
        public IEnumerable<ConsultantApponintmentDetailByUserIdViewModel> ConsultantAppointmentDetailsByAppointmentId(int appointmentId);

        public ConsultantRetainClientListViewModel GetClientDetails(int userId);
        public bool UpdateConsultantAvailability(int userId, bool isAvailable);
        List<NotificationMaster> ConsultantNotificationList(int ConsultantId, DateTime date);
        //int GetNotificationCount(int ConsultantId);

        void UpdateConsultantSlotAsync(List<string> SlotDate, int ConsultantId);
        string[] ListofAvailableHoursConsultant(int ConsultantId);
        List<ConsultantSlot> ListofBusinessHoursConsultant(int ConsultantId, string SlotDate);

        void Updatecommunicationlanguage(string CommunicationLanguage, int ConsultantId);

        void Updatetypeofservice(string Typeofservice, int ConsultantId);

        bool Addhikeretentionamount(decimal RetentionAmount, int ConsultantId);

        bool DeleteAccount(int ConsultantId);

        List<NotificationMaster> PaymentHistoryList(int ConsultantId,decimal exchangerate);

        int AddWithdrawAmount(Withdraw withdraw);

        string GetCommetChatConsultnatList(int userId);

    }
}
