using IMMIWeb.Infrastructure;
using IMMIWeb.Service.Models;
using IMMIWeb.Service.Service.Appointment;
using IMMIWeb.Service.Service.Consultant;
using IMMIWeb.Service.Service.StripePay;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using IMMIWeb.Service.Service.Setting;
using IMMIWeb.Service.Service.User;
using System.Collections.Generic;
using IMMIWeb.Service.Service.General;
using IMMIWeb.Service.Service.Communication;

namespace IMMIWeb.Areas.Consultant.Controllers
{
    [Area("Consultant")]
    [Authorize(Roles = "Consultant")]
    public class ConsultantHomeController : Controller
    {

        #region Fields
        private readonly IConsultantRepository _consultantRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IAppointmentPaymentRepository _appointmentPaymentRepository;
        private readonly IStripeRepository _stripeRepository;
        private readonly IUserRepository _userRepository;        
        private readonly ITypeOfServiceRepository _typeOfServiceRepository;
        private readonly IMapper _mapper;
        private readonly ILanguageRepository _languageRepository;
        private readonly ICountryRepository _countryRepository;

        int sCountVal = Convert.ToInt32(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("GeneralInfo")["sCountVal"]);
        int eCountVal = Convert.ToInt32(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("GeneralInfo")["eCountVal"]);
        #endregion

        #region Ctor
        public ConsultantHomeController(
            IConsultantRepository consultantRepository,
            IAppointmentRepository appointmentRepository,
            IStripeRepository stripeRepository,
            IAppointmentPaymentRepository appointmentPaymentRepository,
            ITypeOfServiceRepository typeOfServiceRepository,            
            IMapper mapper,
            IUserRepository userRepository,
            ILanguageRepository languageRepository,
            ICountryRepository countryRepository
            )
        {
            _consultantRepository = consultantRepository;
            _appointmentRepository = appointmentRepository;
            _stripeRepository = stripeRepository;
            _appointmentPaymentRepository = appointmentPaymentRepository;
            _typeOfServiceRepository = typeOfServiceRepository;
            _consultantRepository = consultantRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _languageRepository = languageRepository;
            _countryRepository = countryRepository;

        }
        #endregion

        #region Method
        public IActionResult Index()
        {

            var getCurrentUser = _consultantRepository.Find(x => x.Id == SessionFactory.CurrentUserId).FirstOrDefault();

            if (!string.IsNullOrEmpty(getCurrentUser.CometChatConsultantUid))
            {
                SessionFactory.CometChatUid = getCurrentUser.CometChatConsultantUid;
                string cometchatClist = _userRepository.GetCommetChatUserList(getCurrentUser.Id);
                SessionFactory.SessionCommetChatUserList = cometchatClist;
            }
            else
            {
                string UidName = IMMIWeb.Service.Service.CometChat.Service.CreateUser("consultant", getCurrentUser.Email, getCurrentUser.Mobile, (getCurrentUser.FirstName + getCurrentUser.LastName), getCurrentUser.ProfilePic, Convert.ToString(getCurrentUser.Id));
                

                IMMIWeb.Consultant consultantObjC = new IMMIWeb.Consultant();
                consultantObjC = getCurrentUser;
                consultantObjC.CometChatConsultantUid = UidName.ToLower();
                _consultantRepository.Update(consultantObjC);
                SessionFactory.CometChatUid = UidName;
            }


            List<AppointmentPendingViewModel> lstAppointmentPendingViewModel = new List<AppointmentPendingViewModel>();

            lstAppointmentPendingViewModel = _appointmentRepository.AppointmentPendingUser(SessionFactory.CurrentUserId).ToList();

            return View(lstAppointmentPendingViewModel);
        }
        public JsonResult ConsultantAcceptRequest(int appId)
        {
            string returnMsg = string.Empty;

            if (appId > 0)
            {
                Appointment appointment = _appointmentRepository.Find(x => x.Id == appId).FirstOrDefault();
                appointment.UserRequestTypeName = (int)MasterEnum.EUserRequestType.Approved;
                appointment.AppointmentStatusName = (int)MasterEnum.EAppointmentStatus.Consultant_Appointment_Accepted;
                _appointmentRepository.Update(appointment);

                returnMsg = "acceptTrue";

                //Open the Comment Code For Notification
                CommonInsertNotificationandSendNotificationparam paramnot = new CommonInsertNotificationandSendNotificationparam();

                paramnot.Header = "Appointment Accepted";
                paramnot.Body = "Appointment Accepted for New User Login";
                paramnot.Title = "Advenuss";
                paramnot.Description = "Appointment Accepted for New User Login";
                paramnot.UserId = appointment.UserId;
                paramnot.ConsultantId = SessionFactory.CurrentUserId;
                paramnot.NotificationTypeName = 12;

                Send.CommonInsertNotificationandSendNotification(paramnot);
            }
            else
            {
                returnMsg = "Invalid appoitment.";
            }

            return Json(returnMsg);
        }
        public JsonResult ConsultantRejectRequest(int appId,string reason)
        {
            string returnMsg = string.Empty;

            if (appId > 0 || !string.IsNullOrEmpty(reason))
            {
                var getAppointmentPayment = _appointmentPaymentRepository.Find(x => x.AppointmentId == appId).FirstOrDefault();
                var getAppointment = _appointmentRepository.Find(x => x.Id == appId).FirstOrDefault();

                //string status = _stripeRepository.RefundPayment(Convert.ToInt64(getAppointmentPayment.Amount), getAppointmentPayment.TransactionId);

                //if (status.ToLower().ToString() == "succeeded")
                //{
                    var getSlot = _consultantRepository.BookConsultantSlot(Convert.ToInt32(getAppointment.ConsultantSlotId), false);
                    Appointment appointmentObj = new Appointment();
                    appointmentObj = getAppointment;

                    appointmentObj.AppointmentStatusName = (int)MasterEnum.EAppointmentStatus.Consultant_Appointment_Rejected;
                    appointmentObj.UserRequestTypeName = (int)MasterEnum.EUserRequestType.Denied;
                    appointmentObj.CancelledByUserTypeName = Convert.ToInt32((int)MasterEnum.EUserType.Consultant);
                    appointmentObj.CancelledById = SessionFactory.CurrentUserId;
                    appointmentObj.CancellationDate = Convert.ToDateTime(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));

                    double tPer = 0.2;
                    decimal tVal = Convert.ToDecimal(tPer) * Convert.ToDecimal(getAppointmentPayment.Amount);

                //decimal penaltyCharges = ((decimal)getAppointmentPayment.Amount * (20 / 100) * 100);
                decimal penaltyCharges = tVal;

                //appointmentObj.RejectionAmount = penaltyCharges;
                appointmentObj.CancellationReason = reason;
                    _appointmentRepository.Update(appointmentObj);

                    int rejectCnt = 0;

                    var getConsultant = _consultantRepository.Find(x => x.Id == SessionFactory.CurrentUserId).FirstOrDefault();

                    IMMIWeb.Consultant consultantObj = new IMMIWeb.Consultant();

                    consultantObj = getConsultant;

                    if(consultantObj.RequestRejectionCount == null)
                    {
                        consultantObj.RequestRejectionCount = 0;
                    }

                    consultantObj.RequestRejectionCount = consultantObj.RequestRejectionCount + 1;
                    if(consultantObj.RequestRejectionCount >= Convert.ToInt32(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("GeneralInfo")["RejectionCountVal"]))
                    {
                        consultantObj.SuspendedBy = "Systematic";
                        consultantObj.IsSuspended = true;
                }
                    _consultantRepository.Update(consultantObj);


                    rejectCnt = Convert.ToInt32(consultantObj.RequestRejectionCount);

                    returnMsg = "rejectTrue";

                    if(rejectCnt >= Convert.ToInt32(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("GeneralInfo")["RejectionCountVal"]))
                    {
                        returnMsg = "rejectTrueWithLogout";
                    }

                //Open the Comment Code For Notification
                CommonInsertNotificationandSendNotificationparam paramnot = new CommonInsertNotificationandSendNotificationparam();

                paramnot.Header = "Appointment Rejected";
                paramnot.Body = "Appointment Rejected for New User Login";
                paramnot.Title = "Advenuss";
                paramnot.Description = "Appointment Rejected for New User Login";
                paramnot.UserId = appointmentObj.UserId;
                paramnot.ConsultantId = SessionFactory.CurrentUserId;
                paramnot.NotificationTypeName = 13;

                Send.CommonInsertNotificationandSendNotification(paramnot);

                //}
                //else
                //{
                //    returnMsg = "Invalid appoitment.";
                //}                
            }
            else
            {
                returnMsg = "Invalid appoitment.";
            }

            return Json(returnMsg);
        }
        public IActionResult ProfileDetails()
        {
            int id = (int)SessionFactory.CurrentUserId;

            var getCurrentConsultant = _consultantRepository.Find(x => x.Id == SessionFactory.CurrentUserId).FirstOrDefault();

            var exchangeRate = getCurrentConsultant != null ? Common.ExchangeRate(getCurrentConsultant.Country ?? 0) : 0;

            ConsultantDetailViewModel Model = new ConsultantDetailViewModel();

            var getConsultantData = _userRepository.GetConsultantDetails(id,0);

            getConsultantData.exchangerate = exchangeRate;

            Model.GetConsultantDetail = getConsultantData;

            return View(Model);
        }

        public List<NotificationMaster> ConsultantNotificationList()
        {
            List<NotificationMaster> result = new List<NotificationMaster>();

            var getCurrentConsultant = _consultantRepository.Find(x => x.Id == SessionFactory.CurrentUserId).FirstOrDefault();
            if (SessionFactory.CurrentUserId != 0)
            {
                result = _consultantRepository.ConsultantNotificationList(SessionFactory.CurrentUserId, getCurrentConsultant.CreatedOn.Date);
            }
            // result = _consultantRepository.ConsultantNotificationList(78);

            return result;

            //return result;
        }

        [HttpGet]
        public int GetNotificationCount()
        {

            int result = 0;
            var getCurrentConsultant = _consultantRepository.Find(x => x.Id == SessionFactory.CurrentUserId).FirstOrDefault();

            if(getCurrentConsultant != null)
            {
                result = _userRepository.GetNotificationCount(SessionFactory.CurrentUserId, getCurrentConsultant.CreatedOn.Date);
            }
           

            return result;


        }



        public IActionResult ChangeCommunicationLanguage(string languageName)
        {
            string[] languageParts = languageName.Split('|').Select(part => part.Trim()).ToArray();
            ViewBag.LanguageParts = languageParts;
            ConsultantViewModel Model = new ConsultantViewModel();
            var getLanguageList = _languageRepository.CommonLanguageList();
            Model.lstLanguage = _mapper.Map<List<CommonListViewModel>>(getLanguageList);
            return View(Model.lstLanguage);
        }

        [HttpPost]
        public IActionResult Updatecommunicationlanguage(string CommunicationLanguage)
        {
            if (!string.IsNullOrEmpty(CommunicationLanguage))
            {
                _consultantRepository.Updatecommunicationlanguage(CommunicationLanguage, SessionFactory.CurrentUserId);
            }
            var response = new { redirect = Url.Action("Index", "ConsultantHome") };
            return Json(response);
        }


        public IActionResult ChangeTypeOfService(string typeofserviceName)
        {
            var getTypeOfServiceList = _typeOfServiceRepository.CommonTypeOfServiceList();

            var getConsultantDetails = _consultantRepository.GetConsultantTypeOfServices(SessionFactory.CurrentUserId).ToList();

            //getTypeOfServiceList = getTypeOfServiceList.Where(x => x.CountryId == SessionFactory.CurrentUserCountryId).ToList();


            string[] serviceParts = typeofserviceName.Split('|').Select(part => part.Trim()).ToArray();
            ViewBag.ServiceParts = serviceParts;
            ConsultantViewModel Model = new ConsultantViewModel();

            //var getTypeofService = _typeOfServiceRepository.CommonTypeOfServiceList();
            Model.lstTypeOfService = _mapper.Map<List<CommonListViewModel>>(getConsultantDetails);
            
            return View(Model.lstTypeOfService);
        }

        [HttpPost]
        public IActionResult Updatetypeofservice(string Typeofservice)
        {
            if (!string.IsNullOrEmpty(Typeofservice))
            {
                _consultantRepository.Updatetypeofservice(Typeofservice, SessionFactory.CurrentUserId);
            }
            var response = new { redirect = Url.Action("Index", "ConsultantHome") };
            return Json(response);
        }

        public IActionResult RetentionFeeHike(decimal RetentionCharge, decimal AppCharge, decimal TaxCharge)
        {
            decimal totalCharge = RetentionCharge - AppCharge - TaxCharge;

            ViewBag.RetentionCharge = RetentionCharge;
            ViewBag.AppCharge = AppCharge;
            ViewBag.TaxCharge = TaxCharge;
            ViewBag.TotalCharge = totalCharge;
            return View();
        }

        [HttpPost]
        public IActionResult Addhikeretentionamount(decimal RetentionAmount)
        {
            bool result = false;
            var response = new object();
            if (RetentionAmount != 0)
            {
                var getCurrentConsultant = _consultantRepository.Find(x => x.Id == SessionFactory.CurrentUserId).FirstOrDefault();

                var exchangeRate = getCurrentConsultant != null ? Common.ExchangeRate(getCurrentConsultant.Country ?? 0) : 0;
                var retamount = Math.Round(RetentionAmount/exchangeRate,2);

                result = _consultantRepository.Addhikeretentionamount(retamount, SessionFactory.CurrentUserId);
            }
            if (result == true)
            {
                response = new { redirect = Url.Action("Index", "ConsultantHome") };
            }
            else
            {
                response = "";
            }
            
            return Json(response);
        }

        [HttpDelete]
        public IActionResult DeleteAccount()
        {
            bool result = false;
          result =  _consultantRepository.DeleteAccount(SessionFactory.CurrentUserId);

            return Json(result);
        }

        public IActionResult RatingReviewList()
        {
            ConsultantDetailViewModel Model = new ConsultantDetailViewModel();
            var getUserReviewData = _userRepository.GetConsultantReview(SessionFactory.CurrentUserId, sCountVal, eCountVal);
            if (getUserReviewData != null && getUserReviewData.Count() > 0)
            {
                //Model.lstGetConsultantReviewViewModel = _mapper.Map<List<GetConsultantReviewViewModel>>(getUserReviewData);
                Model.lstGetConsultantReviewViewModel = getUserReviewData;
            }
            return View(Model);

            //return result;
        }

        public JsonResult ConsultantRatingReviewLoadMore(int sCnt)
        {
            //var getCurrentConsultant = _consultantRepository.Find(x => x.Id == SessionFactory.CurrentUserId).FirstOrDefault();
            ConsultantDetailViewModel Model = new ConsultantDetailViewModel();
            int eCnt = sCnt + 2;

            var getUserReviewData = _userRepository.GetConsultantReview(SessionFactory.CurrentUserId, sCnt, eCnt);

            if (getUserReviewData != null && getUserReviewData.Count() > 0)
            {

                Model.lstGetConsultantReviewViewModel = getUserReviewData;
            }
            var data = Model.lstGetConsultantReviewViewModel;

            return Json(data);
        }

        public List<NotificationMaster> PaymentHistoryList()
        {
            List<NotificationMaster> result = new List<NotificationMaster>();



            var exchangerate = _countryRepository.GetExchangeRateCountryWise(SessionFactory.CurrentUserCountryId);
           


            if (SessionFactory.CurrentUserId != 0)
            {
                result = _consultantRepository.PaymentHistoryList(SessionFactory.CurrentUserId, exchangerate);
            }
            // result = _consultantRepository.ConsultantNotificationList(78);

            return result;

            //return result;
        }


        [HttpPost]
        public IActionResult AddWithdrawAmount(decimal WithDrawAmount)
        {
            int result = 0;
            string status = string.Empty;
            try
            {
                
                Withdraw withdrawObj = new Withdraw();
                var exchangerate = _countryRepository.GetExchangeRateCountryWise(SessionFactory.CurrentUserCountryId);
                var getCurrentConsultant = _consultantRepository.Find(x => x.Id == SessionFactory.CurrentUserId).FirstOrDefault();
                if (getCurrentConsultant != null && !string.IsNullOrEmpty(getCurrentConsultant.ConsultantStripeAccountId))
                {
                    withdrawObj.ConsultantId = SessionFactory.CurrentUserId;
                    withdrawObj.WithdrawAmount = WithDrawAmount / exchangerate;
                    withdrawObj.CreatedOn = DateTime.UtcNow;
                    withdrawObj.ConsultantStripeAccountId = getCurrentConsultant.ConsultantStripeAccountId;
                    withdrawObj.IsActive = true;

                    status = _stripeRepository.FundTransfer((long)withdrawObj.WithdrawAmount, withdrawObj.ConsultantStripeAccountId);

                    if(status == "200" || status == "OK")
                    {
                        result = _consultantRepository.AddWithdrawAmount(withdrawObj);

                        if (result != 0)
                        {



                            IMMIWeb.Consultant consultantObjC = new IMMIWeb.Consultant();
                            consultantObjC = getCurrentConsultant;
                            consultantObjC.EarnAmount = getCurrentConsultant.EarnAmount - withdrawObj.WithdrawAmount;
                            _consultantRepository.Update(consultantObjC);
                        }
                    }                                        
                }
            }
            catch(Exception x)
            {
                //throw;
                status = "";
            }
                       
           // result = _consultantRepository.AddWithdrawAmount(SessionFactory.CurrentUserId,WithDrawAmount, exchangerate);

            return Json(status);
        }


        [HttpPost]
        public bool SocialSendNotificationtoUser(int UserId, int ConsultantId, int NotificationId)
        {
            bool result = false;
            DbA976eeImmitestContext _dbContext = new DbA976eeImmitestContext();
            List<NotificationModel> objList = new List<NotificationModel>();

            try
            {
                if (UserId != 0 && ConsultantId != 0 && NotificationId != 0)
                {
                    var consultantname = _dbContext.Consultants.Where(m => m.Id == ConsultantId).Select(r => new
                    {
                        FullName = r.FirstName + " " + r.LastName
                    }).FirstOrDefault();

                    (from p in _dbContext.NotificationMasters
                     where p.SenderId == UserId && p.ReceiverId == ConsultantId && p.Id == NotificationId
                     select p).ToList()
                                                                     .ForEach(x =>
                                                                     {
                                                                         x.NotificationTypeName = 5;
                                                                         x.SenderId = ConsultantId;
                                                                         x.ReceiverId = UserId;
                                                                         x.Header = "Found a match";
                                                                         x.Body = consultantname.FullName + " is a valid candidate for your requirements.";
                                                                         x.SenderUserType = 3;
                                                                         x.ReceiverUserType = 2;
                                                                         x.IsRead = false;
                                                                     });


                    _dbContext.SaveChanges();

                    var ListofUser = _dbContext.Users.Where(d => d.Id == UserId && d.IsVerified == true && d.IsActive == true).Select(r => new
                    {
                        r.DeviceToken,
                        r.DeviceType,
                    }).ToList();

                    if (ListofUser.Count > 0)
                    {
                        NotificationModel obj = Common.SetNotificationModel("Found a match", consultantname + " is a valid candidate for your requirements.", ListofUser[0].DeviceToken, ListofUser[0].DeviceType);

                        Service.Service.Utility.SendNotification(obj);

                    }
                }

                result = true;
            }
            catch (Exception x)
            {
                result = false;
            }
            return result;

        }


        #endregion

    }
}
