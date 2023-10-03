using Dapper;
using IMMIWeb.Service.Models;
using IMMIWeb.Service.Repo;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using IMMIWeb.Service.Service.Communication;
using IMMIWeb.Service.Service.Consultant;
using IMMIWeb.Service.Service.General;
using IMMIWeb.Service.Service.StripePay;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SendGrid.Helpers.Mail;
using Stripe;
using Stripe.FinancialConnections;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Twilio.TwiML.Voice;
using static System.Net.WebRequestMethods;
using Twilio.Rest.Oauth.V1;
using static Azure.Core.HttpHeader;
using Common = IMMIWeb.Service.Service.General.Common;
using System.Globalization;
using Polly;
using FirebaseAdmin.Messaging;

namespace IMMIWeb.Service.Service.Consultant
{
    public class ConsultantRepository : GenericRepository<IMMIWeb.Consultant>, IConsultantRepository
    {
        private IConfiguration Configuration;
        public ConsultantRepository(DbA976eeImmitestContext context, IConfiguration configuration) : base(context)
        {
            Configuration = configuration;
        }

        public int AddConsultant(IMMIWeb.Consultant consultant)
        {
            int returnId = 0;

            _dbContext.Add(consultant);
            _dbContext.SaveChanges();
            returnId = consultant.Id;

            return returnId;
        }
        public bool BookConsultantSlot(int ConsultantSlotId, bool status)
        {
            try
            {
                IMMIWeb.ConsultantSlot slotObj = _dbContext.ConsultantSlots.Where(x => x.Id == ConsultantSlotId).FirstOrDefault();
                if (slotObj != null)
                {
                    slotObj.IsActive = status;
                    _dbContext.ConsultantSlots.Update(slotObj);
                    _dbContext.SaveChanges();
                }
                return true;

            }
            catch (Exception e)
            {
                return false;
                throw;
            }
        }
        public IEnumerable<CommonListViewModel> GetConsultantCommunicationLanguage(int ConsultantId)
        {
            var query = _dbContext.ConsultantLanguages.Where(x => x.IsActive)
                .Join(_dbContext.Languages.Where(x => x.IsActive == true), e => e.Language, d => d.Id,
                (CommunicationLanguage, Language) => new CommonListViewModel
                {
                    Name = Language.Name,
                    Id = Language.Id
                }).ToList();
            return query;
        }
        public IEnumerable<CommonListViewModel> GetConsultantCountryForService(int ConsultantId)
        {
            var query = _dbContext.ConsultantServiceForCountries.Where(x => x.IsActive)
                .Join(_dbContext.Countries.Where(x => x.IsActive == true), e => e.Country, d => d.Id,
                (CountryForService, Country) => new CommonListViewModel
                {
                    Name = Country.Name,
                    Id = Country.Id
                }).ToList();
            return query;
        }
        public DateTime GetConsultantSlotDate(int ConsultantSlotId)
        {

            var getDate = _dbContext.ConsultantSlots.Where(x => x.Id == ConsultantSlotId).FirstOrDefault();

            return getDate.Date;
        }
        public IEnumerable<CommonListViewModel> GetConsultantTypeOfServices(int ConsultantId)
        {
            var query = _dbContext.ConsultantTypeOfServices.Where(x => x.IsActive)
                .Join(_dbContext.TypeOfServices.Where(x => x.IsActive == true), e => e.TypeOfService, d => d.Id,
                (ConsultantTypeOfServices, TypeOfServices) => new CommonListViewModel
                {
                    Name = TypeOfServices.Name,
                    Id = TypeOfServices.Id
                }).ToList();
            return query;
        }

        public IEnumerable<ConsultantRetainClientListViewModel> GetConsultantClientList(int Consultantid)
        {
            List<ConsultantRetainClientListViewModel> clientList = new List<ConsultantRetainClientListViewModel>();

            var clients = (from retain in _dbContext.Retains
                           join user in _dbContext.Users on retain.UserId equals user.Id into userGroup
                           from user in userGroup.DefaultIfEmpty()
                           join immigrationCountry in _dbContext.Countries on retain.RetainCountryForService equals immigrationCountry.Id into immigrationCountryGroup
                           from immigrationCountry in immigrationCountryGroup.DefaultIfEmpty()
                           join belongingCountry in _dbContext.Countries on user.Country equals belongingCountry.Id into belongingCountryGroup
                           from belongingCountry in belongingCountryGroup.DefaultIfEmpty()
                           where retain.ConsultantId == Consultantid
                           select new
                           {
                               retain.ConsultantId,
                               retain.UserId,
                               CreatedOn = retain.CreatedOn, //TimeZoneInfo.ConvertTimeFromUtc(retain.CreatedOn, TimeZoneInfo.Local).Date,
                               retain.IsAct,
                               retain.RetainTypeOfService,
                               retain.RetainCountryForService,
                               retain.RetainCommunicationLanguage,
                               FirstName = user != null ? user.FirstName : null,
                               LastName = user != null ? user.LastName : null,
                               Email = user != null ? user.Email : null,
                               ProfilePic = user != null ? user.ProfilePic : null,
                               MobileNumber = user != null ? user.Mobile : null,
                               CountryCode = user != null ? user.MobileCountryCode : null,
                               ImmigrationCountryName = immigrationCountry != null ? immigrationCountry.Name : null,
                               BelongingCountryName = belongingCountry != null ? belongingCountry.Name : null,
                           }).ToList();

            clientList = clients.Select(c => new ConsultantRetainClientListViewModel
            {
                ConsultantId = c.ConsultantId,
                UserId = c.UserId,
                CreatedOn = c.CreatedOn,
                FirstName = c.FirstName,
                LastName = c.LastName,
                MobileNumber = c.MobileNumber,
                Email = c.Email,
                ProfilePic = c.ProfilePic,
                CountryCode = c.CountryCode,
                ImmigrationCountryName = c.ImmigrationCountryName,
                BelongCountryName = c.BelongingCountryName,
                IsAct = c.IsAct,
                RetainTypeOfService = c.RetainTypeOfService,
                RetainCountryForService = c.RetainCountryForService,
                RetainCommunicationLanguage = c.RetainCommunicationLanguage
            }).ToList();

            return clientList;
        }

        public IEnumerable<ConsultantPaymentHistoryViewModel> GetPaymentHistoryList(int ConsultantId)
        {
            List<ConsultantPaymentHistoryViewModel> paymentList = new List<ConsultantPaymentHistoryViewModel>();

            var retainPayments = from appointment in _dbContext.Appointments
                                 join appointmentPayment in _dbContext.AppointmentPayments on appointment.ConsultantId equals appointmentPayment.ConsultantId
                                 join user in _dbContext.Users on appointment.UserId equals user.Id
                                 join retain in _dbContext.Retains on appointment.ConsultantId equals retain.ConsultantId
                                 join retainPayment in _dbContext.RetainPayments on retain.Id equals retainPayment.RetainId
                                 where appointment.ConsultantId == ConsultantId
                                 select new ConsultantPaymentHistoryViewModel
                                 {
                                     ProfilePic = user.ProfilePic ?? string.Empty,
                                     FirstName = user.FirstName ?? string.Empty,
                                     LastName = user.LastName ?? string.Empty,
                                     RetainDate = retain.CreatedOn,
                                     AppointmentDate = (DateTime)appointment.AppointmentDate,
                                     RejectionDate = (DateTime)appointment.CancellationDate,
                                     RetainAmount = retainPayment.RetainAmount ?? 0,
                                     Amount = appointmentPayment.Amount ?? 0,
                                     CancellationReason = appointment.CancellationReason ?? string.Empty,
                                     RejectionAmount = appointment.RejectionAmount ?? 0
                                 };

            //var appointmentPayments = from appointment in _dbContext.Appointments
            //                          join appointmentPayment in _dbContext.AppointmentPayments on appointment.Id equals appointmentPayment.AppointmentId
            //                          join user in _dbContext.Users on appointment.UserId equals user.Id
            //                          join retain in _dbContext.Retains on user.Id equals retain.UserId
            //                          select new ConsultantPaymentHistoryViewModel
            //                          {
            //                              ProfilePic = user.ProfilePic,
            //                              FirstName = user.FirstName,
            //                              LastName = user.LastName,
            //                              RetainDate = retain.CreatedOn,
            //                              Amount = appointmentPayment.Amount,
            //                              CancellationReason = appointment.CancellationReason,
            //                              RejectionAmount = 0 
            //                          };

            //var rejections = from appointment in _dbContext.Appointments
            //                 join user in _dbContext.Users on appointment.UserId equals user.Id
            //                 where appointment.RejectionAmount != null
            //                 select new ConsultantPaymentHistoryViewModel
            //                 {
            //                     ProfilePic = user.ProfilePic,
            //                     FirstName = user.FirstName,
            //                     LastName = user.LastName,
            //                     RetainDate = (DateTime)appointment.CreatedOn,
            //                     Amount = 0,
            //                     CancellationReason = appointment.CancellationReason,
            //                     RejectionAmount = (decimal)appointment.RejectionAmount
            //                 };

            // paymentList.AddRange(retainPayments);
            paymentList = retainPayments.Distinct().ToList();

            return paymentList;
        }





        public ConsultantRetainClientListViewModel GetClientDetails(int userId)
        {
            var clientDetails = (from retain in _dbContext.Retains
                                 join user in _dbContext.Users on retain.UserId equals user.Id into userGroup
                                 from user in userGroup.DefaultIfEmpty()
                                 join immigrationCountry in _dbContext.Countries on retain.RetainCountryForService equals immigrationCountry.Id into immigrationCountryGroup
                                 from immigrationCountry in immigrationCountryGroup.DefaultIfEmpty()
                                 join belongingCountry in _dbContext.Countries on user.Country equals belongingCountry.Id into belongingCountryGroup
                                 from belongingCountry in belongingCountryGroup.DefaultIfEmpty()
                                 join typeOfService in _dbContext.TypeOfServices on user.TypeOfServiceName equals typeOfService.Id into typeOfServiceGroup
                                 from typeOfService in typeOfServiceGroup.DefaultIfEmpty()
                                 join language in _dbContext.Languages on user.CommunicationLanguage equals language.Id into languageGroup
                                 from language in languageGroup.DefaultIfEmpty()
                                 join retainPayment in _dbContext.RetainPayments on retain.Id equals retainPayment.RetainId into retainPaymentGroup
                                 from retainPayment in retainPaymentGroup.DefaultIfEmpty()
                                 let charge = _dbContext.Charges.FirstOrDefault()
                                 join userDocument in _dbContext.UserDocuments on retain.UserId equals userDocument.UserId into userDocumentGroup
                                 from document in userDocumentGroup.DefaultIfEmpty()
                                 where retain.UserId == userId
                                 select new ConsultantRetainClientListViewModel
                                 {
                                     ConsultantId = retain.ConsultantId,
                                     UserId = retain.UserId,
                                     CreatedOn = retain.CreatedOn,
                                     IsAct = retain.IsAct,
                                     RetainTypeOfService = retain.RetainTypeOfService,
                                     RetainCommunicationLanguage = retain.RetainCommunicationLanguage,
                                     FirstName = user != null ? user.FirstName : null,
                                     LastName = user != null ? user.LastName : null,
                                     Email = user != null ? user.Email : null,
                                     ProfilePic = user != null ? user.ProfilePic : null,
                                     MobileNumber = user != null ? user.Mobile : null,
                                     CountryCode = user != null ? user.MobileCountryCode : null,
                                     ImmigrationCountryName = immigrationCountry != null ? immigrationCountry.Name : null,
                                     BelongCountryName = belongingCountry != null ? belongingCountry.Name : null,
                                     TypeofService = typeOfService != null ? typeOfService.Name : null,
                                     Language = language != null ? language.Name : null,
                                     ReatinAmount = Convert.ToDecimal(retainPayment != null ? retainPayment.RetainAmount : 0.0M),
                                     TaxCharge = (decimal)charge.TaxCharge,
                                     AppCharge = (decimal)charge.ApplicationCharge,
                                     PaymentMode = retainPayment != null ? retainPayment.PaymentModeName : 0,
                                     //FileName = document != null ? document.Filename: null,
                                     //FileExtension = document != null ? document.Extensions: null,
                                     //FileUrl = document != null ? document.DocUrl: null,
                                 }).FirstOrDefault();

            return clientDetails;
        }


        public IEnumerable<string> GetConsultatnSlotDate(int ConsultantId, string Timezone)
        {
           
            //return _dbContext.ConsultantSlots.Where(x => x.ConsultantId == 78 && x.Date.Date >= DateTime.Now.Date).Select(x => x.Date.Date.ToString("yyyy-MM-dd")).Distinct();
            //return _dbContext.ConsultantSlots.Where(x => x.ConsultantId == 78 && TimeZoneInfo.ConvertTimeFromUtc(x.Date, TimeZoneInfo.Local).Date >= DateTime.Now.Date).Select(x => TimeZoneInfo.ConvertTimeFromUtc(x.Date , TimeZoneInfo.Local).ToString("yyyy-MM-dd hh:mm:ss.sss tt")).Distinct();
            //var localtimezone = TimeZoneInfo.Local;
            //var localtimezone = Common.GetLocalTimeZone();
            //TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(localtimezone.StandardName);
            DateTime currentDate = DateTime.Now.Date;

            var LocalTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(currentDate, TimeZoneInfo.Local.Id, Timezone);

            var distinctDates = _dbContext.ConsultantSlots
             .Where(c => c.ConsultantId == ConsultantId)
             .AsEnumerable() // Switch to client evaluation here
             .Where(c => Common.GetTimeZone(c.Date, Timezone).Date >= LocalTime)
             .Select(localDate => Common.GetTimeZone(localDate.Date, Timezone).ToString("yyyy-MM-dd hh:mm:ss.sss tt")) //localDate.Date.ToString("yyyy-MM-dd"))
             .Distinct();
            return distinctDates;
        }
        public IEnumerable<SlotDetail> GetConsultatnSlotTime(int ConsultantId, string Date, string Timezone)
        {
            //var localtimezone = TimeZoneInfo.Local;
            var localtimezone = Common.GetLocalTimeZone();
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(localtimezone.StandardName);

            //var query = _dbContext.ConsultantSlots.Where(
            //    x => x.ConsultantId == 78 && x.Date.Date == Convert.ToDateTime(Date) /*&& x.StartHour > DateTime.Now.Hour*/ && x.IsActive == null)
            //    .Select(x => new SlotDetail { Id = x.Id, Hour = Common.GetTimeZone(x.StartHour,ConsultantId) });
            

            var query = _dbContext.ConsultantSlots
             .Where(c => c.ConsultantId == ConsultantId)
             .AsEnumerable() // Switch to client evaluation here
             .Where(c => Common.GetTimeZone(c.Date,Timezone).Date == Convert.ToDateTime(Date).Date && c.IsActive == null)
             .Select(x => new SlotDetail
             {
                 Id = x.Id,
                 Date = Common.GetTimeZone(x.Date, Timezone),
                 Hour = TimeZoneInfo.ConvertTimeFromUtc(x.Date, timeZoneInfo).Hour,
                 Minutes = TimeZoneInfo.ConvertTimeFromUtc(x.Date, timeZoneInfo).Minute
             })
             .Distinct();


            return query;
        }
        public IMMIWeb.Consultant IsValidConsultant(string mobileCountryCode, string mobile)
        {
            return _dbContext.Consultants.Where(x => x.MobileCountryCode == mobileCountryCode && x.Mobile == mobile).FirstOrDefault();
        }
        public IMMIWeb.Consultant ValidConsultant(int ConsultantId)
        {
            return _dbContext.Consultants.Where(x => x.Id == ConsultantId
            && x.IsAgreement == true && x.IsActive == true && x.IsVerified == true && x.EmailConfirmed == true
            && x.IsConsultantStripAccountVerified == true
            && x.IsAdminApproved == true).FirstOrDefault();

        }
        public void AddConsultantSlotAsync(List<string> SlotDate, int ConsultantId)
        {
            var ConsultantBusinesshours =
                     (from d in _dbContext.ConsultantSlots
                      where d.ConsultantId == ConsultantId
                      select new
                      {
                          d.Date,
                      }).Distinct().ToList();


            var localtimezone = Common.GetLocalTimeZone();



            foreach (var item in SlotDate)
            {
                if (!ConsultantBusinesshours.Any(b => b.Date.Date == Convert.ToDateTime(item).Date))
                {
                    ConsultantSlot cSlot = new ConsultantSlot();
                    cSlot.ConsultantId = ConsultantId;
                    cSlot.Date = Convert.ToDateTime(item);
                    cSlot.TimeZone = localtimezone.StandardName;

                    _dbContext.ConsultantSlots.Add(cSlot);
                    _dbContext.SaveChanges();
                }

            }


            //string[] dateMaster = SlotDate.Split("-");

            //string startDateString = dateMaster[0];
            //string endDateString = dateMaster[1];

            //DateTime startDate = Convert.ToDateTime(startDateString);
            //DateTime endDate = Convert.ToDateTime(endDateString);

            //startDate = startDate.Date + DateTime.Now.TimeOfDay;
            //endDate = endDate.Date + DateTime.Now.TimeOfDay;



            //List<ConsultantSlot> lstConsultantSlot = new List<ConsultantSlot>();
            //DateTime currentDate;
            //var localtimezone = TimeZoneInfo.Local;

            //for (DateTime currentDate = startDate; currentDate <= endDate; currentDate = currentDate.AddDays(1))
            //{
            //    if (!ConsultantBusinesshours.Any(b => b.Date.Date == currentDate.Date))
            //    {
            //        string[] hrsArray = SlotHrs.Split(',');

            //        foreach (string hrs in hrsArray)
            //        {
            //            // DateTime newDate = currentDate.AddHours(Convert.ToInt32(hrs));
            //            ConsultantSlot consultantSlotObj = new ConsultantSlot();
            //            consultantSlotObj.ConsultantId = ConsultantId;
            //            consultantSlotObj.Date = currentDate.AddHours(Convert.ToInt32(hrs)).ToUniversalTime();
            //            consultantSlotObj.IsActive = null;
            //            consultantSlotObj.StartHour = Convert.ToInt32(hrs);
            //            consultantSlotObj.EndHour = Convert.ToInt32(hrs) + 1;
            //            consultantSlotObj.TimeZone = localtimezone.StandardName;
            //            lstConsultantSlot.Add(consultantSlotObj);
            //        }
            //    }
            //}

            //List<DateTime> parsedDates = SlotDate
            // .Select(dtString => DateTime.ParseExact(dtString, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture))
            // .ToList();

            //List<ConsultantSlot> consultantDataList = parsedDates
            // .Where(parsedDate => !ConsultantBusinesshours.Any(b => b.Date.Date == parsedDate.Date))
            // .Select(parsedDate => new ConsultantSlot
            // {
            //     ConsultantId = ConsultantId,
            //     Date = parsedDate.Date,
            //     TimeZone = localtimezone.StandardName
            // })
            // .ToList();


            //List<ConsultantSlot> consultantDataList = SlotDate
            //         //.Select(dtString => DateTime.ParseExact(dtString, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture))
            //         .Where(dt => !ConsultantBusinesshours.Any(b => b.Date.Date == DateTime.Parse(dt).Date))
            //         .Select(dt => new ConsultantSlot
            //         {
            //             ConsultantId = ConsultantId,
            //             Date = DateTime.Parse(dt).Date + DateTime.Parse(dt).TimeOfDay,
            //             TimeZone = localtimezone.StandardName
            //         })
            //         .ToList();


            //_dbContext.ConsultantSlots.AddRange(consultantDataList);
            //_dbContext.SaveChanges();
        }
        public IEnumerable<GetConsultantByTosClangCountryViewModel> GetConsultantByTosClangCountry(int tosId, int langId, int cntId, int startCnt, int endCnt, int userid, bool isGuestUser)
        {
            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                    TOSId = tosId,
                    LangId = langId,
                    CntId = cntId,
                    StartCnt = startCnt,
                    EndCnt = endCnt,
                    UserId = userid,
                };

                string spName = "GetConsultantByTosClangCountry";
                if(isGuestUser)
                {
                    spName = "GetConsultantByTosClangCountryForGuest";
                }

                return connection.Query<GetConsultantByTosClangCountryViewModel>(spName, parameters, commandType: CommandType.StoredProcedure)
                   .Select(x => new GetConsultantByTosClangCountryViewModel
                   {
                       CurrentId = x.CurrentId,
                       FirstName = x.FirstName,
                       LastName = x.LastName,
                       TypeOfServiceName = x.TypeOfServiceName,
                       LanguageName = x.LanguageName,
                       CountryName = x.CountryName,
                       AvgRating = x.AvgRating,
                       CountRating = x.CountRating,
                       ProfilePic = x.ProfilePic,
                       ConsultantRank = x.ConsultantRank,
                       IsFavConsultantornot = x.IsFavConsultantornot,
                   });
            }
        }
        public IEnumerable<GetFavouriteConsultantByUserIdViewModel> GetFavouriteConsultantByUserId(int UserId)
        {
            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                    UserId = UserId
                };

                return connection.Query<GetFavouriteConsultantByUserIdViewModel>("GetFavouriteConsultantByUserId", parameters, commandType: CommandType.StoredProcedure)
                    .Select(x => new GetFavouriteConsultantByUserIdViewModel
                    {
                        CurrentId = x.CurrentId,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        ProfilePic = x.ProfilePic,
                        IsAvailable = x.IsAvailable,
                        TypeOfServiceName = x.TypeOfServiceName,
                        LanguageName = x.LanguageName,
                        CountryName = x.CountryName,
                        AvgRating = x.AvgRating,
                        CountRating = x.CountRating,
                    });
            }
        }
        public bool UpdateConsultantAvailability(int userId, bool isAvailable)
        {
            try
            {
                var user = _dbContext.Consultants.FirstOrDefault(u => u.Id == userId);

                if (user != null)
                {
                    user.IsAvailable = isAvailable;
                    _dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }


        public List<NotificationMaster> ConsultantNotificationList(int ConsultantId, DateTime date)
        {
            var NotificationList = new List<NotificationMaster>();
            var AdminNotificationList = new List<NotificationMaster>();

            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                    ConsultantId = ConsultantId
                };

                NotificationList = connection.Query<NotificationMaster>("ConsultantNotificationList", parameters, commandType: CommandType.StoredProcedure)
                   .Select(x => new NotificationMaster
                   {
                       Id = x.Id,
                       Header = x.Header,
                       Body = x.Body,
                       CreatedOn = x.CreatedOn,
                       ReceiverId = x.ReceiverId,
                       SenderId = x.SenderId,
                       NotificationTypeName = x.NotificationTypeName

                   }).ToList();
            }

            var notificationlist = _dbContext.AdminNotifications.Where(d => (d.SpecificRole == ConsultantId || d.SpecificRole == null && d.Role == "Both" && d.CreatedOn.Date >= date.Date)).ToList();
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

        public void UpdateConsultantSlotAsync(List<string> SlotDate,int ConsultantId)
        {


            

            var Consulantslotdata = _dbContext.ConsultantSlots.Where(d => d.ConsultantId == ConsultantId && d.Date.Date == Convert.ToDateTime(SlotDate[0]).Date).ToList();

            if (Consulantslotdata.Count > 0)
            {
                _dbContext.ConsultantSlots.RemoveRange(Consulantslotdata);
                _dbContext.SaveChanges();

                var localtimezone = Common.GetLocalTimeZone();
                foreach (var item in SlotDate)
                {

                    ConsultantSlot cSlot = new ConsultantSlot();
                    cSlot.ConsultantId = ConsultantId;
                    cSlot.Date = Convert.ToDateTime(item);
                    cSlot.TimeZone = localtimezone.StandardName;

                    _dbContext.ConsultantSlots.Add(cSlot);
                    _dbContext.SaveChanges();

                }

               
                
            }
        }

        public string[] ListofAvailableHoursConsultant(int ConsultantId)
        {
            string[] disabledDates = new string[0];
            try
            {
                var ConsultantBusinesshours =
                   (from d in _dbContext.ConsultantSlots
                    where d.ConsultantId == ConsultantId
                    select new
                    {
                        d.Date

                    }).Distinct().ToList();

                if (ConsultantBusinesshours.Count > 0)
                {
                    disabledDates = ConsultantBusinesshours.Select(d => d.Date.ToString("yyyy-MM-dd hh:mm:ss.sss tt")).ToArray();

                }
                return disabledDates;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public List<ConsultantSlot> ListofBusinessHoursConsultant(int ConsultantId, string SlotDate)
        {
            var ConsultantSlotList = new List<ConsultantSlot>();


            DateTime date = Convert.ToDateTime(SlotDate);
            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                    ConsultantId = ConsultantId,
                    SlotDate = date.Date.Date
                };

                ConsultantSlotList = connection.Query<ConsultantSlot>("ListofBusinessHoursConsultant", parameters, commandType: CommandType.StoredProcedure)
                   .Select(x => new ConsultantSlot
                   {
                       Date = x.Date,
                       StartHour = x.StartHour,
                       EndHour = x.EndHour,
                       Id = x.Id,

                   }).ToList();


            }

            return ConsultantSlotList;
        }

        public void Updatecommunicationlanguage(string CommunicationLanguage, int ConsultantId)
        {
            string[] languageParts = CommunicationLanguage.Split(',');
            int[] commlanguage = new int[languageParts.Length];

            for (int i = 0; i < languageParts.Length; i++)
            {
                commlanguage[i] = int.Parse(languageParts[i].ToString());
            }
            // int[] commlanguage = CommunicationLanguage.Split(',');

            var itemToRemoveLanguage = _dbContext.ConsultantLanguages.Where(x => x.ConsultantId == ConsultantId).ToList();
            if (itemToRemoveLanguage.Count > 0)
            {
                _dbContext.ConsultantLanguages.RemoveRange(itemToRemoveLanguage);

                _dbContext.SaveChanges();
                // The item has been removed from the database
            }

            var consultantLanguages = commlanguage.Select(lang => new ConsultantLanguage
            {
                ConsultantId = ConsultantId,
                Language = lang,
                IsActive = true
            });

            _dbContext.ConsultantLanguages.AddRange(consultantLanguages);

            _dbContext.SaveChanges();

        }

        public void Updatetypeofservice(string Typeofservice, int ConsultantId)
        {
            string[] serviceParts = Typeofservice.Split(',');
            int[] typeofservice = new int[serviceParts.Length];

            for (int i = 0; i < serviceParts.Length; i++)
            {
                typeofservice[i] = int.Parse(serviceParts[i].ToString());
            }
            // int[] commlanguage = CommunicationLanguage.Split(',');

            var itemToRemoveService = _dbContext.ConsultantTypeOfServices.Where(x => x.ConsultantId == ConsultantId).ToList();
            if (itemToRemoveService.Count > 0)
            {
                _dbContext.ConsultantTypeOfServices.RemoveRange(itemToRemoveService);

                _dbContext.SaveChanges();
                // The item has been removed from the database
            }

            var consultanttypeofservice = typeofservice.Select(service => new ConsultantTypeOfService
            {
                ConsultantId = ConsultantId,
                TypeOfService = service,
                IsActive = true,
                CountryId = _dbContext.TypeOfServices
                                           .Where(con => service == con.Id)
                                           .Select(con => con.CountryId)
                                           .FirstOrDefault() ?? 0 // Assuming CountryId is of type int
            });

            _dbContext.ConsultantTypeOfServices.AddRange(consultanttypeofservice);

            _dbContext.SaveChanges();

        }

        public bool Addhikeretentionamount(decimal RetentionAmount, int ConsultantId)
        {
            try
            {
                var records = new ConsultantChargeRequest
                {
                    Amount = RetentionAmount,
                    ConsultantId = ConsultantId,
                    Status = "Pending",
                    CreatedOn = Convert.ToDateTime(DateTime.UtcNow),
                    IsActive = true,
                };
                _dbContext.ConsultantChargeRequests.Add(records);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
                throw;
            }
        }

        public bool DeleteAccount(int ConsultantId)
        {
            try
            {
                var account = (from p in _dbContext.Consultants
                               where p.Id == ConsultantId
                               select p).ToList();

                if (account.Count > 0)
                {
                    //string uniqueStamp = IMMIWeb.Service.Service.General.Common.GenerateUniqueStamp();
                    string uniqueStamp = Common.GenerateOTP();
                    account.ForEach(x =>
                    {
                        x.IsDeleteConsultantAccount = true;
                        x.Mobile = !string.IsNullOrEmpty(account[0].Mobile) ? account[0].Mobile + uniqueStamp : string.Empty;
                        x.Email = !string.IsNullOrEmpty(account[0].Email) ? account[0].Email + uniqueStamp : string.Empty;
                    });

                    _dbContext.SaveChanges();

                    DeleteCometChatConsultantparam paramcomet = new DeleteCometChatConsultantparam();
                    paramcomet.CometChatConsultantUid = account[0].CometChatConsultantUid;
                    Send.CometChatDeleteConsultantAsync(paramcomet);
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
                throw;
            }
        }

        public IEnumerable<ConsultantAppointmentByStatusViewModel> ConsultantAppointmentByStatus(int Consultantid, int StatusType)
        {
            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                    ConsultantId = Consultantid,
                    Status = StatusType,
                };

                var data = connection.Query<ConsultantAppointmentByStatusViewModel>("ConsultantAppointmentByStatus", parameters, commandType: CommandType.StoredProcedure)
                    .Select(x => new ConsultantAppointmentByStatusViewModel
                    {
                        UserId = x.UserId,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        ProfilePic = x.ProfilePic,
                        CometChatUserUID = x.CometChatUserUID,
                        LanguageName = x.LanguageName,
                        AppointmentId = x.AppointmentId,
                        ConsultantId = x.ConsultantId,
                        UserRequestTypeName = x.UserRequestTypeName,
                        TypeOfService = x.TypeOfService,
                        CountryName = x.CountryName,
                        //BookingDate = x.BookingDate,
                        //BookingTime = x.BookingTime,
                        BookingDate = x.BookingDate,
                       // BookingTime = TimeZoneInfo.ConvertTimeFromUtc(x.BookingDate, TimeZoneInfo.Local).Hour,  // x.BookingTime,
                       // BookingMinutes = TimeZoneInfo.ConvertTimeFromUtc(x.BookingDate, TimeZoneInfo.Local).Minute,
                        SessionTitle = x.SessionTitle,
                    });
                return data;
            }
        }

        public List<NotificationMaster> PaymentHistoryList(int ConsultantId, decimal exchangerate)
        {
            var NotificationList = new List<NotificationMaster>();

            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                    ConsultantId = ConsultantId,
                    ExchangeRate = exchangerate
                };

                NotificationList = connection.Query<NotificationMaster>("PaymentHistoryList", parameters, commandType: CommandType.StoredProcedure)
                   .Select(x => new NotificationMaster
                   {
                       Id = x.Id,
                       Header = x.Header,
                       Body = x.Body,
                       CreatedOn = x.CreatedOn,
                       ReceiverId = x.ReceiverId,
                       SenderId = x.SenderId,
                       NotificationTypeName = x.NotificationTypeName

                   }).ToList();


            }

            return NotificationList;
        }

        public int AddWithdrawAmount(Withdraw withdraw)
        {
            int returnId = 0;

            _dbContext.Add(withdraw);
            _dbContext.SaveChanges();
            returnId = withdraw.Id;

            return returnId;
        }


        public IEnumerable<ConsultantApponintmentDetailByUserIdViewModel> ConsultantAppointmentDetailsByAppointmentId(int appointmentId)
        {
            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                    Appointmentid = appointmentId
                };

                var data = connection.Query<ConsultantApponintmentDetailByUserIdViewModel>("GetAppointmentByAppointmentId", parameters, commandType: CommandType.StoredProcedure)
                    .Select(x => new ConsultantApponintmentDetailByUserIdViewModel
                    {
                        UserId = x.UserId,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        ProfilePic = x.ProfilePic,
                        ApplyForCountry = x.ApplyForCountry,
                        BelongCountry = x.BelongCountry,
                        LanguageName = x.LanguageName,
                        AppointmentId = x.AppointmentId,
                        ConsultantId = x.ConsultantId,
                        UserRequestTypeName = x.UserRequestTypeName,
                        TypeOfService = x.TypeOfService,
                        //BookingTime = x.BookingTime,
                        CreatedOn = x.CreatedOn,
                        //BookingDate = x.BookingDate,
                        SessionTitle = x.SessionTitle,
                        BookingDate = x.BookingDate,  
                        //BookingTime = TimeZoneInfo.ConvertTimeFromUtc(x.BookingDate, TimeZoneInfo.Local).Hour, 
                        //BookingMinutes = TimeZoneInfo.ConvertTimeFromUtc(x.BookingDate, TimeZoneInfo.Local).Minute,
                    });
                return data;
            }
        }

        public string GetCommetChatConsultnatList(int userId)
        {
            string strReturn = string.Empty;
            string returnStr = string.Empty;

            var distinctConsultants = (from r in _dbContext.Retains
                                       join c in _dbContext.Consultants on r.ConsultantId equals c.Id
                                       where r.UserId == userId
                                       select c.CometChatConsultantUid).Distinct();

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
