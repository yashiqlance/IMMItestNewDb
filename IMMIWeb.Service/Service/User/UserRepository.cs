using Dapper;
using IMMIWeb.Service.Models;
using IMMIWeb.Service.Repo;
using IMMIWeb.Service.Service.Communication;
using IMMIWeb.Service.Service.Consultant;
using IMMIWeb.Service.Service.General;
using IMMIWeb.Service.Service.StripePay;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SendGrid.Helpers.Mail;
using Stripe;
using Stripe.FinancialConnections;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Twilio.TwiML.Voice;
using static System.Net.WebRequestMethods;
using Microsoft.AspNetCore.Http;



namespace IMMIWeb.Service.Service.User
{
    public class UserRepository : GenericRepository<IMMIWeb.User>, IUserRepository
    {
        private IConfiguration Configuration;
        IStripeRepository stripeRepository;
        //private IDbConnection connection;
        public UserRepository(DbA976eeImmitestContext context, IConfiguration _configuration, IStripeRepository _stripeRepository) : base(context)
        {
            Configuration = _configuration;
            // connection= _connection;
            stripeRepository = _stripeRepository;
        }

        public int AddUser(IMMIWeb.User user)
        {
            int returnId = 0;

            _dbContext.Add(user);
            _dbContext.SaveChanges();
            returnId = user.Id;

            return returnId;
        }

        public int AddUserCard(UserCardsDetail userCardsDetail)
        {
            int returnId = 0;

            try
            {
                _dbContext.UserCardsDetails.Add(userCardsDetail);
                _dbContext.SaveChanges();
                returnId = userCardsDetail.CardId;
            }
            catch (Exception e)
            {

                throw;
            }
            return returnId;
        }

        public UserCardsDetail GetUserCardsDetail(int id)
        {
            return _dbContext.UserCardsDetails.Where(x => x.Id == id && x.IsPrimary == true).FirstOrDefault();
        }

        public List<UserCardsDetail> GetUserAllCardsDetail(int userId)
        {
            return _dbContext.UserCardsDetails.Where(x => x.Id == userId).ToList();
        }

        //public IMMIWeb.User GetUserProfilePicById(int id)
        //{
        //    IMMIWeb.User user = _dbContext.Users.FirstOrDefault(x => x.Id == id);
        //    return user;
        //}

        public bool IsUserCardExist(int UserId)
        {
            bool isExist = false;

            var getUserCard = _dbContext.UserCardsDetails.Where(x => x.Id == UserId).FirstOrDefault();
            if (getUserCard != null)
            {
                isExist = true;
            }
            return isExist;

        }

        public List<GetAvailableConsultantData> UserAppointmentRequestList(AvailableConsultantParam param)
        {
            List<GetAvailableConsultantData> result = new List<GetAvailableConsultantData>();
            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {

                    CommunicationLanguage = param.CommunicationLanguage,
                    ImmigrationCountry = param.ImmigrationCountry,
                    TypeofService = param.TypeofService,
                    PageNumber = 10,
                    PageSize = param.page,
                    UserId = param.UserId,
                };

                result = connection.Query<GetAvailableConsultantData>("GetAvailableConsultant", parameters, commandType: CommandType.StoredProcedure)
                   .Select(x => new GetAvailableConsultantData
                   {
                       FirstName = x.FirstName,
                       LastName = x.LastName,
                       Email = x.Email,
                       LicenceNumber = x.LicenceNumber,
                       Mobile = x.Mobile,
                       MobileCountryCode = x.MobileCountryCode,
                       DeviceToken = x.DeviceToken,
                       DeviceType = x.DeviceType,
                       LanguageName = x.LanguageName,
                       CountryName = x.CountryName,
                       ServiceName = x.ServiceName,
                       averageRating = x.averageRating,
                       reviewCount = x.reviewCount,
                       IsFavConsultantornot = x.IsFavConsultantornot,
                       AppointmentFees = x.AppointmentFees,
                   }).ToList();

            }

            return result;

        }

        public List<GetUpcomingConsulantantAppointment> UpcomingConsultant(UpcomingConsultantParam param)
        {
            List<GetUpcomingConsulantantAppointment> result = new List<GetUpcomingConsulantantAppointment>();
            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                    UserId = param.UserId,
                };

                result = connection.Query<GetUpcomingConsulantantAppointment>("GetUpcomingConsultantAppointment", parameters, commandType: CommandType.StoredProcedure)
                   .Select(x => new GetUpcomingConsulantantAppointment
                   {
                       FirstName = x.FirstName,
                       LastName = x.LastName,
                       AppointmentDate = x.AppointmentDate,
                       SessionTitle = x.SessionTitle,
                       StartHour = x.StartHour,
                       EndHour = x.EndHour,
                   }).ToList();

            }

            return result;

        }

        public GetConsultantDetail GetConsultantDetails(int id, int UserId)
        {
            GetConsultantDetail result = new GetConsultantDetail();

            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                    ConsultantId = id,
                    UserId = UserId
                };

                result = connection.Query<GetConsultantDetail>("GetConsultantDetails", parameters, commandType: CommandType.StoredProcedure)
                   .Select(x => new GetConsultantDetail
                   {
                       FirstName = x.FirstName,
                       LastName = x.LastName,
                       Email = x.Email,
                       LicenceNumber = x.LicenceNumber,
                       RetainAmount = x.RetainAmount == null || x.RetainAmount == 0 ? x.RetainProcessCharges : x.RetainAmount,
                       Mobile = x.Mobile,
                       MobileCountryCode = x.MobileCountryCode,
                       DeviceToken = x.DeviceToken,
                       DeviceType = x.DeviceType,
                       LanguageName = x.LanguageName,
                       CountryName = x.CountryName,
                       ServiceName = x.ServiceName,
                       averageRating = x.averageRating,
                       reviewCount = x.reviewCount,
                       Review = x.Review,
                       Rating = x.Rating,
                       IsFavConsultantornot = x.IsFavConsultantornot,
                       AppointmentFees = x.AppointmentFees,
                       ProfilePic = x.ProfilePic,
                       Id = x.Id,
                       ApplicationCharge = x.ApplicationCharge,
                       TaxCharge = x.TaxCharge,
                       ConsultantId = x.ConsultantId,
                       WithdrawAmount = x.WithdrawAmount,
                       UniqueId = x.UniqueId,
                   }).FirstOrDefault();

            }

            return result;

        }

        public IEnumerable<GetConsultantReviewViewModel> GetConsultantReview(int id, int startCnt, int endCnt)
        {
            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                    ConsultantId = id,
                    StartCnt = startCnt,
                    EndCnt = endCnt
                };

                return connection.Query<GetConsultantReviewViewModel>("GetConsultantReview", parameters, commandType: CommandType.StoredProcedure)
                   .Select(x => new GetConsultantReviewViewModel
                   {
                       Id = x.Id,
                       FirstName = x.FirstName,
                       LastName = x.LastName,
                       ProfilePic = x.ProfilePic,
                       Rating = x.Rating,
                       Review = x.Review,
                       CreatedOn = x.CreatedOn,
                       ConsultantId = x.ConsultantId,
                       Rank = x.Rank
                   });
            }
        }

        //public IEnumerable<UserRetainConsultantViewModel> GetUserRetainConsultantList()
        //{
        //    List<UserRetainConsultantViewModel> retainlist = new List<UserRetainConsultantViewModel>();

        //    var retainList = (from Consultant in _dbContext.Consultants
        //                      join retain in _dbContext.Retains on Consultant.Id equals retain.Id into retainGroup
        //                      from retain in retainGroup.DefaultIfEmpty()
        //                      where retain.UserId == )
        //}


        public int AddRatingReviewConsultant(RatingReviewConsultant param)
        {
            int returnId = 0;

            try
            {
                _dbContext.RatingReviewConsultants.Add(param);
                _dbContext.SaveChanges();
                returnId = param.Id;
            }
            catch (Exception e)
            {

                throw;
            }
            return returnId;
        }

        public List<ListofbusinessHoursConsultant> ConsultantBusinessHours(ConsultantDetailsParam param)
        {
            List<ListofbusinessHoursConsultant> result = new List<ListofbusinessHoursConsultant>();

            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                    ConsultantId = param.ConsultantId,
                    Date = param.Date,
                };

                result = connection.Query<ListofbusinessHoursConsultant>("ConsultantBusinessHours", parameters, commandType: CommandType.StoredProcedure)
                   .Select(x => new ListofbusinessHoursConsultant
                   {
                       //Date = x.Date,
                       StartHour = x.StartHour,
                       EndHour = x.EndHour,
                   }).ToList();

            }

            return result;

        }

        public int[] BookingAppointment(UserAppointmentBooking param)
        {
            var returnId = new int[2];
            var stripecharge = new Stripe.Charge();
            var AppointmentPaymentData = new AppointmentPayment();
            var AppointmentData = new IMMIWeb.Appointment();
            StripeConfiguration.ApiKey = Configuration["Integration:key"];

            try
            {



                var option = new ChargeCreateOptions
                {
                    Amount = param.Amount * 100,
                    Currency = param.Currency,

                    Source = param.StripeCardId,
                    Customer = param.StripeCustomerId,
                };

                var services = new ChargeService();
                stripecharge = services.Create(option);


                if (stripecharge.Status == "succeeded")
                {

                    using (var _context = new DbA976eeImmitestContext())
                    {


                        AppointmentData.UserId = param.UserId;
                        AppointmentData.ConsultantSlotId = param.ConsultantSlotId;
                        AppointmentData.CreatedOn = Convert.ToDateTime(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                        //AppointmentData.IsUpcoming = true;

                        AppointmentData.UserRequestTypeName = 1;
                        AppointmentData.SessionMode = param.CommunicationMode == "Chat" ? 1 : param.CommunicationMode == "Audio" ? 2 : param.CommunicationMode == "Video" ? 3 : 0;

                        AppointmentData.AppointmentStatusName = 1;
                        DateTime utcDateTime = DateTime.SpecifyKind((DateTime)param.AppointmentDate, DateTimeKind.Utc);
                        AppointmentData.AppointmentDate = utcDateTime;
                        AppointmentData.ServiceType = param.ServiceType;
                        AppointmentData.ApplyForCountry = param.ApplyForCountry;
                        _context.Appointments.Add(AppointmentData);
                        returnId[0] = _context.SaveChanges();
                    }

                    using (var _context = new DbA976eeImmitestContext())
                    {

                        (from p in _context.ConsultantSlots
                         where p.ConsultantId == param.ConsultantId && p.Id == param.ConsultantSlotId
                         select p).ToList()
                                                        .ForEach(x =>
                                                        {
                                                            x.IsActive = true;
                                                        });
                        _context.SaveChanges();
                    }

                    using (var _dbContext = new DbA976eeImmitestContext())
                    {

                        var AppointmentID = _dbContext.Appointments.Where(d => d.UserId == param.UserId).Select(r => r.Id).Single();


                        AppointmentPaymentData.AppointmentId = AppointmentID;
                        AppointmentPaymentData.PaymentStatusName = 1;
                        AppointmentPaymentData.TransactionId = stripecharge.Id;
                        AppointmentPaymentData.IsPayment = true;
                        AppointmentPaymentData.IsAct = true;
                        AppointmentPaymentData.CreatedOn = Convert.ToDateTime(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                        AppointmentPaymentData.ConsultantId = param.ConsultantId;
                        AppointmentPaymentData.UserId = param.UserId;
                        AppointmentPaymentData.Amount = param.Amount;
                        AppointmentPaymentData.StripeCustomerId = param.StripeCustomerId;
                        AppointmentPaymentData.StripeCardId = param.StripeCardId;

                        _dbContext.AppointmentPayments.Add(AppointmentPaymentData);
                        returnId[1] = _dbContext.SaveChanges();
                    }

                    CommonInsertNotificationandSendNotificationparam paramnot = new CommonInsertNotificationandSendNotificationparam();

                    paramnot.Header = "Appointment Request";
                    paramnot.Body = "Appointment for New User Login";
                    paramnot.Title = "Advenuss";
                    paramnot.Description = "Appointment for New User Login";
                    paramnot.UserId = param.UserId;
                    paramnot.ConsultantId = param.ConsultantId;
                    paramnot.NotificationTypeName = 6;


                    Send.CommonInsertNotificationandSendNotification(paramnot);

                    // CommonInsertNotificationandSendNotification(paramnot);

                }


            }
            catch (Exception e)
            {

                throw;
            }
            return returnId;
        }


        //public void CommonInsertNotificationandSendNotification(CommonInsertNotificationandSendNotificationparam compar)
        //{
        //    var paramnot = new NotificationMaster();

        //    using (var _context = new DbA976eeImmitestContext())
        //    {
        //        paramnot.SenderId = compar.UserId;
        //        paramnot.ReceiverId = compar.ConsultantId;
        //        paramnot.Header = compar.Header;

        //        //  paramnot.Body = EnumExtensions.GetEnumDescription(NotificationDescription.AppointmentCancel);
        //        paramnot.Body = compar.Body;
        //        paramnot.CreatedOn = Convert.ToDateTime(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        //        paramnot.SenderUserType = 2;
        //        paramnot.ReceiverUserType = 3;
        //        //paramnot.NotificationTypeName = 10;
        //        paramnot.NotificationTypeName = compar.NotificationTypeName;
        //        paramnot.IsAct = true;
        //        _context.NotificationMasters.Add(paramnot);
        //        _context.SaveChanges();
        //    }




        //    //InsertNotificationUser(paramnot);

        //    CommonSendNotificationtoConsultantparam paramconsnot = new CommonSendNotificationtoConsultantparam();
        //    // paramconsnot.Title = "IMMI";
        //    // paramconsnot.Description = "Appointment Canceled By User";

        //    paramconsnot.Title = compar.Title;
        //    paramconsnot.Description = compar.Description;
        //    paramconsnot.UserId = compar.UserId;
        //    paramconsnot.ConsultantId = compar.ConsultantId;

        //    CommonSendNotificationtoConsultant(paramconsnot);

        //}



        //public void CommonSendNotificationtoConsultant(CommonSendNotificationtoConsultantparam param)
        //{
        //     List<NotificationModel> objList = new List<NotificationModel>();
        //    //JsonResult result;
        //    if (param.UserId != 0 && param.ConsultantId != 0)
        //    {

        //        var Listofconsultant = _dbContext.Consultants.Where(d => d.Id == param.ConsultantId && d.IsAdminApproved == true && d.IsSuspended != true && d.IsAvailable == true).Select(r => new
        //        {
        //            r.DeviceToken,
        //            r.DeviceType,
        //        }).ToList();

        //        if (Listofconsultant.Count > 0)
        //        {
        //            NotificationModel obj = Common.SetNotificationModel(param.Title, param.Description, Listofconsultant[0].DeviceToken, Listofconsultant[0].DeviceType);


        //            Utility.SendNotification(obj);



        //        }

        //    }


        //}


        public List<UserRatingReviewModel> ViewRatingReviewConsultant(int ConsultantId)
        {
            var ViewReviewsDetails = _dbContext.RatingReviewConsultants.Where(m => m.ConsultantId == ConsultantId).ToList();

            var MapReviewDetail = ViewReviewsDetails.Select(u => new UserRatingReviewModel
            {
                ConsultantId = u.ConsultantId,
                UserId = u.UserId,
                Review = u.Review,
                Rating = u.Rating
            }).ToList();

            return MapReviewDetail;
        }


        public List<GetBookedRetainHistoryConsultant> ListofBookedRetainCancelConsultant(listofbookretcanceclparam param)
        {
            List<GetBookedRetainHistoryConsultant> result = new List<GetBookedRetainHistoryConsultant>();
            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                    ConsultantId = param.ConsultantId,
                    Type = param.Type,
                    UserId = param.UserId
                };

                result = connection.Query<GetBookedRetainHistoryConsultant>("GetBookedRetainHistoryConsultant", parameters, commandType: CommandType.StoredProcedure)
                   .Select(x => new GetBookedRetainHistoryConsultant
                   {
                       FirstName = x.FirstName,
                       LastName = x.LastName,
                       Email = x.Email,
                       LicenceNumber = x.LicenceNumber,
                       Mobile = x.Mobile,
                       MobileCountryCode = x.MobileCountryCode,
                       DeviceToken = x.DeviceToken,
                       DeviceType = x.DeviceType,
                       LanguageName = x.LanguageName,
                       CountryName = x.CountryName,
                       ServiceName = x.ServiceName,
                       averageRating = x.averageRating,
                       reviewCount = x.reviewCount,
                       IsFavConsultantornot = x.IsFavConsultantornot,
                       StartHour = x.StartHour,
                       EndHour = x.EndHour,
                       AppointmentDate = x.AppointmentDate,
                       Amount = x.Amount,
                       SessionStartTime = x.SessionStartTime,
                       SessionEndTime = x.SessionEndTime,
                       CancelledByUserTypeName = x.CancelledByUserTypeName,
                       CancellationDate = x.CancellationDate,
                       CancelledById = x.CancelledById,
                       Filename = x.Filename,
                       UserDocumentId = x.UserDocumentId,
                       Size = x.Size,
                       Extensions = x.Extensions,

                   }).ToList();

            }
            return result;
        }

        public string CancelConsultation(int ConsultantId, int UserId, decimal Amount)
        {
            //var refund = new Refund();
            // var stripecharge = new Stripe.Charge();
            string result = string.Empty;
            StripeConfiguration.ApiKey = Configuration["Integration:key"];

            var ListofUserSlot = (from p in _dbContext.Appointments
                                  join t in _dbContext.AppointmentPayments on p.Id equals t.AppointmentId
                                  where p.UserId == UserId && p.ConsultantId == ConsultantId
                                  select new
                                  {
                                      p,
                                      t,

                                  }).ToList();

            if (ListofUserSlot.Count > 0)
            {
                ListofUserSlot.ForEach(x =>
                {
                    x.p.AppointmentStatusName = 2;
                    x.p.CancelledByUserTypeName = 2;
                    x.p.CancelledById = UserId;
                    x.p.CancellationDate = Convert.ToDateTime(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                    //x.p.IsUpcoming = false;
                });
                _dbContext.SaveChanges();

                DateTime firstDate = (DateTime)ListofUserSlot[0].p.AppointmentDate;

                try
                {
                    if (firstDate < DateTime.Now.Subtract(TimeSpan.FromHours(24)))
                    {
                        double percentage = 80;
                        long DownPay = (long)(ListofUserSlot[0].t.Amount);
                        long Amount1 = (long)(DownPay * (percentage / 100));
                        result = stripeRepository.RefundPayment(Amount1, ListofUserSlot[0].t.TransactionId);




                        //var option = new RefundCreateOptions
                        //{
                        //    Amount = (long?)(DownPay * (percentage / 100) * 100),
                        //    Charge = ListofUserSlot[0].t.TransactionId
                        //};
                        //var services = new RefundService();
                        //refund = services.Create(option);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                CommonInsertNotificationandSendNotificationparam paramnot = new CommonInsertNotificationandSendNotificationparam();

                paramnot.Header = "Appointment Cancel";
                paramnot.Body = "Appointment Canceled By User";
                paramnot.Title = "Advenuss";
                paramnot.Description = "Appointment Canceled By User";
                paramnot.UserId = UserId;
                paramnot.ConsultantId = ConsultantId;
                paramnot.NotificationTypeName = 11;

                Send.CommonInsertNotificationandSendNotification(paramnot);

                //CommonInsertNotificationandSendNotification(paramnot);
            }
            return result;
        }


        public List<CountryViewModel> GetBelongingCountry()
        {
            var MapReviewDetail = new List<CountryViewModel>();

            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                };

                MapReviewDetail = connection.Query<CountryViewModel>("GetBelongingCountry", parameters, commandType: CommandType.StoredProcedure)
                   .Select(x => new CountryViewModel
                   {
                       Id = x.Id,
                       Name = x.Name,
                       MobileCode = x.MobileCode

                   }).ToList();

            }



            var BelongingCountryListFromDB = _dbContext.Countries.ToList();

            if (BelongingCountryListFromDB.Count > 0)
            {
                MapReviewDetail = BelongingCountryListFromDB.Select(u => new CountryViewModel
                {
                    Id = u.Id,
                    Name = u.Name,
                    MobileCode = u.MobileCode
                }).ToList();
            }



            return MapReviewDetail;
        }

        public List<LanguageViewModel> GetCommunicationLanguage()
        {
            var MapReviewDetail = new List<LanguageViewModel>();

            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                };

                MapReviewDetail = connection.Query<LanguageViewModel>("GetCommunicationLanguage", parameters, commandType: CommandType.StoredProcedure)
                   .Select(x => new LanguageViewModel
                   {
                       Id = x.Id,
                       Name = x.Name

                   }).ToList();

            }
            return MapReviewDetail;
        }

        public List<ImmiCountry> GetImmigrationCountry()
        {
            var MapReviewDetail = new List<ImmiCountry>();

            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                };

                MapReviewDetail = connection.Query<ImmiCountry>("GetImmigrationCountry", parameters, commandType: CommandType.StoredProcedure)
                   .Select(x => new ImmiCountry
                   {
                       Id = x.Id,
                       Name = x.Name,

                   }).ToList();

            }

            return MapReviewDetail;
        }

        public List<TypeOfServiceViewModel> GetTypeofService(int CountryId)
        {
            var MapReviewDetail = new List<TypeOfServiceViewModel>();

            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                };

                MapReviewDetail = connection.Query<TypeOfServiceViewModel>("GetTypeofService", parameters, commandType: CommandType.StoredProcedure)
                   .Select(x => new TypeOfServiceViewModel
                   {
                       Id = x.Id,
                       Name = x.Name,
                       IsActive = x.IsActive
                   }).ToList();

            }

            return MapReviewDetail;
        }

        public List<GetAgreementsModel> RetrieveAgreements(int UserRole)
        {
            var GetAgreements = new List<GetAgreementsModel>();

            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                    UserRole = UserRole,
                };

                var MapReviewDetail = (List<RetrieveAgreementsModel>)connection.Query<RetrieveAgreementsModel>("GetCMSContent", parameters, commandType: CommandType.StoredProcedure);

                GetAgreements = MapReviewDetail.Select(x => new GetAgreementsModel
                {
                    AboutUs = x.Module == 1 ? x.Description : "",
                    TermsandConditions = x.Module == 2 ? x.Description : "",
                    PrivacyPolicy = x.Module == 3 ? x.Description : "",
                    Agreements = x.Module == 4 ? x.Description : "",
                    UserRole = x.UserRole
                }).ToList();

            }

            return GetAgreements;
        }

        public List<UserDocuments> ListofUserDocuments(int UserId)
        {
            var MapReviewDetail = new List<UserDocuments>();

            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                    UserId = UserId
                };

                MapReviewDetail = connection.Query<UserDocuments>("GetUserDocuments", parameters, commandType: CommandType.StoredProcedure)
                   .Select(x => new UserDocuments
                   {
                       Id = x.Id,
                       Filename = x.Filename,
                       Size = x.Size,
                       Extensions = x.Extensions

                   }).ToList();

            }

            return MapReviewDetail;
        }


        //public int AddUserDocuments(UserAddDocument param)
        //{
        //    int returnId = 0;
        //    var records = new UserDocument();

        //    try
        //    {
        //        records.UserId = param.UserId;
        //        records.Filename = param.Filename;
        //        records.Size = param.Size;
        //        records.Extensions = param.Extensions;

        //        records.CreatedOn = Convert.ToDateTime(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        //        records.IsActive = true;


        //        _dbContext.UserDocuments.Add(records);
        //        _dbContext.SaveChanges();

        //        CommonInsertNotificationandSendNotificationparam paramnot = new CommonInsertNotificationandSendNotificationparam();

        //        paramnot.Header = "Add/Edit Documents";
        //        paramnot.Body = "User Add/Edit Documents";
        //        paramnot.Title = "Advenuss";
        //        paramnot.Description = "User Add/Edit Documents";
        //        paramnot.UserId = param.UserId;
        //        paramnot.ConsultantId = param.ConsultantId;
        //        paramnot.NotificationTypeName = 11;

        //        Send.CommonInsertNotificationandSendNotification(paramnot);

        //        // CommonInsertNotificationandSendNotification(paramnot);

        //        returnId = records.Id;
        //    }
        //    catch (Exception e)
        //    {

        //        throw;
        //    }
        //    return returnId;
        //}

        public int RemoveUserDocuments(RemoveUserDocumentparam param)
        {
            int returnId = 0;

            var DocumentList = (from p in _dbContext.UserDocuments
                                where p.Id == param.UserId && p.Filename == param.Filename && p.Extensions == param.Extensions
                                select p).ToList();


            if (DocumentList.Count > 0)
            {
                _dbContext.UserDocuments.RemoveRange(DocumentList);
                returnId = _dbContext.SaveChanges();

            }

            return returnId;

        }

        public List<LanguageViewModel> GetApplicationLanguage()
        {
            var MapReviewDetail = new List<LanguageViewModel>();

            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                };

                MapReviewDetail = connection.Query<LanguageViewModel>("GetCommunicationLanguage", parameters, commandType: CommandType.StoredProcedure)
                   .Select(x => new LanguageViewModel
                   {
                       Id = x.Id,
                       Name = x.Name,
                       IsActive = x.IsActive
                   }).ToList();

            }
            return MapReviewDetail;
        }


        public string RemoveUserCardsDetail(Removecarddetailparam param)
        {
            string returnId = string.Empty;
            var records = new UserDocument();
            var deletecard = new Stripe.Card();

            try
            {
                var UserCardsDetails = (from d in _dbContext.UserCardsDetails
                                            //join c in _dbContext.AppointmentPayments on d.CardId equals c.CardId
                                            //where c.UserId == param.id && c.IsAct == true
                                            //select d).ToList();
                                        where d.Id == param.id && d.CardId == param.Cardid && d.IsPrimary != true
                                        select d).ToList();


                if (UserCardsDetails.Count > 0 /*&& UserCardsDetails.Count == 1*/)
                {
                    if ((bool)!UserCardsDetails[0].IsPrimary)
                    {
                        _dbContext.UserCardsDetails.RemoveRange(UserCardsDetails);
                        _dbContext.SaveChanges();


                        var service = new Stripe.CardService();
                        deletecard = service.Delete(
                          UserCardsDetails[0].StripeCustomerId,
                          UserCardsDetails[0].StripeCardId);
                    }
                }
                else
                {
                    //
                }



                returnId = deletecard.Id;
            }
            catch (Exception e)
            {

                throw;
            }
            return returnId;
        }

        public List<UserCardsDetail> GetUserCards(int id, int cardId)
        {
            try
            {
                var userCards = _dbContext.UserCardsDetails
                    .Where(c => c.CardId == cardId)
                    .ToList();

                return userCards;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void SetPrimaryCard(int cardId, int userId)
        {
            try
            {
                var UserCardsDetails = (from d in _dbContext.UserCardsDetails
                                        where d.Id == userId
                                        select d).ToList();

                if (UserCardsDetails.Count > 0)
                {
                    var stripcardid = UserCardsDetails.Select(x => x.StripeCardId).ToArray();
                    //var EMIList =
                    //        (from s in _dbContext.Emitables
                    //         join r in _dbContext.UserCardsDetails on
                    //          s.StripeCardId equals
                    //          r.StripeCardId
                    //         select s).ToList();

                    //if (EMIList.Count > 0)
                    //{
                    //    //This card contain EMI.Please select another card.
                    //}
                    //else
                    //{
                    var updatedRows = UserCardsDetails
                   .GroupBy(row => row.Id)
                   .SelectMany(group => group.Select((row, index) =>
                   {
                       row.IsPrimary = (index == 0) ? false : false;
                       return row;
                   }))
                   .ToList();
                    _dbContext.SaveChanges();
                    var cardToSetAsPrimary = updatedRows.FirstOrDefault(r => r.CardId == cardId && r.Id == userId);
                    if (cardToSetAsPrimary != null)
                    {
                        cardToSetAsPrimary.IsPrimary = true;

                        foreach (var card in updatedRows.Where(r => r.CardId == cardId && r.Id != userId))
                        {
                            card.IsPrimary = false;
                        }

                        _dbContext.SaveChanges();
                        //}
                        //var updatedrecords = updatedRows.Where(r => r.CardId == cardId && r.Id == userId).ToList();
                        //updatedrecords.ForEach(s => s.IsPrimary = true);
                        //_dbContext.SaveChanges();
                    }

                }
                else
                {
                    //Issue in Add Primary Card
                }
            }
            catch (Exception ex) { throw; }
        }

        public UserProfileDetail GetUserProfileDetails(int UserId)
        {
            var MapReviewDetail = new UserProfileDetail();

            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                    UserId = UserId
                };

                MapReviewDetail = connection.Query<UserProfileDetail>("GetUserProfileDetails", parameters, commandType: CommandType.StoredProcedure)
                   .Select(x => new UserProfileDetail
                   {
                       UserId = x.UserId,
                       UserFirstName = x.UserFirstName,
                       UserLastName = x.UserLastName,
                       UserEmail = x.UserEmail,
                       UserMobile = x.UserMobile,
                       UserMobileCountryCode = x.UserMobileCountryCode,
                       UserDeviceToken = x.UserDeviceToken,
                       UserDeviceType = x.UserDeviceType,
                       CommunicationLanguage = x.CommunicationLanguage,
                       ImmigrationCountry = x.ImmigrationCountry,
                       TypeofServiceName = x.TypeofServiceName,
                       Country = x.Country,
                       CardName = x.CardName,
                       CardId = x.CardId,
                       CardNumber = x.CardNumber,
                       StripeCustomerId = x.StripeCustomerId,
                       StripeCardId = x.StripeCardId,
                       ExpMonth = x.ExpMonth,
                       ExpYear = x.ExpYear,
                       ProfilePic = x.ProfilePic,
                       IsGuest = x.IsGuest,
                       UniqueId = x.UniqueId,
                   }).FirstOrDefault();

            }

            return MapReviewDetail;
        }


        public int UpdateCountryLanguageService(UpdateCountryLanguageparam param)
        {
            int returnId = 0;

            var ListofConLanSer = (from p in _dbContext.Users
                                   where p.Id == param.UserId
                                   select p).ToList();

            if (ListofConLanSer.Count > 0)
            {
                ListofConLanSer.ForEach(x =>
                 {
                     x.ImmigrationCountry = param.ImmigrationCountryId;
                     x.CommunicationLanguage = param.CommunicationLanguageId;
                     x.TypeOfServiceName = param.TypeofService;
                 });

                returnId = _dbContext.SaveChanges();

            }

            return returnId;

        }

        public List<HelpFAQ> HelpFAQ()
        {

            var Userhelp = new List<HelpFAQ>();

            var UserhelpDetails = _dbContext.Helps.ToList();

            if (UserhelpDetails.Count > 0)
            {
                Userhelp = UserhelpDetails.Select(x => new HelpFAQ
                {
                    Id = x.Id,
                    Question = x.Question,
                    Answer = x.Answer

                }).ToList();

            }

            return Userhelp;

        }

        public int ContactUs(ContactUsDetails param)
        {
            int returnId = 0;
            var records = new ContactU();


            records.EmailId = param.EmailId;
            records.Subject = param.Subject;
            records.Description = param.Description;
            records.IsActive = true;
            _dbContext.Add(records);
            returnId = _dbContext.SaveChanges();

            Send.SendMail(param.Subject, param.Description, param.EmailId);



            return returnId;

        }

        public int DeleteAccount(int UserId)
        {
            int returnId = 0;

            var listAccount = (from p in _dbContext.Users
                               join b in _dbContext.Retains on p.Id equals b.UserId
                               join n in _dbContext.Emitables on b.Id equals n.RetainId
                               where p.Id == UserId
                               select p).ToList();


            if (listAccount.Count < 0 && listAccount.Count == 0)
            {
                listAccount.ForEach(x => x.IsActive = false);
                returnId = _dbContext.SaveChanges();


                DeleteCometChatUserparam paramcomet = new DeleteCometChatUserparam();
                paramcomet.UserId = UserId;
                Send.CometChatDeleteUserAsync(paramcomet);

            }

            return returnId;

        }

        public int AddVerificationHandler(VerificationHandler verificationHandler)
        {
            int returnId = 0;
            try
            {
                _dbContext.Add(verificationHandler);
                _dbContext.SaveChanges();
                returnId = verificationHandler.Id;
            }
            catch (Exception ex)
            {

                throw;
            }
            return returnId;
        }
        public int UpdateVerificationHandler(VerificationHandler verificationHandler)
        {
            int returnId = 0;
            try
            {
                _dbContext.Update(verificationHandler);
                _dbContext.SaveChanges();
                returnId = verificationHandler.Id;
            }
            catch (Exception ex)
            {

                throw;
            }
            return returnId;
        }

        public VerificationHandler GetVerificationHandler(string mobileNum, string OTP)
        {
            var getData = _dbContext.VerificationHandlers.Where(x => x.UniqueId == OTP && x.PlatformDetail == mobileNum).FirstOrDefault();

            if (getData != null)
            {
                getData.Attempt = getData.Attempt + 1;

                _dbContext.VerificationHandlers.Update(getData);
                _dbContext.SaveChanges();

            }

            return getData;
        }

        public VerificationHandler GetVerificationHandlerById(int id)
        {
            var getData = _dbContext.VerificationHandlers.Where(x => x.Id == id).FirstOrDefault();

            if (getData != null)
            {
                getData.Attempt = getData.Attempt + 1;

                _dbContext.VerificationHandlers.Update(getData);
                _dbContext.SaveChanges();

            }

            return getData;
        }

        public GetUserDetail GetUserDetails(int id)
        {
            GetUserDetail result = new GetUserDetail();

            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                    UserId = id,
                };

                result = connection.Query<GetUserDetail>("UserDetailsById", parameters, commandType: CommandType.StoredProcedure)
                   .Select(x => new GetUserDetail
                   {
                       UserId = id,
                       FirstName = x.FirstName,
                       LastName = x.LastName,
                       Email = x.Email,
                       MobileCountryCode = x.MobileCountryCode,
                       Mobile = x.Mobile,
                       ProfilePic = x.ProfilePic,
                       LanguageName = x.LanguageName,
                       TypeOfService = x.TypeOfService,
                       BelongCountryName = x.BelongCountryName,
                       ImmigrationCountry = x.ImmigrationCountry,
                   }).FirstOrDefault();

            }

            return result;

        }

        public int AddFavouriteConsultant(int ConsultantId, int UserId)
        {
            int returnId = 0;
            var param = new FavouriteConsultant();
            try
            {
                param.ConsultantId = ConsultantId;
                param.UserId = UserId;
                param.IsActive = true;
                param.CreatedOn = DateTime.UtcNow;
                _dbContext.FavouriteConsultants.Add(param);
                _dbContext.SaveChanges();
                returnId = param.Id;

            }
            catch (Exception e)
            {

                throw;
            }
            return returnId;
        }

        public int RemoveFavouriteConsultant(int ConsultantId, int UserId)
        {
            int returnId = 0;
            var param = new FavouriteConsultant();
            try
            {
                var RemoveFavList = (from p in _dbContext.FavouriteConsultants
                                     where p.UserId == UserId && p.ConsultantId == ConsultantId
                                     select p).FirstOrDefault();
                if (RemoveFavList != null)
                {
                    _dbContext.FavouriteConsultants.Remove(RemoveFavList);
                    _dbContext.SaveChanges();
                    returnId = 1;
                }
                else
                {
                    returnId = 0;
                }
            }
            catch (Exception e)
            {

                throw;
            }
            return returnId;
        }

        public int AddReview(RatingReviewConsultant ratingModel)
        {
            int returnId = 0;
            try
            {
                _dbContext.RatingReviewConsultants.Add(ratingModel);
                _dbContext.SaveChanges();
                returnId = ratingModel.Id;
            }
            catch (Exception e)
            {
                // Handle exception if needed
                throw;
            }
            return returnId;
        }
        public List<NotificationMaster> UserNotificationList(int UserId, DateTime date)
        {
            var NotificationList = new List<NotificationMaster>();
            var AdminNotificationList = new List<NotificationMaster>();

            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                    UserId = UserId
                };

                NotificationList = connection.Query<NotificationMaster>("UserNotificationList", parameters, commandType: CommandType.StoredProcedure)
                   .Select(x => new NotificationMaster
                   {
                       Id = x.Id,
                       Header = x.Header,
                       Body = x.Body,
                       CreatedOn = x.CreatedOn,  //x.CreatedOn,
                       ReceiverId = x.ReceiverId,
                       SenderId = x.SenderId,
                       NotificationTypeName = x.NotificationTypeName

                   }).ToList();

                
            }
            var notificationlist = _dbContext.AdminNotifications.Where(d => (d.SpecificRole == UserId || d.SpecificRole == null && d.Role == "Both" && d.CreatedOn.Date >= date.Date)).ToList();
            if (notificationlist.Count > 0)
            {
                notificationlist.ForEach(r => r.IsRead = true);
                _dbContext.SaveChanges();

                AdminNotificationList = notificationlist.Select(r => new NotificationMaster
                {
                    Id = r.Id,
                    Header = r.Title,
                    Body = r.Description,
                    CreatedOn = r.CreatedOn,
                }).ToList();



            }
            List<NotificationMaster> mergedNotifications = NotificationList.Concat(AdminNotificationList).ToList();

            return mergedNotifications;
        }

        public int GetNotificationCount(int Id, DateTime date)
        {

            try
            {
                var NotificationCount = _dbContext.NotificationMasters.Where(d => d.ReceiverId == Id && d.IsAct == true && (d.IsRead == null || d.IsRead == false)).ToList();
                var AdminCount = _dbContext.AdminNotifications.Where(c => (c.SpecificRole == Id || c.SpecificRole == null && c.Role == "Both" && c.CreatedOn.Date >= date.Date) && (c.IsRead == null || c.IsRead == false)).ToList();
                return NotificationCount.Count + AdminCount.Count;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public List<NotificationMaster> GetNotificationMasters(int UserId)
        {
            return _dbContext.NotificationMasters.Where(m => m.SenderId == UserId && m.NotificationTypeName == 4).ToList();
        }
        public int FetchReviewOfUser(int userId, int consultantId)
        {
            int returnId = 0;

            var getReview = _dbContext.RatingReviewConsultants.Where(x => x.UserId == userId && x.ConsultantId == consultantId).FirstOrDefault();

            if (getReview != null)
            {
                returnId = getReview.Id;
            }

            return returnId;
        }

        public int GetReviewIdForUserAndConsultant(int userId, int consultantId)
        {
            var review = _dbContext.RatingReviewConsultants
                .FirstOrDefault(x => x.UserId == userId && x.ConsultantId == consultantId);

            return review?.Id ?? 0;
        }

        public int GetCardsCount(int id)
        {
            int getCount = _dbContext.UserCardsDetails.Where(x => x.Id == id).Count();
            return getCount;
        }

        public List<UserDocument> GetUserDocuments(int userId)
        {
            var userDocuments = _dbContext.UserDocuments
                .Where(d => d.UserId == userId)
                .ToList();

            return userDocuments;
        }

        public string GetCommetChatUserList(int consultantId)
        {
            string strReturn = string.Empty;
            string returnStr = string.Empty;

            var distinctConsultants = (from r in _dbContext.Retains
                                       join c in _dbContext.Users on r.UserId equals c.Id
                                       where r.ConsultantId == consultantId
                                       select c.CometChatUserUid).Distinct();

            if(distinctConsultants != null && distinctConsultants.Count() > 0)
            {
                foreach (var item in distinctConsultants.ToList())
                {
                    strReturn += item + ",";
                }
            }
            
            if(!string.IsNullOrEmpty(strReturn))
            {
                returnStr = strReturn.Substring(0, strReturn.Length - 1);
            }
            
            return strReturn;
        }
    }
}
