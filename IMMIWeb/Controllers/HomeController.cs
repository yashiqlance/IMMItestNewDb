using AutoMapper;
using IMMIWeb.Service.Models;
using IMMIWeb.Service.Service.CMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using IMMIWeb.Infrastructure;
using IMMIWeb.Service.Service.User;
using IMMIWeb.Service.Service.Communication;
using IMMIWeb.Service.Service.Setting;
using Amazon.S3.Model.Internal.MarshallTransformations;
using IMMIWeb.Service.Service.General;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using static System.Net.WebRequestMethods;
using System.Security.Claims;
using NuGet.DependencyResolver;
using Microsoft.DotNet.Scaffolding.Shared.Project;
using Newtonsoft.Json;
using System.Net;
using IMMIWeb.Service.Service.Consultant;
using IMMIWeb.Service.Service.StripePay;
using IMMIWeb.Service.Service.Appointment;
using Microsoft.EntityFrameworkCore;
using Stripe;
using IMMIWeb.Service.Service.ReviewRating;
using Microsoft.AspNetCore.Http;

namespace IMMIWeb.Controllers
{
    [Authorize(Roles = "User")]
    public class HomeController : Controller
    {
        #region Fields

        private readonly ICMSRepository _cmsRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<HomeController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;
        private readonly IConsultantRepository _consultantRepository;
        private readonly ITypeOfServiceRepository _typeOfServiceRepository;
        private readonly IOtherRepository _otherRepository;
        private readonly IStripeRepository _stripeRepository;
        private readonly IAppointmentPaymentRepository _appointmentPaymentRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IReviewRatingRepository _reviewRatingRepository;
        private readonly ILanguageRepository _languageRepository;
        private readonly IAppLanguageRepository _appLanguageRepository;

        string OTPValidity = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("GeneralInfo")["OTPValidity"];

        int OTPAttemptVal = Convert.ToInt32(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("GeneralInfo")["OTPAttempt"]);
        int sCountVal = Convert.ToInt32(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("GeneralInfo")["sCountVal"]);
        int eCountVal = Convert.ToInt32(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("GeneralInfo")["eCountVal"]);

        #endregion

        #region Ctor

        public HomeController(
            Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment,
            ITypeOfServiceRepository typeOfServiceRepository,
            ILogger<HomeController> logger,
            ICMSRepository cmsRepository,
            IMapper mapper,
            IUserRepository userRepository,
            ICountryRepository countryRepository,
            IConsultantRepository consultantRepository,
            IOtherRepository otherRepository,
            IStripeRepository stripeRepository,
            IAppointmentPaymentRepository appointmentPaymentRepository,
            IAppointmentRepository appointmentRepository,
            IReviewRatingRepository reviewRatingRepository,
            IAppLanguageRepository appLanguageRepository,
            ILanguageRepository languageRepository)
        {
            _typeOfServiceRepository = typeOfServiceRepository;
            _cmsRepository = cmsRepository;
            _mapper = mapper;
            _logger = logger;
            _userRepository = userRepository;
            _countryRepository = countryRepository;
            _hostingEnvironment = hostingEnvironment;
            _consultantRepository = consultantRepository;
            _otherRepository = otherRepository;
            _stripeRepository = stripeRepository;
            _appointmentPaymentRepository = appointmentPaymentRepository;
            _appointmentRepository = appointmentRepository;
            _reviewRatingRepository = reviewRatingRepository;
            _languageRepository = languageRepository;
            _appLanguageRepository = appLanguageRepository;
        }

        #endregion

        #region Method

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult UserHomeIndex()
        {
            

            UserHomeViewModel Model = new UserHomeViewModel();

            List<GetConsultantByTosClangCountryViewModel> ListModelConsultant = new List<GetConsultantByTosClangCountryViewModel>();
            var getCurrentUser = _userRepository.Find(x => x.Id == SessionFactory.CurrentUserId).FirstOrDefault();

            if (!string.IsNullOrEmpty(getCurrentUser.CometChatUserUid))
            {
                SessionFactory.CometChatUid = getCurrentUser.CometChatUserUid;
                string cometchatClist = _consultantRepository.GetCommetChatConsultnatList(getCurrentUser.Id);
                SessionFactory.SessionCommetChatConsultnatList = cometchatClist;
            }
            else
            {
                string UidName = IMMIWeb.Service.Service.CometChat.Service.CreateUser("user", getCurrentUser.Email, getCurrentUser.Mobile, (getCurrentUser.FirstName + getCurrentUser.LastName), getCurrentUser.ProfilePic, Convert.ToString(getCurrentUser.Id));
                SessionFactory.CometChatUid = UidName;

                User userObjC = new User();
                userObjC = getCurrentUser;
                userObjC.CometChatUserUid = UidName.ToLower();
                _userRepository.Update(userObjC);
            }

            int tosId = Convert.ToInt32(getCurrentUser.TypeOfServiceName);
            int langId = Convert.ToInt32(getCurrentUser.CommunicationLanguage);
            int cntId = Convert.ToInt32(getCurrentUser.ImmigrationCountry);

            bool isGuest = false;
            if(getCurrentUser.IsGuest==true)
            {
                isGuest=true;
            }

            Model.lstModelConsultant = _consultantRepository.GetConsultantByTosClangCountry(tosId, langId, cntId, sCountVal, eCountVal, SessionFactory.CurrentUserId, isGuest).ToList();

            List<AppointmentPendingViewModel> ListModelAppoitment = new List<AppointmentPendingViewModel>();
            ListModelAppoitment = _appointmentRepository.AppointmentPendingUserById(SessionFactory.CurrentUserId).ToList();
            Model.lstModelAppoitment = ListModelAppoitment;

            return View(Model);
        }
        public JsonResult UserHomeIndexLoadMore(int sCnt)
        {
            var getCurrentUser = _userRepository.Find(x => x.Id == SessionFactory.CurrentUserId).FirstOrDefault();

            int tosId = Convert.ToInt32(getCurrentUser.TypeOfServiceName);
            int langId = Convert.ToInt32(getCurrentUser.CommunicationLanguage);
            int cntId = Convert.ToInt32(getCurrentUser.ImmigrationCountry);

            bool isGuest = false;
            if (getCurrentUser.IsGuest == true)
            {
                isGuest = true;
            }

            int eCnt = 1000;
            var data = _consultantRepository.GetConsultantByTosClangCountry(tosId, langId, cntId, sCnt, eCnt, SessionFactory.CurrentUserId, isGuest);

            return Json(data);
        }
        public IActionResult UserProfile()
        {
            int id = (int)SessionFactory.CurrentUserId;
            UserViewModel userViewModel = new UserViewModel();

            var getUserData = _userRepository.GetUserDetails(id);

            int getCardsCount = _userRepository.GetCardsCount(id);

            if (!string.IsNullOrEmpty(getUserData.ImmigrationCountry))
            {
                int immigrationCountryId;
                if (int.TryParse(getUserData.ImmigrationCountry, out immigrationCountryId))
                {
                    var immigrationCountryName = _countryRepository.GetCountryNameById(immigrationCountryId);
                    userViewModel.ImmigrationCountryName = immigrationCountryName;
                }
            }

            var userWithCountry = _userRepository.GetById(SessionFactory.CurrentUserId);

            if (userWithCountry != null)
            {
                int getBelongCountryId = (int)userWithCountry.Country;

                var belongCountryName = _countryRepository.GetCountryNameById(getBelongCountryId);

                userViewModel.BelongCountryName = belongCountryName;
            }

            userViewModel.GetUserDetails = getUserData;


            TempData["CardsCount"] = getCardsCount;

            return View(userViewModel);
        }
        public IActionResult EditUserProfile()
        {
            int id = (int)SessionFactory.CurrentUserId;
            UserViewModel userViewModel = new UserViewModel();

            var getUserData = _userRepository.GetUserDetails(id);

            userViewModel.GetUserDetails = getUserData;
            return View(userViewModel);
        }
        public IActionResult ConsultantDetail(int id)
        {
            var getCurrentUser = _userRepository.Find(x => x.Id == SessionFactory.CurrentUserId).FirstOrDefault();

           
            int cntId = Convert.ToInt32(getCurrentUser.ImmigrationCountry);

            var exchangeRate = Common.ExchangeRate(cntId);

            ConsultantDetailViewModel Model = new ConsultantDetailViewModel();

            var getUserReviewData = _userRepository.GetConsultantReview(id, sCountVal, eCountVal);
            var getConsultantData = _userRepository.GetConsultantDetails(id , SessionFactory.CurrentUserId);

            getConsultantData.exchangerate = exchangeRate;
            if (getUserReviewData != null && getUserReviewData.Count() > 0)
            {
                Model.lstGetConsultantReviewViewModel = getUserReviewData;
            }

            
            Model.SlotCount = _consultantRepository.GetConsultatnSlotDate(id,SessionFactory.TimeZone).Count();
            Model.ConsultantId = id;
            Model.GetConsultantDetail = getConsultantData;

            return View(Model);
        }
        public JsonResult ConsultantDetailLoadMore(int sCnt, int id)
        {
            //int eCnt = sCnt + 2;
            int eCnt = 1000;
            var data = _userRepository.GetConsultantReview(id, sCnt, eCnt);
            return Json(data);
        }
        public IActionResult BookConsultant(int id)
        {
            BookConsultantViewModel Model = new BookConsultantViewModel();

            var getConsultantData = _userRepository.GetConsultantDetails(id,SessionFactory.CurrentUserId);

            Model.GetConsultantDetail = getConsultantData;
            Model.lstConsultantDate = _consultantRepository.GetConsultatnSlotDate(id,SessionFactory.TimeZone);
            Model.UserCard = _userRepository.GetUserCardsDetail(SessionFactory.CurrentUserId);

            if (Model.lstConsultantDate == null || Model.lstConsultantDate.Count() == 0)
            {
                return RedirectToAction("UserHomeIndex", "Home");
            }
          
            return View(Model);
        }
        [HttpPost]
        public JsonResult GetConsultantTimeByDate(int ConsultantId, string Date)
        {
            if (ConsultantId > 0 && !string.IsNullOrEmpty(Date))
            {
                var response = _consultantRepository.GetConsultatnSlotTime(ConsultantId, Date, SessionFactory.TimeZone).ToList();

                if (response != null && response.Count() > 0)
                {
                    return Json(response);
                }
                else
                {
                    return Json("");
                }
            }
            else
            {
                return Json("");
            }
        }
        [HttpPost]
        public JsonResult BookUserAppointment(int ConsultantId, int ConsultantSlotId, int SessionModeType)
        {
            string returnMsg = string.Empty;

            #region Payment and book slot

            if (ConsultantId > 0 && ConsultantSlotId > 0)
            {
                var getValidConsultant = _consultantRepository.ValidConsultant(ConsultantId);
                var getSlot = _consultantRepository.BookConsultantSlot(ConsultantSlotId, true);

                if (getValidConsultant != null && getSlot == true)
                {
                    var getPrimaryCard = _userRepository.GetUserCardsDetail(SessionFactory.CurrentUserId);

                    if (getPrimaryCard != null)
                    {
                        long appointmentCharge = Convert.ToInt64(_otherRepository.GetCharges().AppointmentBookingCharges);

                        ChargeViewModel charge = new ChargeViewModel();
                        charge.Amount = appointmentCharge;
                        charge.Currency = "CAD";
                        charge.CardId = getPrimaryCard.StripeCardId;
                        charge.CustomerId = getPrimaryCard.StripeCustomerId;
                        charge.Desc = "Appointment Booked By '" + SessionFactory.CurrentUserName + "' To our Consultant '" + getValidConsultant.FirstName + " " + getValidConsultant.LastName + "' On '" + DateTime.UtcNow + "'  ";

                        PaymentResponse paymentResponse = new PaymentResponse();

                        //paymentResponse = _stripeRepository.MakePayment(charge);
                        paymentResponse.Status = "succeeded";
                        paymentResponse.Id = "1";

                        if (!string.IsNullOrEmpty(paymentResponse.Id) && paymentResponse.Status == "succeeded")
                        {
                            int appointmentId = 0;

                            #region Add Appointment

                            Appointment appointment = new Appointment();
                            appointment.UserId = SessionFactory.CurrentUserId;
                            appointment.ConsultantId = ConsultantId;
                            appointment.ConsultantSlotId = ConsultantSlotId;
                            appointment.CreatedOn = DateTime.UtcNow;
                            appointment.LastUpdatedOn = DateTime.UtcNow;
                            appointment.IsUpcoming = true;
                            appointment.UserRequestTypeName = (int)MasterEnum.EUserRequestType.Pending;
                            appointment.SessionMode = SessionModeType;
                            appointment.AppointmentStatusName = (int)MasterEnum.EAppointmentStatus.User_Request_For_Appointment;
                            appointment.AppointmentDate = _consultantRepository.GetConsultantSlotDate(ConsultantSlotId);
                            appointment.ServiceType = _userRepository.Find(x => x.Id == SessionFactory.CurrentUserId).FirstOrDefault().TypeOfServiceName;
                            appointment.ApplyForCountry = _userRepository.Find(x => x.Id == SessionFactory.CurrentUserId).FirstOrDefault().ImmigrationCountry;
                            appointment.ExtendCount = 0;

                            appointmentId = _appointmentRepository.BookAppointment(appointment);

                            #endregion

                            #region Add Payment

                            if (appointmentId > 0)
                            {
                                int appointmentPaymentId = 0;

                                AppointmentPayment appointmentPayment = new AppointmentPayment();
                                appointmentPayment.AppointmentId = appointmentId;
                                appointmentPayment.UserId = SessionFactory.CurrentUserId;
                                appointmentPayment.ConsultantId = ConsultantId;
                                appointmentPayment.Amount = appointmentCharge;
                                appointmentPayment.PaymentStatusName = (int)MasterEnum.EPaymentStatus.Success;

                                appointmentPayment.StripeCustomerId = getPrimaryCard.StripeCustomerId;
                                appointmentPayment.StripeCardId = getPrimaryCard.StripeCardId;
                                appointmentPayment.TransactionId = paymentResponse.Id;
                                appointmentPayment.CardId = getPrimaryCard.CardId;

                                appointmentPayment.IsPayment = true;
                                appointmentPayment.IsAct = true;
                                appointmentPayment.CreatedOn = DateTime.UtcNow;

                                appointmentPaymentId = _appointmentPaymentRepository.BookAppointmentPayment(appointmentPayment);

                                if (appointmentPaymentId > 0)
                                {
                                    returnMsg = "True";
                                }
                                else
                                {
                                    returnMsg = "There is some issue try after sometimes";
                                }
                            }


                            CommonInsertNotificationandSendNotificationparam paramnot = new CommonInsertNotificationandSendNotificationparam();

                            paramnot.Header = "Appointment Request";
                            paramnot.Body = "Appointment for New User Login";
                            paramnot.Title = "Advenuss";
                            paramnot.Description = "Appointment for New User Login";
                            paramnot.UserId = SessionFactory.CurrentUserId;
                            paramnot.ConsultantId = ConsultantId;
                            paramnot.NotificationTypeName = 6;




                            Send.CommonInsertNotificationandSendNotification(paramnot);

                            #endregion
                        }
                        else
                        {
                            returnMsg = "Something went wrong please try after sometime!";
                        }
                    }
                    else
                    {
                        returnMsg = "Please Add Card";
                    }
                }
                else
                {
                    returnMsg = "Sorry for the inconvenience caused. Please try later";
                }
            }
            else
            {
                returnMsg = "Invalid process";
            }
            return Json(returnMsg);

            #endregion

        }
        [HttpPost]
        public JsonResult VerifyEmail(string Email)
        {
            bool returnMsg = true;
            var getCurrentUser = _userRepository.Find(x => x.Id == SessionFactory.CurrentUserId).FirstOrDefault();
            User objUser = new User();
            objUser = getCurrentUser;
            Guid guid = Guid.NewGuid();
            string guidString = guid.ToString("N").Substring(0, 16).Replace("-", "");
            objUser.EmailVerificationToken = guidString;
            _userRepository.Update(objUser);


            #region Send Verification Code Mail

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}";
            string url = baseUrl.ToString() + "/UserAccount/NewEmailVerification?code=" + guidString + "&newEmail=" + Email + "";

            EmailTemplateMaker emailTemplateMaker = new EmailTemplateMaker(_hostingEnvironment);
            string[] paramArray;
            string imagePath = string.Empty;
            paramArray = new string[2] { imagePath, url };

            var emailConfirmationTemplate = emailTemplateMaker.GetTemplate("sendEmailVerificationLink", "VerifyEmail.html", paramArray);

            Common.SendMail("Advenuss Email Verification Link", emailConfirmationTemplate, Email);

            #endregion


            return Json(returnMsg);
        }

        //[AllowAnonymous]
        [HttpPost]
        //[Route("UserHome/AddFavouriteConsultant")]
        public int AddFavouriteConsultant(int ConsultantId)
        {

            int result = 0;
            //int UserId = 15;
            if (ConsultantId != 0)
            {
                result = _userRepository.AddFavouriteConsultant(ConsultantId, SessionFactory.CurrentUserId);
            }


            return result;

            //return result;
        }
        //[AllowAnonymous]
        [HttpPost]
        public int RemoveFavouriteConsultant(int ConsultantId)
        {
            //List<GetAvailableConsultantData> result = new List<GetAvailableConsultantData>();
            int result = 0;
            // int UserId = 15;
            if (ConsultantId != 0)
            {
                result = _userRepository.RemoveFavouriteConsultant(ConsultantId, SessionFactory.CurrentUserId);
            }


            return result;

            //return result;
        }

        [HttpPost]
        public JsonResult SubmitReview(RatingReviewConsultant model, int userId, int consultantId, int rating, string review)
        {
            if (ModelState.IsValid)
            {
                //var getUserReview = _userRepository.FetchReviewOfUser(userId, consultantId);

                //if (getUserReview == 0)
                //{
                    model.CreatedOn = DateTime.UtcNow;
                    model.IsActive = true;
                    model.Review = review;
                    model.Rating = rating;
                    model.UserId = userId;
                    model.ConsultantId = consultantId;

                    _userRepository.AddReview(model);

                //}
                return Json(new { success = true, message = "Review submitted successfully." });
            }
            else
            {
                // Return validation errors if necessary
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return Json(new { success = false, errors = errors });
            }
        }

        public List<NotificationMaster> UserNotificationList()
        {
            List<NotificationMaster> result = new List<NotificationMaster>();
            //SessionFactory.CurrentUserId
            var getCurrentUser = _userRepository.Find(x => x.Id == SessionFactory.CurrentUserId).FirstOrDefault();
            if (SessionFactory.CurrentUserId != 0)
            {
                result = _userRepository.UserNotificationList(SessionFactory.CurrentUserId, getCurrentUser.CreatedOn.Date);
            }


            return result;

            //return result;
        }

        [HttpGet]
        public int GetNotificationCount()
        {

            int result = 0;
            var getCurrentUser = _userRepository.Find(x => x.Id == SessionFactory.CurrentUserId).FirstOrDefault();
            if(getCurrentUser != null)
            {
                result = _userRepository.GetNotificationCount(SessionFactory.CurrentUserId, getCurrentUser.CreatedOn.Date);
            }
            


            return result;


        }

        [HttpGet]
        public JsonResult FetchReviewOfUser(int userId, int consultantId)
        {
            int reviewId = FetchReviewId(userId, consultantId);
            return Json(reviewId);
        }

        private int FetchReviewId(int userId, int consultantId)
        {
            var review = _userRepository.GetReviewIdForUserAndConsultant(userId, consultantId);

            return review;
        }

        [HttpGet]
        public JsonResult GetReviewDetails(int reviewId)
        {
            var reviewDetails = _reviewRatingRepository.GetById(reviewId);

            return Json(reviewDetails);
        }

        [HttpGet]
        public IActionResult GetFavouriteConsultantbyUserId()
        {
            List<GetFavouriteConsultantByUserIdViewModel> ListModelConsultant = new List<GetFavouriteConsultantByUserIdViewModel>();

            ListModelConsultant = _consultantRepository.GetFavouriteConsultantByUserId(SessionFactory.CurrentUserId).ToList();

            return View(ListModelConsultant);
        }

        public IActionResult UpdateUserInfomation()
        {

            User userObj = new User();
            var data = _userRepository.Find(x => x.Id == SessionFactory.CurrentUserId).FirstOrDefault();

            UserInformationUpdateViewModel model = new UserInformationUpdateViewModel();

            var getImmigrationCountry = _countryRepository.TypeOfServiceCountryList();
            model.lstCountryByTypeOfService = _mapper.Map<List<CommonListViewModel>>(getImmigrationCountry);

            var getTypeofService = _typeOfServiceRepository.CommonTypeOfServiceList();
            model.lstTypeOfService = _mapper.Map<List<CommonListViewModel>>(getTypeofService);

            var getCommunicationLanguage = _languageRepository.CommonLanguageList();
            model.lstLanguage = _mapper.Map<List<CommonListViewModel>>(getCommunicationLanguage);

            var getAppLanguage = _appLanguageRepository.CommonAppLanguageList();
            model.lstAppLanguage = _mapper.Map<List<CommonListViewModel>>(getAppLanguage);

            model.ImmigrationCountry = (int)data.ImmigrationCountry;
            model.CommunicationLanguage = (int)data.CommunicationLanguage;
            model.TypeOfServiceName = (int)data.TypeOfServiceName;
            //model.ApplicationLanguage = (int)data.ApplicationLanguage;

            return View(model);
        }
        [HttpPost]
        public IActionResult UpdateUserInfomation(UserInformationUpdateViewModel informationUpdateViewModel)
        {
            UserInformationUpdateViewModel model = new UserInformationUpdateViewModel();

            User userObj = new User();
            userObj = _userRepository.Find(x => x.Id == SessionFactory.CurrentUserId).FirstOrDefault();

            userObj.CommunicationLanguage = informationUpdateViewModel.CommunicationLanguage;
            userObj.TypeOfServiceName = informationUpdateViewModel.TypeOfServiceName;
            userObj.ImmigrationCountry = informationUpdateViewModel.ImmigrationCountry;
            userObj.ApplicationLanguage = informationUpdateViewModel.ApplicationLanguage;

            _userRepository.Update(userObj);

            var getImmigrationCountry = _countryRepository.TypeOfServiceCountryList();
            model.lstCountryByTypeOfService = _mapper.Map<List<CommonListViewModel>>(getImmigrationCountry);

            var getTypeofService = _typeOfServiceRepository.CommonTypeOfServiceList();
            model.lstTypeOfService = _mapper.Map<List<CommonListViewModel>>(getTypeofService);

            var getCommunicationLanguage = _languageRepository.CommonLanguageList();
            model.lstLanguage = _mapper.Map<List<CommonListViewModel>>(getCommunicationLanguage);

            var getAppLanguage = _appLanguageRepository.CommonAppLanguageList();
            model.lstAppLanguage = _mapper.Map<List<CommonListViewModel>>(getAppLanguage);

            model.ImmigrationCountry = (int)userObj.ImmigrationCountry;
            model.CommunicationLanguage = (int)userObj.CommunicationLanguage;
            model.TypeOfServiceName = (int)userObj.TypeOfServiceName;
            model.ApplicationLanguage = (int)userObj.ApplicationLanguage;

            return View(model);
        }

        [HttpPost]
        public JsonResult GetServiceByCountry(int countryId)
        {
            var getTypeOfServiceList = _typeOfServiceRepository.CommonTypeOfServiceList().Where(x => x.CountryId == countryId).ToList();

            return Json(getTypeOfServiceList);
        }

        [HttpPost]
        public JsonResult UserPresentInCall(string SessionId)
        {
            string response = string.Empty;

            if (!string.IsNullOrEmpty(SessionId))
            {
                HttpContext.Session.SetString("AppointmentSessionId", SessionId);

                var getAppointment = _appointmentRepository.Find(x => x.SlotSessionId == SessionId 
                && x.UserId == SessionFactory.CurrentUserId
                ).FirstOrDefault();

                if (getAppointment != null)
                {
                    Appointment appointment = new Appointment();
                    appointment = getAppointment;
                    appointment.IsUserPresent = true;
                    appointment.AppointmentStatusName = (int)MasterEnum.EAppointmentStatus.Working;
                    _appointmentRepository.Update(appointment);
                    response = "update the appointment by user true";
                }
            }

            return Json(response);
        }

        [HttpPost]
        public JsonResult UserExtendCall()
        {
            string response = string.Empty;
            string SessionId = HttpContext.Session.GetString("AppointmentSessionId");

            if (!string.IsNullOrEmpty(SessionId))
            {
                //HttpContext.Session.SetString("AppointmentSessionId", SessionId);

                var getAppointment = _appointmentRepository.Find(x => x.SlotSessionId == SessionId).FirstOrDefault();

                if (getAppointment != null)
                {
                    int cnt = Convert.ToInt32(getAppointment.ExtendCount) + 1;

                    Appointment appointment = new Appointment();
                    appointment = getAppointment;
                    appointment.ExtendCount = cnt;
                    _appointmentRepository.Update(appointment);
                    response = "true";
                }
            }

            return Json(response);
        }

        [HttpPost]
        public JsonResult AppointmentCallEndedUser(string SessionId)
        {

            string response = string.Empty;
            //string SessionId = HttpContext.Session.GetString("AppointmentSessionId");

            if (!string.IsNullOrEmpty(SessionId))
            {
                var getAppointment = _appointmentRepository.Find(x => x.SlotSessionId == SessionId).FirstOrDefault();
                var getConsultantData = _userRepository.GetConsultantDetails(Convert.ToInt32(getAppointment.ConsultantId),SessionFactory.CurrentUserId);
                
                response = getConsultantData.ConsultantId.ToString()+","+
                    getConsultantData.FirstName + "," + 
                    getConsultantData.LastName + "," + 
                    getConsultantData.ProfilePic+"," + 
                    getConsultantData.LanguageName + "," +
                    getConsultantData.ServiceName;
            }
            return Json(response);
        }


        [HttpPost]
        public JsonResult AppointmentChatEndedUser(string CUid)
        {

            string response = string.Empty;
            //string SessionId = HttpContext.Session.GetString("AppointmentSessionId");

            if (!string.IsNullOrEmpty(CUid))
            {
                int findCounsultantId = _consultantRepository.Find(x => x.CometChatConsultantUid == CUid).FirstOrDefault().Id;
                var getConsultantData = _userRepository.GetConsultantDetails(findCounsultantId,0);

                response = getConsultantData.ConsultantId.ToString() + "," +
                    getConsultantData.FirstName + "," +
                    getConsultantData.LastName + "," +
                    getConsultantData.ProfilePic + "," +
                    getConsultantData.LanguageName + "," +
                    getConsultantData.ServiceName;
            }
            return Json(response);
        }

        [HttpPost]
        public JsonResult GetConsultantDetailById(string id)
        {

            string response = string.Empty;
           
            if (!string.IsNullOrEmpty(id))
            {

                var getCurrentUser = _userRepository.Find(x => x.Id == SessionFactory.CurrentUserId).FirstOrDefault();


                int cntId = Convert.ToInt32(getCurrentUser.ImmigrationCountry);

                var exchangeRate = Common.ExchangeRate(cntId);

                var getConsultantData = _userRepository.GetConsultantDetails(Convert.ToInt32(id), SessionFactory.CurrentUserId);

                response = getConsultantData.ConsultantId.ToString() + "," +
                    getConsultantData.FirstName + "," +
                    getConsultantData.LastName + "," +
                    getConsultantData.ProfilePic + "," +
                    getConsultantData.LanguageName + "," +
                    getConsultantData.ServiceName + "," +  Math.Round(getConsultantData.RetainAmount * exchangeRate,2) + "," +
                    getConsultantData.UniqueId;
            }
            return Json(response);
        }


        #endregion

    }
}