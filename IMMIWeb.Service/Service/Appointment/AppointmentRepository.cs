using IMMIWeb.Service.Models;
using IMMIWeb.Service.Repo;
using IMMIWeb.Service.Service.General;
using IMMIWeb.Service.Service.User;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using Twilio.Rest.Serverless.V1.Service.Asset;

namespace IMMIWeb.Service.Service.Appointment
{
    public class AppointmentRepository : GenericRepository<IMMIWeb.Appointment>, IAppointmentRepository
    {
        private IConfiguration Configuration;

        public AppointmentRepository(DbA976eeImmitestContext context, IConfiguration _configuration) : base(context)
        {
            Configuration = _configuration;
        }

        public int BookAppointment(IMMIWeb.Appointment appointment)
        {
            int returnId = 0;

            try
            {
                _dbContext.Appointments.Add(appointment);
                _dbContext.SaveChanges();
                returnId = appointment.Id;
            }
            catch (Exception e)
            {
                returnId = appointment.Id;
                throw;
            }
            return returnId;
        }

        public List<UserRequestViewModel> UserAppointmentRequestList(int counsultantId)
        {
            List<UserRequestViewModel> lstUserRequest = new List<UserRequestViewModel>();
            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                    ConsultantId = counsultantId,
                };

                lstUserRequest = connection.Query<UserRequestViewModel>("GetRequestedUserList", parameters, commandType: CommandType.StoredProcedure)
                    .Select(x => new UserRequestViewModel
                    {
                        TOS = x.TOS,
                        AFC = x.AFC,
                        AppointmentId = x.AppointmentId,
                        ConsultantId = x.ConsultantId,
                        UserId = x.UserId,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        AppointmentDate = x.AppointmentDate,
                        AppointmentTime = x.AppointmentTime
                    }).ToList();
            }

            return lstUserRequest;

        }

        public IEnumerable<AppointmentPendingViewModel> AppointmentPendingUser(int counsultantId)
        {

            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                    ConsultantId = counsultantId,
                };

                return connection.Query<AppointmentPendingViewModel>("AppointmentPendingRequestByConsultantId", parameters, commandType: CommandType.StoredProcedure)
                    .Select(x => new AppointmentPendingViewModel
                    {
                        AppointmentId = x.AppointmentId,
                        UserId = x.UserId,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        ProfilePic = x.ProfilePic,
                        LanguageName = x.LanguageName,
                        TypeOfService = x.TypeOfService,
                        CountryName = x.CountryName,
                        BookingDate = x.BookingDate,
                        //BookingTime = TimeZoneInfo.ConvertTimeFromUtc(x.BookingDate, TimeZoneInfo.Local).Hour,  // x.BookingTime,
                        //BookingMinutes = TimeZoneInfo.ConvertTimeFromUtc(x.BookingDate, TimeZoneInfo.Local).Minute,

                        //BookingDate = x.BookingDate,
                        //BookingTime = x.BookingTime,
                        SessionTitle = x.SessionTitle,
                        UserRequestTypeName = x.UserRequestTypeName,
                        CometChatUserUID = x.CometChatUserUID,
                        AppointmentStatusName = x.AppointmentStatusName,
                        rating = x.rating,
                        reviewCount = x.reviewCount,
                    });
            }
        }

        public List<AppointmentPendingViewModel> AppointmentPendingUserById(int UserId)
        {
            //var localtimezone = TimeZoneInfo.Local;
            //TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(localtimezone.StandardName);

            List<AppointmentPendingViewModel> lstAppointmentPendingViewModel = new List<AppointmentPendingViewModel>();

            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                    UserId = UserId,
                };

                lstAppointmentPendingViewModel = connection.Query<AppointmentPendingViewModel>("AppointmentPendingRequestByUserId", parameters, commandType: CommandType.StoredProcedure)
                    .Select(x => new AppointmentPendingViewModel
                    {
                        AppointmentId = x.AppointmentId,
                        UserId = x.UserId,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        ProfilePic = x.ProfilePic,
                        LanguageName = x.LanguageName,
                        TypeOfService = x.TypeOfService,
                        CountryName = x.CountryName,
                        BookingDate = x.BookingDate,
                        //BookingTime = TimeZoneInfo.ConvertTimeFromUtc(x.BookingDate, TimeZoneInfo.Local).Hour,  // x.BookingTime,
                        //BookingMinutes = TimeZoneInfo.ConvertTimeFromUtc(x.BookingDate, TimeZoneInfo.Local).Minute,
                        SessionTitle = x.SessionTitle,
                        UserRequestTypeName = x.UserRequestTypeName,
                        ConsultantName = x.ConsultantName,
                        ConsultantProfilePic = x.ConsultantProfilePic,
                        CometChatUserUID = x.CometChatUserUID
                    }).ToList();
            }
            return lstAppointmentPendingViewModel;
        }

        public List<AppointmentHistoryViewModel> AppointmentHistoryUserById(int UserId)
        {
            

            List<AppointmentHistoryViewModel> lstAppointmentHistoryViewModel = new List<AppointmentHistoryViewModel>();

            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                    UserId = UserId,
                };

                lstAppointmentHistoryViewModel = connection.Query<AppointmentHistoryViewModel>("AppointmentHistoryRequestByUserId", parameters, commandType: CommandType.StoredProcedure)
                    .Select(x => new AppointmentHistoryViewModel
                    {
                        AppointmentId = x.AppointmentId,                        
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        ProfilePic = x.ProfilePic,
                        ConsultantId = x.ConsultantId,                        
                        AppointmentDate = x.AppointmentDate,
                        //BookingTime = TimeZoneInfo.ConvertTimeFromUtc(x.AppointmentDate, TimeZoneInfo.Local).Hour,  // x.BookingTime,
                        //BookingMinutes = TimeZoneInfo.ConvertTimeFromUtc(x.AppointmentDate, TimeZoneInfo.Local).Minute,
                        CommunicationMode = x.CommunicationMode,
                        UserRequestTypeName = x.UserRequestTypeName,
                        Rating = x.Rating,
                        Review = x.Review,
                        Payment = x.Payment,
                        CallExtendedCount= x.CallExtendedCount
                    }).ToList();
            }
            return lstAppointmentHistoryViewModel;
        }

        public List<AppointmentRetainViewModel> AppointmentRetainUserById(int UserId)
        {


            List<AppointmentRetainViewModel> lstAppointmentRetainViewModel = new List<AppointmentRetainViewModel>();

            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                    UserId = UserId,
                };

                lstAppointmentRetainViewModel = connection.Query<AppointmentRetainViewModel>("AppointmentRetainRequestByUserId", parameters, commandType: CommandType.StoredProcedure)
                    .Select(x => new AppointmentRetainViewModel
                    {
                        RetainId = x.RetainId,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        ProfilePic = x.ProfilePic,
                        ConsultantId = x.ConsultantId,                       
                        AverageRating = x.AverageRating,
                        ReviewCount = x.ReviewCount,
                        Language = x.Language,
                        TypeofService = x.TypeofService
                    }).ToList();
            }
            return lstAppointmentRetainViewModel;
        }

        public RetainDetailsByUserViewModel RetainDetailsUserById(int UserId,int ConsultantId)
        {


            //List<RetainDetailsByUserViewModel> lstRetainDetailsUserViewModel = new List<RetainDetailsByUserViewModel>();

            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                    UserId = UserId,
                    ConsultantId = ConsultantId
                };

                var result = connection.Query<RetainDetailsByUserViewModel>("RetainDetailsByUser", parameters, commandType: CommandType.StoredProcedure)
                    .Select(x => new RetainDetailsByUserViewModel
                    {                        
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        ProfilePic = x.ProfilePic,
                        ConsultantId = x.ConsultantId,
                        AverageRating = x.AverageRating,
                        ReviewCount = x.ReviewCount,
                        UniqueId = x.UniqueId,
                        IsFavouriteConsultant = x.IsFavouriteConsultant,
                        CommunicationMode = x.CommunicationMode,
                        RetainDate = x.RetainDate,                                                
                        Filename = x.Filename,
                        Extension = x.Extension,
                        URL = x.URL,
                        TypeofService = x.TypeofService
                    }).FirstOrDefault();
                return result;

            }
            
        }

    }
}
