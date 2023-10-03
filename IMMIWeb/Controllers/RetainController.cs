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
using Amazon.S3.Model;
using Amazon.S3;
using Amazon;
using FirebaseAdmin.Auth;
using IMMIWeb.Service.Service.Retains;
using Stripe.FinancialConnections;

namespace IMMIWeb.Controllers
{
    [Authorize(Roles = "User")]
    public class RetainController : Controller
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
        private readonly IRetainRepository _retainRepository;

        string OTPValidity = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("GeneralInfo")["OTPValidity"];

        int OTPAttemptVal = Convert.ToInt32(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("GeneralInfo")["OTPAttempt"]);
        int sCountVal = Convert.ToInt32(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("GeneralInfo")["sCountVal"]);
        int eCountVal = Convert.ToInt32(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("GeneralInfo")["eCountVal"]);

        #endregion

        #region Ctor

        public RetainController(
            Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment,
            ITypeOfServiceRepository typeOfServiceRepository,
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
            ILanguageRepository languageRepository,
            IRetainRepository retainRepository)
        {
            _typeOfServiceRepository = typeOfServiceRepository;
            _cmsRepository = cmsRepository;
            _mapper = mapper;
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
            _retainRepository = retainRepository;
        }

        #endregion

        #region Method


        [HttpPost]
        public JsonResult UserRetainToConsultant(int cId)
        {
            string response = "retainuser";

            HttpContext.Session.SetString("CurrentRetainConsultantId", Convert.ToString(cId));

            _otherRepository.AddUserRetainToConsultant(SessionFactory.CurrentUserId, cId);

            var getUserData = _userRepository.Find(x => x.Id == SessionFactory.CurrentUserId).FirstOrDefault();

            if (getUserData.IsGuest == true)
            {
                response = "guesteditforretain";
            }

            return Json(response);
        }

        public IActionResult GuestEditForRetain()
        {
            var getCurrentUser = _userRepository.Find(x => x.Id == SessionFactory.CurrentUserId).FirstOrDefault();

            UserViewModel userViewModel = new UserViewModel();

            var getCountryListByTypeOfService = _countryRepository.TypeOfServiceCountryList();
            userViewModel.lstCountryByTypeOfService = _mapper.Map<List<CommonListViewModel>>(getCountryListByTypeOfService);

            var getCountryList = _countryRepository.CommonCountryList();
            userViewModel.lstCountry = _mapper.Map<List<CommonListViewModel>>(getCountryList);

            var getLanguageList = _languageRepository.CommonLanguageList();
            userViewModel.lstLanguage = _mapper.Map<List<CommonListViewModel>>(getLanguageList);

            var getTypeOfServiceList = _typeOfServiceRepository.CommonTypeOfServiceList();
            userViewModel.lstTypeOfService = _mapper.Map<List<CommonListViewModel>>(getTypeOfServiceList);

            var getMobileList = _countryRepository.CommonCountryMobileCodeList();
            userViewModel.lstMobile = _mapper.Map<List<CommonListViewModel>>(getMobileList);

            userViewModel.Mobile = getCurrentUser.Mobile;
            userViewModel.lstCountryByTypeOfServiceVal = Convert.ToInt32(getCurrentUser.ImmigrationCountry);
            userViewModel.lstTypeOfServiceVal = Convert.ToInt32(getCurrentUser.TypeOfServiceName);
            userViewModel.lstMobileVal = getCurrentUser.MobileCountryCode;

            return View(userViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> GuestEditForRetain(UserViewModel userViewModel)
        {
            var getCurrentUser = _userRepository.Find(x => x.Id == SessionFactory.CurrentUserId).FirstOrDefault();
            int checkUserEmailExist = _userRepository.Find(x => x.Email == userViewModel.Email).Count();

            if (checkUserEmailExist == 0)
            {

                Guid guid = Guid.NewGuid();
                string guidString = guid.ToString("N").Substring(0, 16).Replace("-", "");
                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}";
                string url = baseUrl.ToString() + "/UserAccount/EmailVerification?code=" + guidString;

                User userObj = new User();

                userObj = getCurrentUser;

                userObj.FirstName = userViewModel.FirstName;
                userObj.LastName = userViewModel.LastName;
                userObj.Email = userViewModel.Email;
                userObj.Country = userViewModel.Country;
                userObj.CommunicationLanguage = userViewModel.CommunicationLanguage;
                string profilePicURL = await UploadFileAWS(userViewModel.imageUploadAWS);
                userObj.ProfilePic = profilePicURL;
                userObj.UserTypeVal = (int)MasterEnum.EUserType.User;
                userObj.EmailVerificationToken = guidString;

                _userRepository.Update(userObj);

                EmailTemplateMaker emailTemplateMaker = new EmailTemplateMaker(_hostingEnvironment);
                string[] paramArray;
                string imagePath = string.Empty;
                paramArray = new string[2] { imagePath, url };

                var emailConfirmationTemplate = emailTemplateMaker.GetTemplate("sendEmailVerificationLink", "VerifyEmail.html", paramArray);
                Common.SendMail("Advenuss Email Verification Link", emailConfirmationTemplate, userViewModel.Email);

                TempData["AlertMessageSuccess"] = "We just sent you verfication link on your mail " + userViewModel.Email + " Please verify.";

                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                SessionFactory.CurrentUserId = 0;
                SessionFactory.CurrentUserName = string.Empty;
                SessionFactory.CurrentUserEmail = string.Empty;
                SessionFactory.CurrentUserCountryId = 0;
                SessionFactory.CurrentUserMobile = string.Empty;
                SessionFactory.CurrentUserProfilePic = string.Empty;
                SessionFactory.CurrentUserMobileCountryCode = 0;

                //TempData["AlertMessageSuccess"] = "Logout successfully";
                return RedirectToAction("Login", "UserAccount");
            }
            else
            {

                UserViewModel Model = new UserViewModel();

                var getCountryListByTypeOfService = _countryRepository.TypeOfServiceCountryList();
                Model.lstCountryByTypeOfService = _mapper.Map<List<CommonListViewModel>>(getCountryListByTypeOfService);

                var getCountryList = _countryRepository.CommonCountryList();
                Model.lstCountry = _mapper.Map<List<CommonListViewModel>>(getCountryList);

                var getLanguageList = _languageRepository.CommonLanguageList();
                Model.lstLanguage = _mapper.Map<List<CommonListViewModel>>(getLanguageList);

                var getTypeOfServiceList = _typeOfServiceRepository.CommonTypeOfServiceList();
                Model.lstTypeOfService = _mapper.Map<List<CommonListViewModel>>(getTypeOfServiceList);

                var getMobileList = _countryRepository.CommonCountryMobileCodeList();
                Model.lstMobile = _mapper.Map<List<CommonListViewModel>>(getMobileList);

                Model.Mobile = getCurrentUser.Mobile;
                Model.lstCountryByTypeOfServiceVal = Convert.ToInt32(getCurrentUser.ImmigrationCountry);
                Model.lstTypeOfServiceVal = Convert.ToInt32(getCurrentUser.TypeOfServiceName);
                Model.lstMobileVal = getCurrentUser.MobileCountryCode;

                return View(Model);
            }
        }

        public IActionResult RetainUser()
        {

            var getCurrentUser = _userRepository.GetUserProfileDetails(SessionFactory.CurrentUserId);

            if (getCurrentUser.IsGuest == true)
            {
                return RedirectToAction("GuestEditForRetain");
            }


            RetainUploadDocumentViewModel Model = new RetainUploadDocumentViewModel();

            Model.UserId = getCurrentUser.UserId;
            Model.UserName = getCurrentUser.UserFirstName + " " + getCurrentUser.UserLastName;
            Model.UserEmail = getCurrentUser.UserEmail;
            Model.UserMobile = getCurrentUser.UserMobile;
            Model.UserMobileCountryCode = getCurrentUser.UserMobileCountryCode;
            Model.UserProfilePic = getCurrentUser.ProfilePic;
            Model.UserTypeOfService = getCurrentUser.TypeofServiceName;
            Model.UserImmigrationCountry = getCurrentUser.ImmigrationCountry;
            Model.UserResidenceCountry = getCurrentUser.Country;
            Model.UserLanguage = getCurrentUser.CommunicationLanguage;
            Model.UniqueId = getCurrentUser.UniqueId;

            return View(Model);
        }

        [HttpPost]
        public IActionResult RetainUser(string userDoc, int cId, int PaymentType, string emiDates)
        {
            RetainUploadDocumentViewModel Model = new RetainUploadDocumentViewModel();

            var getUserDetail = _userRepository.Find(x => x.Id == SessionFactory.CurrentUserId).FirstOrDefault();
            var getConsultantDetail = _consultantRepository.Find(x => x.Id == cId).FirstOrDefault();
            var getPrimaryCard = _userRepository.GetUserCardsDetail(SessionFactory.CurrentUserId);
            var getCharges = _otherRepository.GetCharges();
            double emiTotalAmount = 0;

            if (getPrimaryCard != null)
            {
                long appointmentCharge = 0;

                if (getConsultantDetail.RetainAmount == 0 || getConsultantDetail.RetainAmount == null)
                {
                    appointmentCharge = (long)getCharges.RetainProcessCharges;
                }
                else
                {
                    appointmentCharge = (long)getConsultantDetail.RetainAmount;
                }

                if (PaymentType == (int)MasterEnum.EPaymentMode.EMI)
                {
                    double tPer = 0.2;
                    double ePer = 0.8;
                    double tVal = tPer * appointmentCharge;
                    double eVal = ePer * appointmentCharge;
                    appointmentCharge = (long)tVal;
                    emiTotalAmount = eVal / 3;
                }


                ChargeViewModel charge = new ChargeViewModel();
                charge.Amount = appointmentCharge;
                charge.Currency = "CAD";
                charge.CardId = getPrimaryCard.StripeCardId;
                charge.CustomerId = getPrimaryCard.StripeCustomerId;
                charge.Desc = "Retain Process By '" + SessionFactory.CurrentUserName + "' To our Consultant '" + getConsultantDetail.FirstName + " " + getConsultantDetail.LastName + "' On '" + DateTime.UtcNow + "'  ";

                PaymentResponse paymentResponse = new PaymentResponse();
                //paymentResponse = _stripeRepository.MakePayment(charge);
                paymentResponse.Id = "1";
                paymentResponse.Status = "succeeded";

                if (!string.IsNullOrEmpty(paymentResponse.Id) && paymentResponse.Status == "succeeded")
                {
                    decimal amount = appointmentCharge;
                    decimal percentage = Convert.ToDecimal(getCharges.AdminCommissionRate);

                    decimal consultantAmount = (percentage / 100) * amount;

                    int retainId = 0;
                    int retainPaymentId = 0;

                    // Retain Start 
                    Retain retainOb = new Retain();
                    retainOb.ConsultantId = cId;
                    retainOb.UserId = SessionFactory.CurrentUserId;
                    retainOb.CreatedOn = DateTime.UtcNow;
                    retainOb.IsAct = true;
                    retainOb.RetainTypeOfService = Convert.ToInt32(getUserDetail.TypeOfServiceName);
                    retainOb.RetainCountryForService = Convert.ToInt32(getUserDetail.ImmigrationCountry);
                    retainOb.RetainCommunicationLanguage = Convert.ToInt32(getUserDetail.CommunicationLanguage);

                    retainId = _retainRepository.AddRetainConsultant(retainOb);
                    // Retain End

                    if (retainId > 0)
                    {

                        if (PaymentType == (int)MasterEnum.EPaymentMode.FullSwipe)
                        {
                            // Retain Payment Start

                            RetainPayment retainPayment = new RetainPayment();
                            retainPayment.RetainId = retainId;
                            retainPayment.RetainAmount = amount;
                            retainPayment.PaymentModeName = (int)MasterEnum.EPaymentMode.FullSwipe;
                            retainPayment.RetainStripeAccountId = getPrimaryCard.StripeCardId;
                            retainPayment.TransactionId = paymentResponse.Id;
                            retainPayment.ForNumberOfEmi = 0;
                            retainPayment.CreatedOn = DateTime.UtcNow;
                            retainPayment.RetainPaymentStatus = (int)MasterEnum.EPaymentStatus.Success;

                            retainPaymentId = _retainRepository.AddRetainConsultantPayment(retainPayment);

                            // Retain Payment End
                        }
                        else
                        {
                            // EMITable Start 

                            List<Emitable> lstEmitable = new List<Emitable>();
                            int cntEmi = 0;
                            Emitable emiTable = new Emitable();
                            emiTable.RetainId = retainId;
                            emiTable.StripeCustomerId = getPrimaryCard.StripeCustomerId;
                            emiTable.StripeCardId = getPrimaryCard.StripeCardId;
                            emiTable.ForNumberOfEmi = 0;
                            emiTable.TotalAmount = appointmentCharge;
                            emiTable.PaidAmount = amount;
                            emiTable.Emicount = cntEmi;
                            emiTable.IsActive = true;
                            emiTable.Emiamount = 0;
                            emiTable.CreatedOn = DateTime.UtcNow;
                            emiTable.EmiPaymentStatus = (int)MasterEnum.EPaymentStatus.Success;
                            emiTable.IsEmiPaymentPaid = true;
                            lstEmitable.Add(emiTable);

                            List<string> emiDateList = emiDates.Split(',').ToList<string>();

                            foreach (var itemDate in emiDateList)
                            {
                                cntEmi = cntEmi + 1;
                                Emitable emiTableObj = new Emitable();

                                emiTableObj.RetainId = retainId;
                                emiTableObj.Emicount = cntEmi;
                                emiTableObj.Emiamount = (decimal)emiTotalAmount;
                                emiTableObj.CreatedOn = DateTime.UtcNow;
                                emiTableObj.EmiPaymentStatus = (int)MasterEnum.EPaymentStatus.Pending;
                                emiTableObj.EmiDate = DateTimeOffset.Parse(itemDate).UtcDateTime;
                                emiTableObj.StripeCustomerId = getPrimaryCard.StripeCustomerId;
                                emiTableObj.StripeCardId = getPrimaryCard.StripeCardId;

                                //emiTableObj.ForNumberOfEmi = 3;
                                //emiTableObj.TotalAmount = appointmentCharge;
                                //emiTableObj.PaidAmount = amount;
                                //emiTableObj.IsEmiPaymentPaid = true;
                                //emiTableObj.IsActive = true;

                                lstEmitable.Add(emiTableObj);
                            }

                            _retainRepository.AddEmiTable(lstEmitable);
                            // EMITable End
                        }


                        CommonInsertNotificationandSendNotificationparam paramnot = new CommonInsertNotificationandSendNotificationparam();

                        paramnot.Header = "Appointment Retain";
                        paramnot.Body = "Retaining Consultant By User";
                        paramnot.Title = "Advenuss";
                        paramnot.Description = "Retaining Consultant By User";
                        paramnot.UserId = SessionFactory.CurrentUserId;
                        paramnot.ConsultantId = cId;
                        paramnot.NotificationTypeName = 8;

                        Send.CommonInsertNotificationandSendNotification(paramnot);

                        #region Retain Documents

                        if (!string.IsNullOrEmpty(userDoc))
                        {
                            string inputString = userDoc.Substring(0, userDoc.Length - 1);
                            string[] stringArray = inputString.Split(',');

                            List<UserDocument> lstUserAddDocument = new List<UserDocument>();

                            foreach (var item in stringArray)
                            {

                                UserDocument userAddDocument = new UserDocument();
                                string fileName = Path.GetFileName(item);
                                string fileExtension = Path.GetExtension(new Uri(item).LocalPath);

                                userAddDocument.UserId = SessionFactory.CurrentUserId;
                                userAddDocument.Filename = fileName;
                                userAddDocument.Size = null;
                                userAddDocument.Extensions = fileExtension;
                                userAddDocument.RetainId = retainId;
                                userAddDocument.CreatedOn = DateTime.UtcNow;
                                userAddDocument.IsActive = true;
                                userAddDocument.DocUrl = item;

                                lstUserAddDocument.Add(userAddDocument);
                            }


                            if (lstUserAddDocument != null && lstUserAddDocument.Count() > 0)
                            {
                                _retainRepository.AddUserDocuments(lstUserAddDocument, SessionFactory.CurrentUserId, cId);

                                CommonInsertNotificationandSendNotificationparam paramnots = new CommonInsertNotificationandSendNotificationparam();

                                paramnots.Header = "Add/Edit Documents";
                                paramnots.Body = "User Add/Edit Documents";
                                paramnots.Title = "Advenuss";
                                paramnots.Description = "User Add/Edit Documents";
                                paramnots.UserId = SessionFactory.CurrentUserId;
                                paramnots.ConsultantId = cId;
                                paramnots.NotificationTypeName = 10;

                                Send.CommonInsertNotificationandSendNotification(paramnots);


                            }
                        }

                        #endregion

                        return RedirectToAction("UserHomeIndex", "Home");
                    }
                }
                else
                {
                    Model.ErrorMessage = "Payment unsuccessful ";
                }
            }
            else
            {
                Model.ErrorMessage = "Card not found";
            }

            var getCurrentUser = _userRepository.GetUserProfileDetails(SessionFactory.CurrentUserId);

            Model.UserId = getCurrentUser.UserId;
            Model.UserName = getCurrentUser.UserFirstName + " " + getCurrentUser.UserLastName;
            Model.UserEmail = getCurrentUser.UserEmail;
            Model.UserMobile = getCurrentUser.UserMobile;
            Model.UserMobileCountryCode = getCurrentUser.UserMobileCountryCode;
            Model.UserProfilePic = getCurrentUser.ProfilePic;
            Model.UserTypeOfService = getCurrentUser.TypeofServiceName;
            Model.UserImmigrationCountry = getCurrentUser.ImmigrationCountry;
            Model.UserResidenceCountry = getCurrentUser.Country;
            Model.UserLanguage = getCurrentUser.CommunicationLanguage;

            return View(Model);

        }

        [HttpPost]
        public async Task<JsonResult> UploadFileForRetain(IFormFile file)
        {
            string response = string.Empty;

            string newUrl = string.Empty;

            string myBucketName = "guardian-temp"; // your S3 bucket name goes here
            //string s3DirectoryName = "";
            string s3FileName = file.FileName;

            var AWSAccessKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AWSAccessKey")["Key"];
            var AWSSecretKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AWSSecretKey")["Key"];
            IAmazonS3 client = new AmazonS3Client(AWSAccessKey, AWSSecretKey, RegionEndpoint.USEast2);
            var request = new PutObjectRequest()
            {
                BucketName = myBucketName,
                Key = s3FileName,
                InputStream = file.OpenReadStream()
            };

            request.Metadata.Add("Content-Type", file.ContentType);
            PutObjectResponse returnvalue = await client.PutObjectAsync(request);

            if (returnvalue.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                var urlRequest = new GetPreSignedUrlRequest()
                {
                    BucketName = myBucketName,
                    Key = s3FileName,
                    Protocol = Protocol.HTTPS,
                    Expires = DateTime.UtcNow.AddMinutes(20)
                };
                var PresignedUrl = client.GetPreSignedURL(urlRequest);
                newUrl = PresignedUrl;

                int index = PresignedUrl.LastIndexOf(file.FileName); // Find the last index of ".jpg"

                if (index >= 0)
                {
                    newUrl = PresignedUrl.Substring(0, index) + file.FileName;
                }
            }

            response = newUrl;

            return Json(response);
        }

        [HttpPost]
        public async Task<string> UploadFileAWS(IFormFile file)
        {
            string newUrl = string.Empty;

            if (file == null || file.Length == 0)
            {
                return newUrl;
            }
            string myBucketName = "guardian-temp"; // your S3 bucket name goes here
            //string s3DirectoryName = "";
            string s3FileName = file.FileName;

            var AWSAccessKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AWSAccessKey")["Key"];
            var AWSSecretKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AWSSecretKey")["Key"];
            IAmazonS3 client = new AmazonS3Client(AWSAccessKey, AWSSecretKey, RegionEndpoint.USEast2);
            var request = new PutObjectRequest()
            {
                BucketName = myBucketName,
                Key = s3FileName,
                InputStream = file.OpenReadStream()
            };

            request.Metadata.Add("Content-Type", file.ContentType);
            PutObjectResponse returnvalue = await client.PutObjectAsync(request);

            if (returnvalue.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                var urlRequest = new GetPreSignedUrlRequest()
                {
                    BucketName = myBucketName,
                    Key = s3FileName,
                    Protocol = Protocol.HTTPS,
                    Expires = DateTime.UtcNow.AddMinutes(20)
                };
                var PresignedUrl = client.GetPreSignedURL(urlRequest);
                newUrl = PresignedUrl;

                int index = PresignedUrl.LastIndexOf(file.FileName); // Find the last index of ".jpg"

                if (index >= 0)
                {
                    newUrl = PresignedUrl.Substring(0, index) + file.FileName;
                }
            }
            return newUrl;
        }


        public IActionResult ConsultantRetain()
        {
            ConsultantRetainViewModel model = new ConsultantRetainViewModel();

            List<AppointmentPendingViewModel> ListModelAppoitment = new List<AppointmentPendingViewModel>();
            ListModelAppoitment = _appointmentRepository.AppointmentPendingUserById(SessionFactory.CurrentUserId).ToList();
            model.lstModelAppoitment = ListModelAppoitment;

            List<AppointmentHistoryViewModel> ListHistoryAppoitment = new List<AppointmentHistoryViewModel>();
            ListHistoryAppoitment = _appointmentRepository.AppointmentHistoryUserById(SessionFactory.CurrentUserId).ToList();
            model.lstHistoryAppoitment = ListHistoryAppoitment;

            List<AppointmentRetainViewModel> ListRetainAppoitment = new List<AppointmentRetainViewModel>();
            ListRetainAppoitment = _appointmentRepository.AppointmentRetainUserById(SessionFactory.CurrentUserId).ToList();
            model.lstRetainAppoitment = ListRetainAppoitment;

            return View(model);
        }

        [HttpPost]
        public JsonResult cancelAppointment(int appointmentId)
        {
            string response = string.Empty;

            if (!string.IsNullOrEmpty(appointmentId.ToString()))
            {
                HttpContext.Session.SetString("AppointmentId", appointmentId.ToString());

                var getAppointment = _appointmentRepository.Find(x => x.Id == appointmentId
                && x.UserId == SessionFactory.CurrentUserId
                ).FirstOrDefault();

                if (getAppointment != null)
                {
                    Appointment appointment = new Appointment();
                    appointment = getAppointment;
                    appointment.CancelledByUserTypeName = (int)MasterEnum.EUserType.User;
                    appointment.CancelledById = SessionFactory.CurrentUserId;
                    appointment.CancellationDate = DateTime.UtcNow;
                    //appointment.IsCancel = true;
                    _appointmentRepository.Update(appointment);
                    response = "update the appointment by user true";

                    CommonInsertNotificationandSendNotificationparam paramnot = new CommonInsertNotificationandSendNotificationparam();

                    paramnot.Header = "Appointment Cancel";
                    paramnot.Body = "Appointment Canceled By User";
                    paramnot.Title = "Advenuss";
                    paramnot.Description = "Appointment Canceled By User";
                    paramnot.UserId = SessionFactory.CurrentUserId;
                    paramnot.ConsultantId = (int)getAppointment.ConsultantId;
                    paramnot.NotificationTypeName = 11;

                    Send.CommonInsertNotificationandSendNotification(paramnot);



                }
            }

            return Json(response);
        }

        public IActionResult UserRetainConsultantDetails(int ConsultantId)
        {
            ConsultantRetainViewModel model = new ConsultantRetainViewModel();


            //List<RetainDetailsByUserViewModel> ListRetainDetailsAppoitment = new List<RetainDetailsByUserViewModel>();
            var getUserReviewData = _userRepository.GetConsultantReview(ConsultantId, sCountVal, eCountVal);
            var result = _appointmentRepository.RetainDetailsUserById(SessionFactory.CurrentUserId,ConsultantId);
            var userDocuments = _userRepository.GetUserDocuments(SessionFactory.CurrentUserId);
            model.lstRetainDetailsAppoitment = result;
            model.lstGetConsultantReviewViewModel = getUserReviewData;
            model.lstUserDocuments = userDocuments;

            return View(model);
        }

        [HttpPost]
        public bool RemoveUserDocument(string FileName, string FileExtension,int UserId)
        {
            
          var result =  _retainRepository.RemoveUserDocument(FileName, FileExtension, SessionFactory.CurrentUserId);
          return result;
            
        }


        #endregion

    }
}
