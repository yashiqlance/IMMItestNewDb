using IMMIWeb.Service.Models;
using IMMIWeb.Service.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Service.User
{
    public interface IUserRepository : IGenericRepository<IMMIWeb.User>    
    {
        int AddUser(IMMIWeb.User user);
        bool IsUserCardExist(int UserId);
        int AddUserCard(IMMIWeb.UserCardsDetail userCardsDetail);

        public List<UserDocument> GetUserDocuments(int userId);

        int AddReview(RatingReviewConsultant ratingModel);

        int FetchReviewOfUser(int userId, int consultantId);

        int GetReviewIdForUserAndConsultant(int userId, int consultantId);

        int GetCardsCount(int id);

        IMMIWeb.UserCardsDetail GetUserCardsDetail(int id);
        List<UserCardsDetail> GetUserCards(int id, int cardId);
        void SetPrimaryCard(int cardId, int userId);

        List<UserCardsDetail> GetUserAllCardsDetail(int userId);

        //public IMMIWeb.User GetUserProfilePicById(int id);

        List<GetAvailableConsultantData> UserAppointmentRequestList(AvailableConsultantParam param);

        List<GetUpcomingConsulantantAppointment> UpcomingConsultant(UpcomingConsultantParam param);

        GetConsultantDetail GetConsultantDetails(int id,int UserId);
        IEnumerable<GetConsultantReviewViewModel> GetConsultantReview(int id, int startCnt, int endCnt);

        //public IEnumerable<UserRetainConsultantViewModel> GetUserRetainConsultantList();

        public GetUserDetail GetUserDetails(int id);

        int AddRatingReviewConsultant(RatingReviewConsultant param);

        List<ListofbusinessHoursConsultant> ConsultantBusinessHours(ConsultantDetailsParam param);
       int[] BookingAppointment(UserAppointmentBooking param);

       List<UserRatingReviewModel> ViewRatingReviewConsultant(int ConsultantId);

        List<GetBookedRetainHistoryConsultant> ListofBookedRetainCancelConsultant(listofbookretcanceclparam param);
        string CancelConsultation(int ConsultantId,int UserId, decimal Amount);

        List<CountryViewModel> GetBelongingCountry();
        List<LanguageViewModel> GetCommunicationLanguage();

        List<ImmiCountry> GetImmigrationCountry();

        List<TypeOfServiceViewModel> GetTypeofService(int CountryId);


        List<GetAgreementsModel> RetrieveAgreements(int UserRole);

        List<UserDocuments> ListofUserDocuments(int UserId);

        //int AddUserDocuments(UserAddDocument param);

        int RemoveUserDocuments(RemoveUserDocumentparam param);

        List<LanguageViewModel> GetApplicationLanguage();

        string RemoveUserCardsDetail(Removecarddetailparam param);

        UserProfileDetail GetUserProfileDetails(int UserId);

        int UpdateCountryLanguageService(UpdateCountryLanguageparam param);

        List<HelpFAQ> HelpFAQ();

        int ContactUs(ContactUsDetails param);

        int DeleteAccount(int UserId);

        int AddVerificationHandler(VerificationHandler verificationHandler);
        int UpdateVerificationHandler(VerificationHandler verificationHandler);
        VerificationHandler GetVerificationHandler(string mobileNum, string OTP);
        VerificationHandler GetVerificationHandlerById(int id);

        int AddFavouriteConsultant(int ConsultantId, int UserId);

        int RemoveFavouriteConsultant(int ConsultantId, int UserId);

        List<NotificationMaster> UserNotificationList(int UserId, DateTime date);

        int GetNotificationCount(int Id, DateTime date);
        List<NotificationMaster> GetNotificationMasters(int UserId);

        string GetCommetChatUserList(int consultantId);

    }
}
